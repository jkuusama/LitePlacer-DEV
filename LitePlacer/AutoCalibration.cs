using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using LitePlacer.Properties;
using MathNet.Numerics.LinearRegression;

namespace LitePlacer {
    public class AutoCalibration {

        public static void DoNeedleErrorMeasurement(VideoProcessing vp) {
            var cnc = Global.Instance.cnc;
            var originalLocation = cnc.XYALocation;
            PartLocation testLocation = new PartLocation(Settings.Default.UpCam_PositionX, Settings.Default.UpCam_PositionY);
            cnc.Zup();
            cnc.CNC_XY_m(testLocation);

            cnc.ZGuardOff();
            cnc.Zdown();

            // setup camera
            vp.SetFunctionsList("needle");
            vp.FindCircles = true;
            vp.Draw1mmGrid = true;
            var Needle = Global.Instance.needle;

            List<PartLocation> offsetError = new List<PartLocation>();
            for (int i = 0; i <= 360; i += 90) {
                testLocation.A = i;
                var pp = testLocation - Needle.NeedleOffset;

                Needle.Move_m(pp); // move to target
                // mark erro
                var circle = VideoDetection.GetClosestCircle(vp, 10);
                if (circle != null) {
                    vp.MarkA.Add(circle.ToScreenResolution().ToPartLocation().ToPointF());

                    circle.ToMMResolution();
                    offsetError.Add(circle.ToPartLocation());
                }
                Thread.Sleep(500); //wait 1 second
            }

            Global.Instance.mainForm.ShowSimpleMessageBox("Upcamera offset Error is " + PartLocation.Average(offsetError) + "\nMax Error is " + PartLocation.MaxValues(offsetError));

            cnc.ZGuardOn(); //Needle up, move back
            cnc.Zup();
            cnc.CNC_XYA_m(originalLocation);

            //revert to previous state
            vp.Reset();
        }


        public static PointF DownCamera_Calibration(CameraView cv, double moveDistance) {
            var cnc = Global.Instance.cnc;

            cv.SetDownCameraFunctionSet("Homing");
            // move to upcamera position
            cnc.Zup();
            Global.Instance.mainForm.GotoUpCamPosition_button_Click(null, null);
            cnc.Zdown();
            // do calibration
            var ret = DoCameraCalibration(cv.downVideoProcessing, new PartLocation(moveDistance, moveDistance));
            cnc.Zup();
            cv.SetDownCameraFunctionSet("");
            return ret;
        }

        public static PointF UpCamera_Calibration(CameraView cv, double moveDistance) {
            var cnc = Global.Instance.cnc;

            cv.SetUpCameraFunctionSet("Needle");
            // move to upcamera position
            cnc.Zup();
            Global.Instance.mainForm.GotoUpCamPosition_button_Click(null, null);
            cnc.Zdown();
            // do calibration
            var ret = DoCameraCalibration(cv.upVideoProcessing, new PartLocation(moveDistance, moveDistance));
            cnc.Zup();
            cv.SetUpCameraFunctionSet("");
            return ret;
        }


        private static PointF DoCameraCalibration(VideoProcessing vp, PartLocation movement) {
            var cnc = Global.Instance.cnc;
            List<PartLocation> distance = new List<PartLocation>();
            List<PartLocation> pixels = new List<PartLocation>();

            // turn on slack compensation
            var savedSlackCompensation = cnc.SlackCompensation;
            cnc.SlackCompensation = true;
            cnc.Zup();

            var startingPos = cnc.XYLocation;
            startingPos.A = 0;

            var zoom = vp.GetZoom();
            vp.MarkA.Clear();

            for (int i = -4; i < 5; i++) {
                //move
                var newLocation = startingPos + (i * movement);
                cnc.CNC_XYA_m(newLocation);

                //try 5 times to find a circle
                List<Shapes.Circle> circles = new List<Shapes.Circle>();
                for (int tries = 5; tries > 0 && circles.Count == 0; tries--)
                    circles = VideoDetection.FindCircles(vp);
                if (circles.Count == 0) continue; //not found, move and try again

                //find largest circle of the bunch
                var circle = circles.Aggregate((c, d) => c.Radius > d.Radius ? c : d); //find largest circle if we have multiple 
                //  var circlePL = (1 / zoom) * circle.ToPartLocation(); //compensate for zoom
                circle.ToScreenUnzoomedResolution();
                distance.Add(newLocation);
                pixels.Add(circle.ToPartLocation());

                vp.MarkA.Add(circle.Clone().ToScreenResolution().ToPartLocation().ToPointF());
                //DisplayText(String.Format("Actual Loc = {0}\t Measured Loc = {1}", newLocation, UpCamera.PixelsToActualLocation(circlePL)), Color.Blue);
            }


            double XmmPerPixel = 0, YmmPerPixel = 0;
            if (pixels.Count < 2) {
                Global.Instance.mainForm.ShowMessageBox("Unable To Detect Circles",
                    "Try to adjust upcamera processing to see circles, and ensure upcamera needle position is correctly configured",
                    MessageBoxButtons.OK);
            } else {
                // Do regression on X and Y 
                var Xs = pixels.Select(xx => xx.X).ToArray();
                var Ys = distance.Select(xx => xx.X).ToArray();
                var result = SimpleRegression.Fit(Xs, Ys);
                XmmPerPixel = result.Item2;

                Xs = pixels.Select(xx => xx.Y).ToArray();
                Ys = distance.Select(xx => xx.Y).ToArray();
                result = SimpleRegression.Fit(Xs, Ys);
                YmmPerPixel = result.Item2;


                Global.Instance.DisplayText(String.Format("{0} Xmm/pixel   {1} Ymm/pixel", XmmPerPixel, YmmPerPixel), Color.Purple);

                // Now move to the center
                /* need to get gotolocation upcamera working still
                 double X, Y; //error offset
                 GotoUpCamPosition_button_Click(null, null);
                 for (int tries = 5; tries > 0; tries--) {
                     if (GoToLocation_m(UpCamera, Shapes.ShapeTypes.Circle, 1.8, 0.5, out X, out Y)) {
                         Properties.Settings.Default.UpCam_PositionX = Cnc.CurrentX + X;
                         Properties.Settings.Default.UpCam_PositionY = Cnc.CurrentY - Y;
                         UpcamPositionX_textBox.Text = Properties.Settings.Default.UpCam_PositionX.ToString("0.00", CultureInfo.InvariantCulture);
                         UpcamPositionY_textBox.Text = Properties.Settings.Default.UpCam_PositionY.ToString("0.00", CultureInfo.InvariantCulture);

                     }
                 }
                 */

            }

            //restore settings
            cnc.SlackCompensation = savedSlackCompensation;
            //return value
            return new PointF((float)Math.Abs(XmmPerPixel), (float)Math.Abs(YmmPerPixel));
        }


        /*

        public void MeasureSlack() {
            //move in one direction, measure circle, move in opposite direction, measure where we
            //are vs. where we ares upposed to be - the difference is the slack in the system in that direction

            int slack_xltor = 1, slack_xrtol = 3, slack_yttob = 4, slack_ybtot = 2;
            double[] slack = new double[4];

            PartLocation moveVector = new PartLocation(0, 3);
            PartLocation slackMoveVector = new PartLocation(0, .5);
            PartLocation startLocation = Cnc.XYLocation;

            // temp turn off zoom
            var savedZoom = DownCamera.Zoom;
            DownCamera.Zoom = false;

            for (int i = 0; i < 4; i++) {
                double angle = Math.PI / 2;
                CNC_XY_m(startLocation + moveVector.Rotate(angle));
                slack[i] = MeasureSlack(slackMoveVector.Rotate(angle));
            }

            SlackMeasurement_label.Text = String.Format("Slack X:{0:0.###}/{1:0.###}\nSlack Y:{2:0.###}/{3:0.###}", slack[slack_xltor], slack[slack_xrtol], slack[slack_yttob], slack[slack_ybtot]);

            Console.WriteLine("slack x : {0} / {1}", slack_xltor, slack_xrtol);
            Console.WriteLine("slack y : {0} / {1}", slack_ybtot, slack_yttob);

            DownCamera.Zoom = savedZoom;

        }

    
        
        private float MeasureSlack(PartLocation delta) {
            Thread.Sleep(150);

            var start_loc = Cnc.XYLocation;

            var circle1 = MeasureCircle();
            CNC_XY_m(start_loc + delta);
            var circle2 = MeasureCircle();

            var circleMove = circle2 - circle1;

            double zoom = DownCamera.GetMeasurementZoom();
            circleMove.X *= (float)(Properties.Settings.Default.DownCam_XmmPerPixel / zoom);
            circleMove.Y *= (float)(Properties.Settings.Default.DownCam_YmmPerPixel / zoom);

            return (float)(delta.VectorLength() - circleMove.VectorLength());
        }
    */



    }
}

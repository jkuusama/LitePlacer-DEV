using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Reflection;
//using System.Web.Script.Serialization;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Diagnostics;
using AForge.Imaging;
using System.Windows.Media;
using MathNet.Numerics;
using HomographyEstimation;

using System.Text.RegularExpressions;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu;

namespace LitePlacer {
    public partial class FormMain : Form {

        /// <summary>
        /// Calibrate Optical / pixel ratio for UP CAMERA
        /// </summary>
        private void UpCamera_Calibration_button_Click(object sender, EventArgs e) {
            if (!SelectCamera(UpCamera)) return;

            //setup camera
            CamFunctionsClear_button_Click(null, null);
            SetNeedleMeasurement();
            NeedleToDisplay_button_Click(null, null);

            // temp turn off zoom
            var savedZoom = UpCamera.Zoom;
            UpCamera.Zoom = false;

            List<PartLocation> distance = new List<PartLocation>();
            List<PartLocation> pixels = new List<PartLocation>();

            // turn on slack compensation
            bool slackSetting = Cnc.SlackCompensation;
            Cnc.SlackCompensation = true;

            // move to upcamera position
            ZUp_button_Click(null, null); // move needle up
            GotoUpCamPosition_button_Click(null, null);
            ZDown_button_Click(null, null);

            var movement = new PartLocation(.1, .1);
            var startingPos = Cnc.XYLocation;
            startingPos.A = 0;

            var zoom = UpCamera.GetMeasurementZoom();
            UpCamera.MarkA.Clear();
            for (int i = -4; i < 5; i++) {
                //move
                var newLocation = startingPos + (i * movement);
                CNC_XYA_m(newLocation);

                //try 5 times to find a circle
                List<Shapes.Circle> circles = new List<Shapes.Circle>();
                for (int tries = 5; tries > 0 && circles.Count == 0; tries--)
                    circles = UpCamera.videoDetection.FindCircles();
                if (circles.Count == 0) continue; //not found, move and try again

                //find largest circle of the bunch
                var circle = circles.Aggregate((c, d) => c.Radius > d.Radius ? c : d); //find largest circle if we have multiple 
              //  var circlePL = (1 / zoom) * circle.ToPartLocation(); //compensate for zoom
                circle.ToScreenUnzoomedResolution();
                distance.Add(newLocation);
                pixels.Add(circle.ToPartLocation());

                UpCamera.MarkA.Add(circle.Clone().ToScreenResolution().ToPartLocation() .ToPointF());
                //DisplayText(String.Format("Actual Loc = {0}\t Measured Loc = {1}", newLocation, UpCamera.PixelsToActualLocation(circlePL)), Color.Blue);
            }


            if (pixels.Count < 2) {
                ShowMessageBox("Unable To Detect Circles",
                    "Try to adjust upcamera processing to see circles, and ensure upcamera needle position is correctly configured",
                    MessageBoxButtons.OK);
            } else {
                // Do regression on X and Y 
                var Xs = pixels.Select(xx => xx.X).ToArray();
                var Ys = distance.Select(xx => xx.X).ToArray();
                var result = MathNet.Numerics.LinearRegression.SimpleRegression.Fit(Xs, Ys);
                double XmmPerPixel = result.Item2;

                Xs = pixels.Select(xx => xx.Y).ToArray();
                Ys = distance.Select(xx => xx.Y).ToArray();
                result = MathNet.Numerics.LinearRegression.SimpleRegression.Fit(Xs, Ys);
                double YmmPerPixel = result.Item2;


                DisplayText(String.Format("{0} Xmm/pixel   {1} Ymm/pixel", XmmPerPixel, YmmPerPixel), Color.Purple);

                // update values
                Properties.Settings.Default.UpCam_XmmPerPixel = Math.Abs(XmmPerPixel);
                Properties.Settings.Default.UpCam_YmmPerPixel = Math.Abs(YmmPerPixel);
                UpCameraBoxXmmPerPixel_label.Text = "(" + Properties.Settings.Default.UpCam_XmmPerPixel.ToString("0.0000", CultureInfo.InvariantCulture) + "mm/pixel)";
                UpCameraBoxYmmPerPixel_label.Text = "(" + Properties.Settings.Default.UpCam_YmmPerPixel.ToString("0.0000", CultureInfo.InvariantCulture) + "mm/pixel)";
                UpCameraBoxX_textBox.Text = (Math.Abs(XmmPerPixel) * UpCamera.BoxSizeX).ToString("0.000", CultureInfo.InvariantCulture);
                UpCameraBoxY_textBox.Text = (Math.Abs(YmmPerPixel) * UpCamera.BoxSizeY).ToString("0.000", CultureInfo.InvariantCulture);

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
            UpCamera.Zoom = savedZoom;
            Cnc.SlackCompensation = slackSetting;
            ZUp_button_Click(null, null); //move up
            CamFunctionsClear_button_Click(null, null); //clear viewport
        }

          
        // DOWN CAMERA CALIBRATE
        private void button_camera_calibrate_Click(object sender, EventArgs e) {
            if (!SelectCamera(DownCamera)) return;

            //setup camera
            CamFunctionsClear_button_Click(null, null);
            SetHomingMeasurement();

            // temp turn off zoom
            var savedZoom = DownCamera.Zoom;
            DownCamera.Zoom = false;

            List<PartLocation> distance = new List<PartLocation>();
            List<PartLocation> pixels = new List<PartLocation>();

            // turn on slack compensation
            bool slackSetting = Cnc.SlackCompensation;
            Cnc.SlackCompensation = true;

            // move to upcamera position
            ZUp_button_Click(null, null); // move needle up

            double movedistance = .25;
            double.TryParse(calibMoveDistance_textBox.Text, out movedistance);

            var movement = new PartLocation(movedistance,movedistance);
            var startingPos = Cnc.XYLocation;
            DownCamera.MarkA.Clear();

            for (int i = 0; i < 5; i++) {
                //move
                var newLocation = startingPos + (i * movement);
                CNC_XY_m(newLocation);

                //try 5 times to find a circle
                VideoDetection vd = DownCamera.videoDetection;
                Shapes.Circle circle = null;
                for (int tries = 5; tries > 0 && circle == null; tries--)
                    circle = vd.GetClosest( vd.FindCircles() );
                if (circle == null) continue; //couldn't find one

                circle.ToScreenUnzoomedResolution(); //centered and unzoomed
                distance.Add(newLocation);
                pixels.Add(circle.ToPartLocation());

                // work with clone so we don't modify the entry in the previous list
                DownCamera.MarkA.Add(circle.Clone().ToScreenResolution().ToPartLocation().ToPointF());
                  
            }

            if (pixels.Count < 2) {
                ShowMessageBox("Unable To Detect Circles",
                    "Try to adjust upcamera processing to see circles, and ensure upcamera needle position is correctly configured",
                    MessageBoxButtons.OK);
            } else {
                // Do regression on X and Y 
                var Xs = pixels.Select(xx => xx.X).ToArray();
                var Ys = distance.Select(xx => xx.X).ToArray();
                var result = MathNet.Numerics.LinearRegression.SimpleRegression.Fit(Xs, Ys);
                double XmmPerPixel = result.Item2;

                Xs = pixels.Select(xx => xx.Y).ToArray();
                Ys = distance.Select(xx => xx.Y).ToArray();
                result = MathNet.Numerics.LinearRegression.SimpleRegression.Fit(Xs, Ys);
                double YmmPerPixel = result.Item2;


                DisplayText(String.Format("{0} Xmm/pixel   {1} Ymm/pixel", XmmPerPixel, YmmPerPixel), Color.Purple);

                // update values
                Properties.Settings.Default.UpCam_XmmPerPixel = Math.Abs(XmmPerPixel);
                Properties.Settings.Default.UpCam_YmmPerPixel = Math.Abs(YmmPerPixel);
                DownCameraBoxXmmPerPixel_label.Text = "(" + Properties.Settings.Default.UpCam_XmmPerPixel.ToString("0.0000", CultureInfo.InvariantCulture) + "mm/pixel)";
                DownCameraBoxYmmPerPixel_label.Text = "(" + Properties.Settings.Default.UpCam_YmmPerPixel.ToString("0.0000", CultureInfo.InvariantCulture) + "mm/pixel)";
                DownCameraBoxX_textBox.Text = (Math.Abs(XmmPerPixel) * DownCamera.BoxSizeX).ToString("0.000", CultureInfo.InvariantCulture);
                DownCameraBoxY_textBox.Text = (Math.Abs(YmmPerPixel) * DownCamera.BoxSizeY).ToString("0.000", CultureInfo.InvariantCulture);


            }

            //restore settings
            CNC_XY_m(startingPos);
            DownCamera.Zoom = savedZoom;
            Cnc.SlackCompensation = slackSetting;
            CamFunctionsClear_button_Click(null, null); //clear viewport

        }
    

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


        /// <summary>
        /// Will return the center of the average of several circle detections
        /// </summary>
        /// <returns></returns>
        private PartLocation MeasureCircle() {
            var circle = DownCamera.videoDetection.GetClosestAverageCircle(300, 5); //averages 5 circles
            circle.ToScreenResolution();
            return circle.ToPartLocation();
        }


    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using MathNet.Numerics.LinearRegression;

namespace LitePlacer {
    public class TapesClass {
        private DataGridView Grid;
        private NeedleClass Needle;
        private FormMain MainForm;
        private CNC Cnc;
        public SortableBindingList<TapeObj> tapeObjs = new SortableBindingList<TapeObj>();
        public List<string> TapeTypes;
        private const string TapesSaveName = "Tapes.xml";

        public TapesClass(DataGridView gr, NeedleClass ndl, CNC c, FormMain MainF) {
            Grid = gr;
            Needle = ndl;
            MainForm = MainF;
            Cnc = c;
            TapeTypes = AForgeFunctionSet.GetTapeTypes();
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\" + TapesSaveName))
                tapeObjs = Global.DeSerialization<SortableBindingList<TapeObj>>(AppDomain.CurrentDomain.BaseDirectory + @"\" + TapesSaveName);            
        }

        public void SaveAll() {
            Global.Serialization(tapeObjs, AppDomain.CurrentDomain.BaseDirectory + @"\" + TapesSaveName);
        }

        // ========================================================================================
        // ClearAll(): Resets Tape positions and pickup/place Z's.
        public void ClearAll() {
            for (int tape = 0; tape < Grid.Rows.Count; tape++) Reset(tape);
        }

        // ========================================================================================
        // Reset(): Resets one tape position and pickup/place Z's.
        public void Reset(int tape) {
            foreach (var t in tapeObjs) {
                t.Reset();
            }
        }

        public string[] GetListOfTapeIDs() {
            return tapeObjs.Select(x => x.ID).ToArray();
        }

        public TapeObj GetTapeObjByID(string id) {
            foreach (var x in tapeObjs) {
                if (x.ID.Equals(id)) return x;
            }
            return null;
        }

        public TapeObj GetTapeObjByIndex(int id) {
            if (tapeObjs.Count > id)
                return tapeObjs[id];
            return null;
        }

        public void AddTapeObject(int row) {
            tapeObjs.Insert(row, new TapeObj());
        }




        // ========================================================================================
        // Get/Set CurrentPickupZ/CurrentPlaceZ:
        // At the start of the job, we don't know the height of the component. Therefore, Main() probes 
        // the pickup and placement heights on the first part and sets them to the resulting values.
        // For speed, these results are used on the next parts.

        public bool ClearHeights_m(string Id) {
            var tape = GetTapeObjByID(Id);
            if (tape == null) return false;
            tape.PickupZ = -1;
            tape.PlaceZ = -1;
            return true;
        }

       

        // ========================================================================================
        // GotoNextPart_m(): Takes needle to exact location of the part, tape rotation taken in to account.
        // The position is measured using tape holes and knowledge about tape width and pitch (see EIA-481 standard).
        // Id tells the tape name. 
        public bool GotoNextPart_m(string Id) {
            MainForm.DisplayText("GotoNextPart_m(), tape id: " + Id);

            //Load & Parse Data
            TapeObj tapeObj = GetTapeObjByID(Id);
            if (tapeObj == null) return false;

            if (tapeObj.IsFullyCalibrated) {

                // if the tape is calibrated, we can skip the hole detection BS
                var OriginalLocationPrediction = tapeObj.GetCurrentPartLocation();
                PartLocation targetLocation = OriginalLocationPrediction;
                MainForm.DisplayText("Part " + tapeObj.CurrentPartIndex() + "  Source Location = " + OriginalLocationPrediction, Color.Blue);

                // see if it's a component and compute it's actual orientation
                if (tapeObj.TapeType == "Black") {
                    MainForm.DisplayText("USING ENHANCE PART PICKUP", Color.HotPink);
                    //assume it's a qfn - need to build up video recognition lib
                    if (!MainForm.Cnc.CNC_XY_m(OriginalLocationPrediction)) return false;
                    // setup view
                    MainForm.cameraView.SetDownCameraFunctionSet("2x2_QFN_BlackTape");
                    MainForm.cameraView.downVideoProcessing.FindRectangles = true;
                    // move closer
                    double X, Y;
                    MainForm.GoToLocation_m(Shapes.ShapeTypes.Rectangle, 1.5, .2, out  X, out  Y);
                    // new location 
                    targetLocation = Cnc.XYLocation;
                    targetLocation.OffsetBy(X, Y);


                    //pickup
                    var rects = VideoDetection.FindRectangles(MainForm.cameraView.downVideoProcessing);
                    var rect = VideoDetection.GetSmallestCenteredRectangle(rects);
                    if (rect == null) return false;
                    var rectAngle = rect.AngleOffsetFrom90();
                    // rect.ToScreenResolution();
                    // DownCamera.MarkA.Add(rect.ToPartLocation().ToPointF());
                    /*    rect.ToMMResolution();
                        loc = rect.ToPartLocation() + MainForm.Cnc.XYLocation;
                    */

                    targetLocation.A = tapeObj.OriginalPartOrientationVector.ToDegrees() + rectAngle;
                    MainForm.DisplayText("QFN @ " + targetLocation);

                    // MainForm.ShowSimpleMessageBox("Win:"+OriginalLocationPrediction.ToString());
                    MainForm.cameraView.SetDownCameraFunctionSet("");
                    // DownCamera.MarkA.Clear();
                    MainForm.cameraView.downVideoProcessing.FindRectangles = false;
                    MainForm.DisplayText("Moving to " + targetLocation, Color.Red);
                    MainForm.DisplayText("instead of " + OriginalLocationPrediction, Color.Red);
                }

                if (!Needle.Move_m(targetLocation)) return false;
            } else {
                //Setup Camera
                if (!SetCurrentTapeMeasurement_m(tapeObj.TapeType)) return false;

                // Go:
                var p = tapeObj.GetNearestCurrentPartHole();
                double X = p.X;
                double Y = p.Y;
                if (!MainForm.Cnc.CNC_XY_m(p.X, p.Y)) return false;

                // goto hole exact location:
                if (!MainForm.GoToLocation_m(Shapes.ShapeTypes.Circle, 1.8, 0.5, out X, out Y)) {
                    MainForm.ShowMessageBox(
                        "Can't find tape hole",
                        "Tape error",
                        MessageBoxButtons.OK
                    );
                    return false;
                }

                // X,Y are the offsets form the previous location (Cnc.Current{X,y}) 
                // the sum gives us the measured location of the circle
                X += Cnc.CurrentX;
                Y += Cnc.CurrentY;

                // ==================================================
                // find the part location and go there:
                // this is done as a local offset from the hole location detected closest to the part
                var partOffset = tapeObj.GetCurrentPartLocation() - tapeObj.GetNearestCurrentPartHole();
                X += partOffset.X;
                Y += partOffset.Y;
                double A = partOffset.A;

                // Take needle there:
                if (!Needle.Move_m(X, Y, A)) return false;
            }

            // Increment Part 
            tapeObj.NextPart();

            return true;
        }


        public bool SetCurrentTapeMeasurement_m(string type) {
            MainForm.cameraView.SetDownCameraFunctionSet(type+"Tape");
            Thread.Sleep(200);
            return true;
        }


        public bool CalibrateTape(TapeObj x) {
            // Setup Camera
            SetCurrentTapeMeasurement_m(x.TapeType);

            //1 - ensure first hole is correct
            MainForm.DisplayText("Moving to first hole @ " + x.FirstHole, Color.Purple);
            if (!MainForm.Cnc.CNC_XY_m(x.FirstHole)) return false;
            var holepos = MainForm.FindPositionOfClosest(Shapes.ShapeTypes.Circle, 1.8, 0.1); //find this hole with high precision
            if (holepos == null) return false;
            x.FirstHole = holepos;
            MainForm.DisplayText("Found new hole locaiton @ " + x.FirstHole, Color.Purple);

            // move to first hole for shits & giggles
            if (!MainForm.Cnc.CNC_XY_m(x.FirstHole)) return false;
            List<PartLocation> holes = new List<PartLocation>();
            List<int> holeIndex = new List<int>();
            holes.Add(x.FirstHole);
            holeIndex.Add(0);

            //2 - Look for for a few more holes 
            //    XXX-should be adjsuted to acocomodate smaller strips
            for (int i = 2; i < 8; i += 2) {
                if (!MainForm.Cnc.CNC_XY_m(x.GetHoleLocation(i))) break;
                Thread.Sleep(1000);
                var loc = MainForm.FindPositionOfClosest(Shapes.ShapeTypes.Circle, 1.8, 0.2);
                if (loc == null) break;
                holes.Add(loc);
                holeIndex.Add(i);
            }
            if (holes.Count < 2) return false; // didn't get enough points to calibrate this one


            //3 - Do Linear Regression on data if we have 2+ points
            // Fit circle to linear regression // y:x->a+b*x
            Double[] Xs = holes.Select(xx => xx.X - x.FirstHole.X).ToArray();
            Double[] Ys = holes.Select(xx => xx.Y - x.FirstHole.Y).ToArray();
            Tuple<double, double> result = SimpleRegression.Fit(Xs, Ys);
            x.a = result.Item1; //this should be as close to zero as possible if things worked correctly
            x.b = result.Item2; //this represents the slope of the tape
            MainForm.DisplayText(String.Format("Linear Regression: {0} + (0,{1})+(0,{2})x", x.FirstHole, x.a, x.b), Color.Brown);

            //4 - Determine Avg Hole Spacing
            double spacing = 0;
            for (int i = 0; i < holes.Count - 1; i++) {
                spacing += holes[i].DistanceTo(holes[i + 1]) / 2; //distance one hole to the next - /2 because we skip every other hole (step 2)
            }
            x.HolePitch = spacing / (holes.Count - 1); //compute average for holes

            //5 - Done, specify that this is fully calibrated
            x.IsFullyCalibrated = true;
            
            MainForm.DisplayText("Tape " + x.ID + " Calibrated", Color.Brown);
            //MainForm.DisplayText(String.Format("\tEquation = {3} + (0,{0}) + {1} * ({2} * holeNumber)", x.a, x.b, x.HolePitch), System.Drawing.Color.Brown);

            MainForm.cameraView.SetDownCameraFunctionSet("");
            if (!MainForm.Cnc.CNC_XY_m(x.FirstHole)) return false;

            return true;

        }


        public void CalibrateTapes() {
            foreach (TapeObj x in tapeObjs) {
                if (x.IsFullyCalibrated) continue; //skip if calibrated already
                CalibrateTape(x);
            }
        }

    
            


    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace LitePlacer
{
	class TapesClass
	{
		private DataGridView Grid;
		private NeedleClass Needle;
		private FormMain MainForm;
		private Camera DownCamera;
		private CNC Cnc;
        private List<TapeObj> tapeObjs = new List<TapeObj>();


		public TapesClass(DataGridView gr, NeedleClass ndl, Camera cam, CNC c, FormMain MainF)		{
			Grid = gr;
			Needle = ndl;
			DownCamera = cam;
			MainForm = MainF;
			Cnc = c;
		}

		// ========================================================================================
		// ClearAll(): Resets Tape positions and pickup/place Z's.
		public void ClearAll() 		{
			for (int tape = 0; tape < Grid.Rows.Count; tape++) Reset(tape);
		}

		// ========================================================================================
		// Reset(): Resets one tape position and pickup/place Z's.
		public void Reset(int tape)		{
            MainForm.DisplayText("Tape " + tape + " reset", System.Drawing.Color.Brown);
			Grid.Rows[tape].Cells["Next_Column"].Value = "1";
			Grid.Rows[tape].Cells["PickupZ_Column"].Value = "--";
			Grid.Rows[tape].Cells["PlaceZ_Column"].Value = "--";
            Grid.Rows[tape].Cells["Slope_Column"].Value = "";
            Grid.Rows[tape].Cells["HolePitch_Column"].Value = "";

            // reset obj
            tapeObjs[tape].SetPart(1);
            tapeObjs[tape].isPickupZSet = false;
            tapeObjs[tape].isPlaceZSet = false;    
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
            foreach (var x in tapeObjs) {
                if (x.row == id) return x;
            }
            return null;
        }

        /// <summary>
        /// Will reload all the table data into a set of objects
        /// </summary>
        public void ParseAll() {
            tapeObjs.Clear();
            for (int tape = 0; tape < Grid.Rows.Count; tape++)
                tapeObjs.Add(new TapeObj(GetTapeRow(tape), tape));
        }

        public void AddTapeObject(int row) {
            if (GetTapeObjByIndex(row) != null) throw new Exception("Adding row that exists");
            tapeObjs.Add(new TapeObj(GetTapeRow(row), row));
        }

        public void DeleteTapeObject(int row) {
            for (int i = 0; i < tapeObjs.Count; i++) {
                if (tapeObjs[i].row == row) {
                    tapeObjs.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Returns the row corresponding to the row #
        /// </summary>
        /// <param name="row">row #</param>
        /// <returns></returns>
        public DataGridViewCellCollection GetTapeRow(int row) {
            return Grid.Rows[row].Cells;
        }

        /// <summary>
        /// Returns the row corresponding to the string name or null if not found 
        /// </summary>
        /// <param name="id">string name</param>
        /// <returns></returns>
        public DataGridViewCellCollection GetTapeRow(string id) {
            int row;
            if (IdValidates_m(id, out row)) {
                return GetTapeRow(row);
            }
            return null;            
        }

		// ========================================================================================
		// IdValidates_m(): Checks that tape with description of "Id" exists.
		// Tape is set to the corresponding row of the Grid.
		public bool IdValidates_m(string Id, out int Tape) {
            var x = GetTapeObjByID(Id);
            if (x != null) {
                Tape = x.row;
                return true;
            }
            
            Tape = -1;
			MainForm.ShowMessageBox(
				"Did not find tape " + Id.ToString(),
				"Tape data error",
				MessageBoxButtons.OK
			);
			return false;
		}

		// ========================================================================================
		// Get/Set CurrentPickupZ/CurrentPlaceZ:
		// At the start of the job, we don't know the height of the component. Therefore, Main() probes 
		// the pickup and placement heights on the first part and sets them to the resulting values.
		// For speed, these results are used on the next parts.

        public bool ClearHeights_m(string Id) {
            int tape;
            if (!IdValidates_m(Id, out tape)) return false;
            Grid.Rows[tape].Cells["PickupZ_Column"].Value = "--";
            Grid.Rows[tape].Cells["PlaceZ_Column"].Value = "--";
            tapeObjs[tape].isPlaceZSet = false;
            tapeObjs[tape].isPickupZSet = false;
            return true;
        }

		public bool GetCurrentPickupZ_m(string Id, out string Z) {
            var x = GetTapeObjByID(Id);
            if (x != null) {
                Z = x.PickupZ;
                return true;
            }
            Z = "";
            return false;
		}

        public bool SetCurrentPickupZ_m(string Id, string Z) {
            var x = GetTapeObjByID(Id);
            if (x == null) return false;
            x.PickupZ = Z;
            Grid.Rows[x.row].Cells["PickupZ_Column"].Value = Z;
            return true;
        }

        public bool GetCurrentPlaceZ_m(string Id, out string Z) 	{
            var x = GetTapeObjByID(Id);
            if (x != null) {
                Z = x.PlaceZ;
                return true;
            }
            Z = "";
            return false;
		}

		public bool SetCurrentPlaceZ_m(string Id, string Z)		{
            var x = GetTapeObjByID(Id);
            if (x == null) return false;
            x.PlaceZ = Z;
			Grid.Rows[x.row].Cells["PlaceZ_Column"].Value = Z;
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

            if (tapeObj.isFullyCalibrated) {
            
                // if the tape is calibrated, we can skip the hole detection BS
                var OriginalLocationPrediction = tapeObj.GetCurrentPartLocation();
                PartLocation targetLocation = OriginalLocationPrediction;
                MainForm.DisplayText("Part "+tapeObj.CurrentPartIndex()+"  Source Location = " + OriginalLocationPrediction.ToString(),System.Drawing.Color.Blue);
                
                // see if it's a component and compute it's actual orientation
                if (tapeObj.Type == TapeType.Black) {
                    MainForm.DisplayText("USING ENHANCE PART PICKUP", System.Drawing.Color.HotPink);
                    //assume it's a qfn - need to build up video recognition lib
                    if (!MainForm.CNC_XY_m(OriginalLocationPrediction)) return false;
                    // setup view
                    MainForm.SetComponentView();
                    MainForm.DownCamera.FindRectangles = true;
                    // move closer
                    double X, Y;
                    MainForm.GoToLocation_m(Shapes.ShapeTypes.Rectangle, 1.5, .2, out  X, out  Y);
                    // new location 
                    targetLocation = Cnc.XYLocation;
                    targetLocation.OffsetBy(X, Y);


                    //pickup
                    var vd = MainForm.DownCamera.videoDetection;
                    var rects = vd.FindRectangles(DownCamera.GetMeasurementFrame());
                    var rect = vd.GetSmallestCenteredRectangle(rects);
                    if (rect == null) return false;
                    var rectAngle = rect.AngleOffsetFrom90();
                   // rect.ToScreenResolution();
                   // DownCamera.MarkA.Add(rect.ToPartLocation().ToPointF());
                /*    rect.ToMMResolution();
                    loc = rect.ToPartLocation() + MainForm.Cnc.XYLocation;
                */

                    targetLocation.A = tapeObj.PartOrientationVector.ToDegrees() + rectAngle;
                    MainForm.DisplayText("QFN @ " + targetLocation);

                   // MainForm.ShowSimpleMessageBox("Win:"+OriginalLocationPrediction.ToString());
                    MainForm.ClearDownVisualFilter();
                   // DownCamera.MarkA.Clear();
                    DownCamera.FindRectangles = false;
                    MainForm.DisplayText("Moving to " + targetLocation, System.Drawing.Color.Red);
                    MainForm.DisplayText("instead of " + OriginalLocationPrediction, System.Drawing.Color.Red);
                }

                if (!Needle.Move_m(targetLocation)) return false;
           } else {
                //Setup Camera
                if (!SetCurrentTapeMeasurement_m(tapeObj.Type)) return false;

                // Go:
                var p = tapeObj.GetNearestCurrentPartHole();
                double X = p.X;
                double Y = p.Y;
                if (!MainForm.CNC_XY_m(p.X, p.Y)) return false;

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
            // increment table to keep backwards compatibility
            Grid.Rows[tapeObj.row].Cells["Next_Column"].Value = tapeObj.CurrentPartIndex();

            return true;
        }	


        //-----------
        public bool SetCurrentTapeMeasurement_m(TapeType type) {
            switch (type) {
                case TapeType.Clear:
                    MainForm.SetClearTapeMeasurement();
                    break;
                case TapeType.Black:
                    MainForm.SetBlackTapeMeasurement();
                    break;
                case TapeType.White:
                    MainForm.SetPaperTapeMeasurement();
                    break;
                default:
                    return false;
            }
            Thread.Sleep(200);
            return true;
        }

  
        public bool CalibrateTape(TapeObj x) {
            // pull in any changes to the settings
            x.ReParse();
            // Setup Camera
            SetCurrentTapeMeasurement_m(x.Type);

            //1 - ensure first hole is correct
            MainForm.DisplayText("Moving to first hole @ " + x.FirstHole, System.Drawing.Color.Purple);
            if (!MainForm.CNC_XY_m(x.FirstHole)) return false;
            var holepos = MainForm.FindPositionOfClosest(Shapes.ShapeTypes.Circle, 1.8, 0.2); //find this hole with high precision
            if (holepos == null) return false;
            x.FirstHole = holepos;
            MainForm.DisplayText("Found new hole locaiton @ " + x.FirstHole, System.Drawing.Color.Purple);

            // move to first hole for shits & giggles
            if (!MainForm.CNC_XY_m(x.FirstHole)) return false;
            List<PartLocation> holes = new List<PartLocation>();
            List<int> holeIndex = new List<int>();
            holes.Add(x.FirstHole);
            holeIndex.Add(0);

            //2 - Look for for a few more holes 
            //    XXX-should be adjsuted to acocomodate smaller strips
            for (int i = 2; i < 8; i += 2) {
                if (!MainForm.CNC_XY_m(x.GetHoleLocation(i))) break;
                Thread.Sleep(1000);
                var loc = MainForm.FindPositionOfClosest(Shapes.ShapeTypes.Circle, 1.8, 0.5);
                if (loc == null) break;
                holes.Add(loc);
                holeIndex.Add(i);
            }
            if (holes.Count < 2) return false; // didn't get enough points to calibrate this one


            //3 - Do Linear Regression on data if we have 2+ points
            // Fit circle to linear regression // y:x->a+b*x
            Double[] Xs = holes.Select(xx => xx.X - x.FirstHole.X).ToArray();
            Double[] Ys = holes.Select(xx => xx.Y - x.FirstHole.Y).ToArray();
            Tuple<double, double> result = MathNet.Numerics.LinearRegression.SimpleRegression.Fit(Xs, Ys);
            x.a = result.Item1; //this should be as close to zero as possible if things worked correctly
            x.b = result.Item2; //this represents the slope of the tape
            MainForm.DisplayText(String.Format("Linear Regression: {0} + (0,{1})+(0,{2})x",x.FirstHole,x.a,x.b), System.Drawing.Color.Brown);

            //4 - Determine Avg Hole Spacing
            double spacing = 0;
            for (int i = 0; i < holes.Count-1; i++) {
                spacing += holes[i].DistanceTo(holes[i+1]) / 2; //distance one hole to the next - /2 because we skip every other hole (step 2)
            }
            x.HolePitch = spacing/(holes.Count - 1); //compute average for holes

            //5 - Done, specify that this is fully calibrated
            x.isFullyCalibrated = true;

            //6 - Update Row
            x.PushChangesToRow();

            MainForm.DisplayText("Tape " + x.ID + " Calibrated", System.Drawing.Color.Brown);
            //MainForm.DisplayText(String.Format("\tEquation = {3} + (0,{0}) + {1} * ({2} * holeNumber)", x.a, x.b, x.HolePitch), System.Drawing.Color.Brown);

            DownCamera.videoProcessing.ClearFunctionsList();
            if (!MainForm.CNC_XY_m(x.FirstHole)) return false;

            return true;
 
        }


        public void CalibrateTapes() {
            foreach (TapeObj x in tapeObjs) {
                if (x.isFullyCalibrated) continue; //skip if calibrated already
                CalibrateTape(x);
            }
        }



	}

}

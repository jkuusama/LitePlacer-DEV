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

		public TapesClass(DataGridView gr, NeedleClass ndl, Camera cam, CNC c, FormMain MainF)
		{
			Grid = gr;
			Needle = ndl;
			DownCamera = cam;
			MainForm = MainF;
			Cnc = c;
		}

		// ========================================================================================
		// ClearAll(): Resets Tape positions and pickup/place Z's.
		public void ClearAll()
		{
			for (int tape = 0; tape < Grid.Rows.Count; tape++)
			{
				Grid.Rows[tape].Cells["Next_Column"].Value = "1";
				Grid.Rows[tape].Cells["PickupZ_Column"].Value = "--";
				Grid.Rows[tape].Cells["PlaceZ_Column"].Value = "--";
				Grid.Rows[tape].Cells["NextX_Column"].Value = Grid.Rows[tape].Cells["X_Column"].Value;
				Grid.Rows[tape].Cells["NextY_Column"].Value = Grid.Rows[tape].Cells["Y_Column"].Value;
			}
		}

		// ========================================================================================
		// Reset(): Resets one tape position and pickup/place Z's.
		public void Reset(int tape)
		{
			Grid.Rows[tape].Cells["Next_Column"].Value = "1";
			Grid.Rows[tape].Cells["PickupZ_Column"].Value = "--";
			Grid.Rows[tape].Cells["PlaceZ_Column"].Value = "--";
			Grid.Rows[tape].Cells["NextX_Column"].Value = Grid.Rows[tape].Cells["X_Column"].Value;
			Grid.Rows[tape].Cells["NextY_Column"].Value = Grid.Rows[tape].Cells["Y_Column"].Value;
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
		public bool IdValidates_m(string Id, out int Tape)
		{
			Tape = -1;
			foreach (DataGridViewRow Row in Grid.Rows)
			{
				Tape++;
				if (Row.Cells["IdColumn"].Value.ToString() == Id)
				{
					return true;
				}
			}
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

        public bool ClearHeights_m(string Id)
        {
            int tape;
            if (!IdValidates_m(Id, out tape))
            {
                return false;
            }
            Grid.Rows[tape].Cells["PickupZ_Column"].Value = "--";
            Grid.Rows[tape].Cells["PlaceZ_Column"].Value = "--";
            return true;
        }

		public bool GetCurrentPickupZ_m(string Id, out string Z)
		{
			int tape;
			Z = "";
			if (!IdValidates_m(Id, out tape))
			{
				return false;
			}
			Z = Grid.Rows[tape].Cells["PickupZ_Column"].Value.ToString();
			return true;
		}

        public bool SetCurrentPickupZ_m(string Id, string Z)
        {
            int tape;
            if (!IdValidates_m(Id, out tape))
            {
                return false;
            }
            Grid.Rows[tape].Cells["PickupZ_Column"].Value = Z;
            return true;
        }

        public bool GetCurrentPlaceZ_m(string Id, out string Z)
		{
			int tape;
			Z = "";
			if (!IdValidates_m(Id, out tape))
			{
				return false;
			}
			Z = Grid.Rows[tape].Cells["PlaceZ_Column"].Value.ToString();
			return true;
		}

		public bool SetCurrentPlaceZ_m(string Id, string Z)
		{
			int tape;
			if (!IdValidates_m(Id, out tape))
			{
				return false;
			}
			Grid.Rows[tape].Cells["PlaceZ_Column"].Value = Z;
			return true;
		}


        // ========================================================================================
        // GotoNextPart_m(): Takes needle to exact location of the part, tape rotation taken in to account.
        // The position is measured using tape holes and knowledge about tape width and pitch (see EIA-481 standard).
        // Id tells the tape name. 
        public bool GotoNextPart_m(string Id) {
            int Tape = 0;
            MainForm.DisplayText("GotoNextPart_m(), tape id: " + Id);
            if (!IdValidates_m(Id, out Tape)) return false;
            
            //Load & Parse Data
            TapeObj tapeObj = new TapeObj(GetTapeRow(Tape));

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
            
            // Increment Part 
            tapeObj.NextPart();
            // increment table to keep backwards compatibility
            Grid.Rows[Tape].Cells["Next_Column"].Value = tapeObj.CurrentPartIndex();

            return true;
        }	

		// ========================================================================================
		// GotoNextPart_m(): Takes needle to exact location of the part, tape rotation taken in to account.
		// The position is measured using tape holes and knowledge about tape width and pitch (see EIA-481 standard).
		// Id tells the tape name. 
		public bool OLD_GotoNextPart_m(string Id)
		{
			int Tape= 0;
            MainForm.DisplayText("GotoNextPart_m(), tape id: " + Id);
			if(!IdValidates_m(Id, out Tape))
			{
				return false;
			}
			// goto next hole approximate location:
			if (!OLD_SetCurrentTapeMeasurement_m(Tape))  // having the measurement setup here helps with the automatic gain lag
				return false;

			double X= 0;
			double Y= 0;
			if (!double.TryParse(Grid.Rows[Tape].Cells["NextX_Column"].Value.ToString(), out X))
			{
				MainForm.ShowMessageBox(
					"Bad data at Tape " + Tape.ToString() + ", Next X",
					"Tape data error",
					MessageBoxButtons.OK
				);
				return false;
			}

			if (!double.TryParse(Grid.Rows[Tape].Cells["NextY_Column"].Value.ToString(), out Y))
			{
				MainForm.ShowMessageBox(
					"Bad data at Tape " + Tape.ToString() + ", Next Y",
					"Tape data error",
					MessageBoxButtons.OK
				);
				return false;
			}
			// Go:
			if (!MainForm.CNC_XY_m(X, Y))
			{
				return false;
			};

			// goto hole exact location:
			// We want to find the hole less than 2mm from where we think it should be. (Otherwise there is a risk
			// of picking a wrong hole.)
			if (!MainForm.GoToLocation_m(Shapes.ShapeTypes.Circle,1.8, 0.5, out X, out Y))
			{
				MainForm.ShowMessageBox(
					"Can't find tape hole",
					"Tape error",
					MessageBoxButtons.OK
				);
				return false;
			}

			// Get the hole location, we'll need it later:
			X = Cnc.CurrentX + X;
			Y = Cnc.CurrentY + Y;

			//MainForm.ShowMessageBox(
			//    "exactly over the hole",
			//    "test",
			//    MessageBoxButtons.OK
			//);

			// ==================================================
			// find the part location and go there:

			string Width = Grid.Rows[Tape].Cells["WidthColumn"].Value.ToString();
			// Tape measurements: 
			double dL = 2.0;	// Part center pos from hole, tape lenght direction; 2.0mm in all tape types,
                                // except when 2mm part pitch. See below.
			double dW;			// Part center pos from hole, tape width direction. Varies.
			int Pitch;  // Distance from one part to another
			switch (Width)
			{
				case "8/2mm":
					dW = 3.50;
					Pitch = 2;
					break;
				case "8/4mm":
					dW = 3.50;
					Pitch = 4;
					break;

				case "12/4mm":
					dW = 5.50;
					Pitch = 4;
					break;
				case "12/8mm":
					dW = 5.50;
					Pitch = 8;
					break;

				case "16/4mm":
					dW = 7.50;
					Pitch = 4;
					break;
				case "16/8mm":
					dW = 7.50;
					Pitch = 8;
					break;
				case "16/12mm":
					dW = 7.50;
					Pitch = 12;
					break;

				case "24/4mm":
					dW = 11.50;
					Pitch = 4;
					break;
				case "24/8mm":
					dW = 11.50;
					Pitch = 8;
					break;
				case "24/12mm":
					dW = 11.50;
					Pitch = 12;
					break;
				case "24/16mm":
					dW = 11.50;
					Pitch = 16;
					break;
				case "24/20mm":
					dW = 11.50;
					Pitch = 20;
					break;

				default:
					MainForm.ShowMessageBox(
						"Bad data at Tape #" + Tape.ToString() + ", Width",
						"Tape data error",
						MessageBoxButtons.OK
					);
					return false;
				// break;
			}
			// Tape orientation: 
			// +Y: Holeside of tape is right, part is dW(mm) to left, dL(mm) down from hole, A= 0
			// +X: Holeside of tape is down, part is dW(mm) up, dL(mm) to left from hole, A= -90
			// -Y: Holeside of tape is left, part is dW(mm) to right, dL(mm) up from hole, A= -180
			// -X: Holeside of tape is up, part is dW(mm) down, dL(mm) to right from hole, A=-270
			double A= 0.0;
			int pos = 0;
			if (!int.TryParse(Grid.Rows[Tape].Cells["Next_Column"].Value.ToString(), out pos))
			{
				MainForm.ShowMessageBox(
					"Bad data at Tape " + Tape.ToString() + ", Next",
					"Tape data error",
					MessageBoxButtons.OK
				);
				return false;
			}
            if ((Pitch == 2) && ((pos % 2) == 0))
			{
				dL = 0.0;
			};

			double partX= 0;
			double partY= 0;
			switch (Grid.Rows[Tape].Cells["OrientationColumn"].Value.ToString())
			{
				case "+Y":
					partX = X - dW;
					partY = Y - dL;
					A = 0.0;
					break;

				case "+X":
					partX = X - dL;
					partY = Y + dW;
					A = -90.0;
					break;

				case "-Y":
					partX = X + dW;
					partY = Y + dL;
					A = -180.0;
					break;

				case "-X":
					partX = X + dL;
					partY = Y - dW;
					A = -270.0;
					break;

				default:
					MainForm.ShowMessageBox(
						"Bad data at Tape #" + Tape.ToString() + ", Orientation",
						"Tape data error",
						MessageBoxButtons.OK
					);
					return false;
			}
			// rotation:
			if (Grid.Rows[Tape].Cells["RotationColumn"].Value == null)
			{
				MainForm.ShowMessageBox(
					"Bad data at tape " + Id +" rotation" ,
					"Assertion error",
					MessageBoxButtons.OK
				);
				return false;
			}
			switch (Grid.Rows[Tape].Cells["RotationColumn"].Value.ToString())
			{
				case "0deg.":
					break;

				case "90deg.":
					A += 90.0;
					break;

				case "180deg.":
					A += 180.0;
					break;

				case "270deg.":
					A += 270.0;
					break;

				default:
					MainForm.ShowMessageBox(
						"Bad data at Tape " + Id + " rotation",
						"Tape data error",
						MessageBoxButtons.OK
					);
					return false;
					// break;
			};
			while (A > 360.1)
			{
				A -= 360.0;
			}
			while (A < 0.0)
			{
				A += 360.0;
			};

			// Now, partX, partY, A tell the position of the part. Take needle there:
			if (!Needle.Move_m(partX, partY, A))
			{
				return false;
			}

			// ==================================================
			// X, Y still are the current hole location, Pitch is the part size increment and pos the current part count
			// Set next hole approximate location. On 2mm part pitch, increment only at even part count.
            if (Pitch == 2) 
            {
                if((pos % 2) != 0)
                {
                    Pitch = 0;
                }
                else
                {
                    Pitch = 4;
                }
            };
			switch (Grid.Rows[Tape].Cells["OrientationColumn"].Value.ToString())
			{
				case "+Y":
					Y = Y + Pitch;
					break;

				case "+X":
					X = X + Pitch;
					break;

				case "-Y": Y = Y - Pitch;
					break;

				case "-X":
					X = X - Pitch;
					break;
			};
            Grid.Rows[Tape].Cells["NextX_Column"].Value = X.ToString();
			Grid.Rows[Tape].Cells["NextY_Column"].Value = Y.ToString();
            // increment next count
            pos++;
			Grid.Rows[Tape].Cells["Next_Column"].Value = pos.ToString();
			return true;
		}	// end GotoNextPart_m


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
		

		// ========================================================================================
		// SetCurrentTapeMeasurement_m(): sets the camera measurement parameters according to the tape type.
		public bool OLD_SetCurrentTapeMeasurement_m(int row)
		{
			switch (Grid.Rows[row].Cells["TypeColumn"].Value.ToString())
			{
				case "Paper (White)":
					MainForm.SetPaperTapeMeasurement();
					Thread.Sleep(200);   // for automatic camera gain to have an effect
					return true;

				case "Black Plastic":
					MainForm.SetBlackTapeMeasurement();
					Thread.Sleep(200);   // for automatic camera gain to have an effect
					return true;

				case "Clear Plastic":
					MainForm.SetClearTapeMeasurement();
					Thread.Sleep(200);   // for automatic camera gain to have an effect
					return true;

				default:
					MainForm.ShowMessageBox(
						"Bad Type data on row " + row.ToString() + ": " + Grid.Rows[row].Cells["TypeColumn"].Value.ToString(),
						"Bad Type data",
						MessageBoxButtons.OK
					);
					return false;
			}
		}
	}

}

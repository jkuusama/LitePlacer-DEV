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
        private DataGridView CustomGrid;
        private NeedleClass Needle;
		private FormMain MainForm;
		private Camera DownCamera;
		private CNC Cnc;

        public TapesClass(DataGridView gr, DataGridView custom, NeedleClass ndl, Camera cam, CNC c, FormMain MainF)
		{
            CustomGrid = custom;
            Grid = gr;
			Needle = ndl;
			DownCamera = cam;
			MainForm = MainF;
			Cnc = c;
		}

		// ========================================================================================
		// ClearAll(): Resets TapeNumber positions and pickup/place Z's.
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
            // fix #22 reset next coordinates
            Grid.Rows[tape].Cells["NextX_Column"].Value = Grid.Rows[tape].Cells["X_Column"].Value;
			Grid.Rows[tape].Cells["NextY_Column"].Value = Grid.Rows[tape].Cells["Y_Column"].Value;
		}

		// ========================================================================================
		// IdValidates_m(): Checks that tape with description of "Id" exists.
		// TapeNumber is set to the corresponding row of the Grid.
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
        // Fast placement:
        // ========================================================================================
        // The process measures last and first hole positions for a row in job data. It keeps track about
        // the hole location in next columns, and in this process, these are measured, not approximated.
        // The part numbers are found with the GetPartLocationFromHolePosition_m() routine.

        public bool FastParametersOk { get; set; }  // if we should use fast placement in the first place
        public double FastXstep { get; set; }       // steps for hole positions
        public double FastYstep { get; set; }
        public double FastXpos { get; set; }       // we don't want to mess with tape definitions
        public double FastYpos { get; set; }

        // ========================================================================================
        // PrepareForFastPlacement_m: Called before starting fast placement

        public bool PrepareForFastPlacement_m(string TapeID, int ComponentCount)
        {
            int TapeNum;
            if (!IdValidates_m(TapeID, out TapeNum))
            {
                FastParametersOk = false;
                return false;
            }
            int first;
            if (!int.TryParse(Grid.Rows[TapeNum].Cells["Next_Column"].Value.ToString(), out first))
            {
                MainForm.ShowMessageBox(
                    "Bad data at next column",
                    "Sloppy programmer error",
                    MessageBoxButtons.OK);
                FastParametersOk = false;
                return false;
            }
            int last = first + ComponentCount-1;
            // measure holes
            double LastX = 0.0;
            double LastY = 0.0;
            double FirstX = 0.0;
            double FirstY = 0.0;
            if (!GetPartHole_m(TapeNum, last, out LastX, out LastY))
            {
                FastParametersOk = false;
                return false;
            }
            if (last!= first)
            {
                if (!GetPartHole_m(TapeNum, first, out FirstX, out FirstY))
                {
                    FastParametersOk = false;
                    return false;
                }
            }
            else
            {
                FirstX = LastX;
                FirstY = LastY;
            }

            FastXpos = FirstX;
            FastYpos = FirstY;
            if (ComponentCount>1)
            {
                FastXstep = (LastX - FirstX) / (double)(ComponentCount-1);
                FastYstep = (LastY - FirstY) / (double)(ComponentCount-1);
            }
            else
            {
                FastXstep = 0.0;
                FastYstep = 0.0;
            }

            MainForm.DisplayText("Fast parameters:");
            MainForm.DisplayText("First X: " + FirstX.ToString() + ", Y: " + FirstY.ToString());
            MainForm.DisplayText("Last X: " + LastX.ToString() + ", Y: " + LastY.ToString());
            MainForm.DisplayText("Step X: " + FastXstep.ToString() + ", Y: " + FastYstep.ToString());

            return true;
        }

        // ========================================================================================
        // IncrementTape_Fast(): Updates count and next hole locations for a tape
        // Like IncrementTape(), but just using the fast parameters
        public bool IncrementTape_Fast(int TapeNum)
        {
            int pos;
            if (!int.TryParse(Grid.Rows[TapeNum].Cells["Next_Column"].Value.ToString(), out pos))
            {
                MainForm.ShowMessageBox(
                    "Bad data at next column",
                    "Sloppy programmer error",
                    MessageBoxButtons.OK);
                return false;
            }
            if (Grid.Rows[TapeNum].Cells["WidthColumn"].Value.ToString() == "8/2mm" )
	        {
		        if ((pos%2)==0)      // increment hole location only on every other "next" value on 2mm parts
	            {
                    FastXpos += FastXstep*2;
                    FastYpos += FastYstep*2;
                    Grid.Rows[TapeNum].Cells["NextX_Column"].Value = FastXpos.ToString("0.000", CultureInfo.InvariantCulture);
                    Grid.Rows[TapeNum].Cells["NextY_Column"].Value = FastYpos.ToString("0.000", CultureInfo.InvariantCulture);
                }
	        }
            else
            {
                FastXpos += FastXstep;
                FastYpos += FastYstep;
                Grid.Rows[TapeNum].Cells["NextX_Column"].Value = FastXpos.ToString("0.000", CultureInfo.InvariantCulture);
                Grid.Rows[TapeNum].Cells["NextY_Column"].Value = FastYpos.ToString("0.000", CultureInfo.InvariantCulture);
            }
            
            pos += 1;
            Grid.Rows[TapeNum].Cells["Next_Column"].Value = pos.ToString();
           return true;
        }

        // ========================================================================================
        // GetTapeParameters_m(): 
        // Get from the indicated tape the dW, part center pos from hole and Pitch, distance from one part to another
        private bool GetTapeParameters_m(int Tape, out int CustomTapeNum, out double dW, out double FromHole, out double Pitch)
        {
            CustomTapeNum= -1;      // not custom
            dW = 0.0;
            Pitch = 0.0;
            FromHole = 2.0;
            string Width = Grid.Rows[Tape].Cells["WidthColumn"].Value.ToString();
            // TapeNumber measurements: 
            switch (Width)
            {
                case "8/2mm":
                    dW = 3.50;
                    Pitch = 2.0;
                    break;
                case "8/4mm":
                    dW = 3.50;
                    Pitch = 4.0;
                    break;

                case "12/4mm":
                    dW = 5.50;
                    Pitch = 4.0;
                    break;
                case "12/8mm":
                    dW = 5.50;
                    Pitch = 8.0;
                    break;

                case "16/4mm":
                    dW = 7.50;
                    Pitch = 4.0;
                    break;
                case "16/8mm":
                    dW = 7.50;
                    Pitch = 8.0;
                    break;
                case "16/12mm":
                    dW = 7.50;
                    Pitch = 12.0;
                    break;

                case "24/4mm":
                    dW = 11.50;
                    Pitch = 4.0;
                    break;
                case "24/8mm":
                    dW = 11.50;
                    Pitch = 8.0;
                    break;
                case "24/12mm":
                    dW = 11.50;
                    Pitch = 12.0;
                    break;
                case "24/16mm":
                    dW = 11.50;
                    Pitch = 16.0;
                    break;
                case "24/20mm":
                    dW = 11.50;
                    Pitch = 20.0;
                    break;

                default:
                    if (!FindCustomTapeParameters(Width, out CustomTapeNum, out dW, out FromHole, out Pitch))
                    {
                        MainForm.ShowMessageBox(
                            "Bad data at Tape #" + Tape.ToString() + ", Width",
                            "Tape data error",
                            MessageBoxButtons.OK
                        );
                        return false;
                    }
                    break;
            }
            return true;
        }
		// ========================================================================================

        // ========================================================================================
        // GetPartHole_m(): Measures X,Y location of the hole corresponding to part number
        public bool GetPartHole_m(int TapeNum, int PartNum, out double ResultX, out double ResultY)
        {
            ResultX = 0.0;
            ResultY = 0.0;

            // Get start points
            double X = 0.0;
            double Y = 0.0;
            if (!double.TryParse(Grid.Rows[TapeNum].Cells["X_Column"].Value.ToString(), out X))
            {
                MainForm.ShowMessageBox(
                    "Bad data at Tape " + TapeNum.ToString() + ", X",
                    "Tape data error",
                    MessageBoxButtons.OK
                );
                return false;
            }
            if (!double.TryParse(Grid.Rows[TapeNum].Cells["Y_Column"].Value.ToString(), out Y))
            {
                MainForm.ShowMessageBox(
                    "Bad data at Tape " + TapeNum.ToString() + ", Y",
                    "Tape data error",
                    MessageBoxButtons.OK
                );
                return false;
            }

            // Get the hole location guess
            double dW;
            double Pitch;
            double FromHole;
            int CustomTapeNum = -1;
            if (!GetTapeParameters_m(TapeNum, out CustomTapeNum, out dW, out FromHole, out Pitch))
            {
                return false;
            }
            if (Math.Abs(Pitch-2.0)<0.01) // if pitch ==2
            {
                PartNum = (PartNum +1)/ 2;
                Pitch = 4.0;
            }
            double dist = (double)(PartNum-1) * Pitch; // This many mm's from start
            switch (Grid.Rows[TapeNum].Cells["OrientationColumn"].Value.ToString())
            {
                case "+Y":
                    Y = Y + dist;
                    break;

                case "+X":
                    X = X + dist;
                    break;

                case "-Y":
                    Y = Y - dist;
                    break;

                case "-X":
                    X = X - dist;
                    break;

                default:
                    MainForm.ShowMessageBox(
                        "Bad data at Tape #" + TapeNum.ToString() + ", Orientation",
                        "Tape data error",
                        MessageBoxButtons.OK
                    );
                    return false;
            }
            // X, Y now hold the first guess

            // Measuring 
            if (!SetCurrentTapeMeasurement_m(TapeNum))  // having the measurement setup here helps with the automatic gain lag
            {
                return false;
            }
            // Go there:
            if (!MainForm.CNC_XY_m(X, Y))
            {
                return false;
            };

            // get hole exact location:
            if (!MainForm.GoToCircleLocation_m(1.8, 0.1, out X, out Y))
            {
                MainForm.ShowMessageBox(
                    "Can't find tape hole",
                    "Tape error",
                    MessageBoxButtons.OK
                );
                return false;
            }
            ResultX = Cnc.CurrentX + X;
            ResultY = Cnc.CurrentY + Y;
            return true;
        }

		// ========================================================================================
		// GetPartLocationFromHolePosition_m(): Returns the location and rotation of the part
        // Input is the exact (measured) location of the hole

        public bool GetPartLocationFromHolePosition_m(int Tape, double X, double Y, out double PartX, out double PartY, out double A)
        {
            PartX = 0.0;
            PartY = 0.0;
            A = 0.0;

			double dW;	// Part center pos from hole, tape width direction. Varies.
            double dL;   // Part center pos from hole, tape lenght direction. -2mm on all standard tapes
			double Pitch;  // Distance from one part to another

            int CustomTapeNum = -1;
            if (!GetTapeParameters_m(Tape, out CustomTapeNum, out dW, out dL, out Pitch))
	        {
		        return false;
	        }
            // dL = -dL; // so up is + etc.
			// TapeNumber orientation: 
			// +Y: Holeside of tape is right, part is dW(mm) to left, dL(mm) down from hole, A= 0
			// +X: Holeside of tape is down, part is dW(mm) up, dL(mm) to left from hole, A= -90
			// -Y: Holeside of tape is left, part is dW(mm) to right, dL(mm) up from hole, A= -180
			// -X: Holeside of tape is up, part is dW(mm) down, dL(mm) to right from hole, A=-270
            int pos;
			if (!int.TryParse(Grid.Rows[Tape].Cells["Next_Column"].Value.ToString(), out pos))
			{
				MainForm.ShowMessageBox(
					"Bad data at Tape " + Tape.ToString() + ", Next",
					"Tape data error",
					MessageBoxButtons.OK
				);
				return false;
			}     
            // if pitch == 2 and part# is odd, DL=2, other
            if (Math.Abs(Pitch - 2) < 0.01)
            {
                if ((pos % 2) == 1)
                {
                    dL = 2.0;
                }
                else
                {
                    dL = 0.0;
                }
            }

			switch (Grid.Rows[Tape].Cells["OrientationColumn"].Value.ToString())
			{
				case "+Y":
					PartX = X - dW;
					PartY = Y - dL;
					A = 0.0;
					break;

				case "+X":
					PartX = X - dL;
					PartY = Y + dW;
					A = -90.0;
					break;

				case "-Y":
					PartX = X + dW;
					PartY = Y + dL;
					A = -180.0;
					break;

				case "-X":
					PartX = X - dL;
					PartY = Y - dW;
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
					"Bad data at tape " + Grid.Rows[Tape].Cells["IdColumn"].Value.ToString() +" rotation",
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
						"Bad data at Tape " + Grid.Rows[Tape].Cells["IdColumn"].Value.ToString() + " rotation",
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
            return true;
        }

        // ========================================================================================
        // IncrementTape(): Updates count and next hole locations for a tape
        // The caller knows the current hole location, so we don't need to re-measure them
        public bool IncrementTape(int Tape, double HoleX, double HoleY)
        {
            double dW;	// Part center pos from hole, tape width direction. Varies.
            double dL;   // Part center pos from hole, tape lenght direction. -2mm on all standard tapes
            double Pitch;  // Distance from one part to another
            int CustomTapeNum = -1;
            if (!GetTapeParameters_m(Tape, out CustomTapeNum, out dW, out dL, out Pitch))
            {
                return false;
            }

            int pos;
            if (!int.TryParse(Grid.Rows[Tape].Cells["Next_Column"].Value.ToString(), out pos))
			{
				MainForm.ShowMessageBox(
                    "Bad data at Tape " + Grid.Rows[Tape].Cells["IdColumn"].Value.ToString() + ", next",
					"S´loppy programmer error",
					MessageBoxButtons.OK
				);
				return false;
			}

            // Set next hole approximate location. On 2mm part pitch, increment only at even part count.
            if (Math.Abs(Pitch - 2) < 0.000001)
            {
                if ((pos % 2) != 0)
                {
                    Pitch = 0.0;
                }
                else
                {
                    Pitch = 4.0;
                }
            };
            switch (Grid.Rows[Tape].Cells["OrientationColumn"].Value.ToString())
            {
                case "+Y":
                    HoleY = HoleY + (double)Pitch;
                    break;

                case "+X":
                    HoleX = HoleX + (double)Pitch;
                    break;

                case "-Y":
                    HoleY = HoleY - (double)Pitch;
                    break;

                case "-X":
                    HoleX = HoleX - (double)Pitch;
                    break;
            };
            Grid.Rows[Tape].Cells["NextX_Column"].Value = HoleX.ToString();
            Grid.Rows[Tape].Cells["NextY_Column"].Value = HoleY.ToString();
            // increment next count
            pos++;
            Grid.Rows[Tape].Cells["Next_Column"].Value = pos.ToString();
            return true;
        }

        // ========================================================================================
        // UpdateNextCoordinates(): Updates next coordinates for a given tape based on new next coordinate number
        public bool UpdateNextCoordinates(int Tape, int NextNo)
        {
            double dW;	// Part center pos from hole, tape width direction. Varies.
            double dL;   // Part center pos from hole, tape lenght direction. -2mm on all standard tapes
            double Pitch;  // Distance from one part to another
            int CustomTapeNum = -1;
            if (!GetTapeParameters_m(Tape, out CustomTapeNum, out dW, out dL, out Pitch))
            {
                return false;
            }

            int pos = NextNo - 1;

            double offset = pos * Pitch;

            // correct offset for 2mm part pitch
            if (Math.Abs(Pitch - 2) < 0.000001)
            {
                if ((pos % 2) != 0)
                {
                    offset -= Pitch;
                }
            }

            // determin first hole coordinates
            double Hole1X;
            double Hole1Y;

            NumberStyles style = NumberStyles.AllowDecimalPoint;
            CultureInfo culture = CultureInfo.InvariantCulture;
            if (Grid.Rows[Tape].Cells["X_Column"].Value == null)
            {
                return false;
            }
            string s = Grid.Rows[Tape].Cells["X_Column"].Value.ToString();           
            if (!double.TryParse(s, style, culture, out Hole1X))
            {
                return false;
            }
            if (Grid.Rows[Tape].Cells["Y_Column"].Value == null)
            {
                return false;
            }
            s = Grid.Rows[Tape].Cells["Y_Column"].Value.ToString();
            if (!double.TryParse(s, style, culture, out Hole1Y))
            {
                return false;
            }

            double NextX = Hole1X;
            double NextY = Hole1Y;

            // calculate next coordinates based on tape orientation, 1st hole position and offset from above
            switch (Grid.Rows[Tape].Cells["OrientationColumn"].Value.ToString())
            {
                case "+Y":
                    NextY = Hole1Y + offset;
                    break;

                case "+X":
                    NextX = Hole1X + offset;
                    break;

                case "-Y":
                    NextY = Hole1Y - offset;
                    break;

                case "-X":
                    NextX = Hole1X - offset;
                    break;
            };

            Grid.Rows[Tape].Cells["NextX_Column"].Value = NextX.ToString("0.000", CultureInfo.InvariantCulture);
            Grid.Rows[Tape].Cells["NextY_Column"].Value = NextY.ToString("0.000", CultureInfo.InvariantCulture);

            return true;
        }

        // ========================================================================================
        // GotoNextPartByMeasurement_m(): Takes needle to exact location of the part, tape and part rotation taken in to account.
        // The hole position is measured on each call using tape holes and knowledge about tape width and pitch (see EIA-481 standard).
        // Id tells the tape name. 
        // The caller needs the hole coordinates and tape number later in the process, but they are measured and returned here.
        public bool GotoNextPartByMeasurement_m(int TapeNumber, out double HoleX, out double HoleY)
		{
            HoleX = 0;
            HoleY = 0;
            int CustomTapeNumber;
            // If this is a custom tape that doesn't use location marks, we'll return the set position:
            if (IsCustomTape(TapeNumber, out CustomTapeNumber))
            {
                if (!(CustomGrid.Rows[CustomTapeNumber].Cells["UsesLocationMarks_Column"].Value.ToString() == "true"))
                {
                    if (!double.TryParse(CustomGrid.Rows[CustomTapeNumber].Cells["PartOffsetX_Column"].Value.ToString(), out HoleX))
                    {
                        MainForm.ShowMessageBox(
                            "Bad data at custom tape " + CustomGrid.Rows[CustomTapeNumber].Cells["Name_Column"].Value.ToString() + ", X offset",
                            "Custom tape data error",
                            MessageBoxButtons.OK
                        );
                        return false;
                    }
                    if (!double.TryParse(CustomGrid.Rows[CustomTapeNumber].Cells["PartOffsetY_Column"].Value.ToString(), out HoleY))
                    {
                        MainForm.ShowMessageBox(
                            "Bad data at custom tape " + CustomGrid.Rows[CustomTapeNumber].Cells["Name_Column"].Value.ToString() + ", Y offset",
                            "Custom tape data error",
                            MessageBoxButtons.OK
                        );
                        return false;
                    }
                    // Go there:
                    if (!MainForm.CNC_XY_m(HoleX, HoleY))
                    {
                        return false;
                    };
                }
            }

            // Normal case:
			// Go to next hole approximate location:
			if (!SetCurrentTapeMeasurement_m(TapeNumber))  // having the measurement setup here helps with the automatic gain lag
				return false;

			double NextX= 0;
            double NextY = 0;
            if (!double.TryParse(Grid.Rows[TapeNumber].Cells["NextX_Column"].Value.ToString(), out NextX))
			{
				MainForm.ShowMessageBox(
                    "Bad data at Tape " + Grid.Rows[TapeNumber].Cells["IdColumn"].Value.ToString() + ", Next X",
					"Tape data error",
					MessageBoxButtons.OK
				);
				return false;
			}

            if (!double.TryParse(Grid.Rows[TapeNumber].Cells["NextY_Column"].Value.ToString(), out NextY))
			{
				MainForm.ShowMessageBox(
                    "Bad data at Tape " + Grid.Rows[TapeNumber].Cells["IdColumn"].Value.ToString() + ", Next Y",
					"Tape data error",
					MessageBoxButtons.OK
				);
				return false;
			}
			// Go there:
            if (!MainForm.CNC_XY_m(NextX, NextY))
			{
				return false;
			};

			// Get hole exact location:
            // We want to find the hole less than 2mm from where we think it should be. (Otherwise there is a risk
			// of picking a wrong hole.)
            if (!MainForm.GoToCircleLocation_m(1.8, 0.5, out HoleX, out HoleY))
			{
				MainForm.ShowMessageBox(
					"Can't find tape hole",
					"Tape error",
					MessageBoxButtons.OK
				);
				return false;
			}
			// The hole locations are:
            HoleX = Cnc.CurrentX + HoleX;
            HoleY = Cnc.CurrentY + HoleY;

			// ==================================================
			// find the part location and go there:
            double PartX = 0.0;
            double PartY = 0.0;
			double A= 0.0;

            if (!GetPartLocationFromHolePosition_m(TapeNumber, HoleX, HoleY, out PartX, out PartY, out A))
            {
                MainForm.ShowMessageBox(
                    "Can't find tape hole",
                    "Tape error",
                    MessageBoxButtons.OK
                );
            }

			// Now, PartX, PartY, A tell the position of the part. Take needle there:
			if (!Needle.Move_m(PartX, PartY, A))
			{
				return false;
			}

			return true;
		}	// end GotoNextPartByMeasurement_m


		// ========================================================================================
		// SetCurrentTapeMeasurement_m(): sets the camera measurement parameters according to the tape type.
		private bool SetCurrentTapeMeasurement_m(int row)
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

	// ========================================================================================
	// Custom Tapes (or trays, feeders etc.):

        private bool IsCustomTape(int TapeNumber, out int CustomTapeNumber)
        {
            // Tells if TapeNumber is custom tape, sets CustomTapeNumber
            string TapeName = Grid.Rows[TapeNumber].Cells["IdColumn"].Value.ToString();
            for (int i = 0; i < CustomGrid.RowCount-1; i++)
            {
                if (CustomGrid.Rows[i].Cells["Name_Column"].Value.ToString() == TapeName)
                {
                    CustomTapeNumber = i;
                    return true;
                }
            }
            CustomTapeNumber = -1;
            return false;


        }

        private bool GetCustomPartHole_m(int CustomTapeNum, int PartNum, double X, double Y, out double ResultX, out double ResultY)
        {
            ResultX = 0.0;
            ResultY = 0.0;
            return false;
        }

        // ========================================================================================
        // FindCustomTapeParameters(): We did not find the tape width from standard tapes, so the tape must be a custom tape.
        // This routine finds the tape number and the parameters from the custom tape name:
        private bool FindCustomTapeParameters(string Name, out int CustomTapeNum, out double OffsetX, out double OffsetY, out double Pitch)
        {
            OffsetY = 0.0;
            OffsetX = 0.0;
            Pitch = 0.0;
            CustomTapeNum = -1;
            foreach (DataGridViewRow GridRow in CustomGrid.Rows)
            {
                if (GridRow.Cells["Name_Column"].Value == null)
                {
                    break;
                }
                if (GridRow.Cells["Name_Column"].Value.ToString() == Name)
                {
                    // Found it!
                    CustomTapeNum= GridRow.Index;
                    DataGridViewRow Row = CustomGrid.Rows[CustomTapeNum];
                    if (!double.TryParse(Row.Cells["PitchColumn"].Value.ToString(), out Pitch))
                    {
                        MainForm.ShowMessageBox(
                            "Bad data at custom tape " + Name +", Pitch column",
                            "Data error",
                            MessageBoxButtons.OK
                        );
                        return false;
                    }

                    if (Convert.ToBoolean(Row.Cells["UsesLocationMarks_Column"].Value) == true)
                    {
                        if (!double.TryParse(Row.Cells["PartOffsetX_Column"].Value.ToString(), out OffsetX))
                        {
                            MainForm.ShowMessageBox(
                                "Bad data at custom tape " + Name + ", Part Offset X column",
                                "Data error",
                                MessageBoxButtons.OK
                            );
                            return false;
                        }
                        if (!double.TryParse(Row.Cells["PartOffsetY_Column"].Value.ToString(), out OffsetY))
                        {
                            MainForm.ShowMessageBox(
                                "Bad data at custom tape " + Name + ", Part Offset Y column",
                                "Data error",
                                MessageBoxButtons.OK
                            );
                            return false;
                        }
                        return true;
                    }
                    return true;
                }  // end "found it"
            }
            MainForm.ShowMessageBox(
                "Did not find custom tape " + Name,
                "Data error",
                MessageBoxButtons.OK
            );
            return false;
        }

        // ========================================================================================
        // Custom tapes are recognized by their name, which should be placed in the Width column

        private void ResetTapeWidths(DataGridViewComboBoxCell box)
        {
            box.Items.Clear();
            box.Items.Add("8/2mm");
            box.Items.Add("8/4mm");
            box.Items.Add("12/4mm");
            box.Items.Add("12/8mm");
            box.Items.Add("16/4mm");
            box.Items.Add("16/8mm");
            box.Items.Add("16/12mm");
            box.Items.Add("24/4mm");
            box.Items.Add("24/8mm");
            box.Items.Add("24/12mm");
            box.Items.Add("24/16mm");
            box.Items.Add("24/20mm");
        }

        public void AddCustomTapesToTapes()
        {
            if (Grid.RowCount==0)
            {
                return;
            }

            // There must be a cleaner way to do this! However, I didn't find it. :-(
            for (int i = 0; i < Grid.RowCount; i++)
            {
                DataGridViewComboBoxCell box = (DataGridViewComboBoxCell)Grid.Rows[i].Cells["WidthColumn"];
                // Clear and put in standard tape types
                ResetTapeWidths(box);
                // Add custom tape names
                for (int j = 0; j < CustomGrid.RowCount-1; j++)
                {
                    box.Items.Add(CustomGrid.Rows[j].Cells["Name_Column"].Value.ToString());
                }
            }

        }

        // AddWidthValues():
        // Loading a saved tape might have custom tape name in the width column. We need to add this manually to the available
        // selections for it to be visble. This function does that and is called after loading tapes grid.
        public void AddWidthValues()
        {
            for (int i = 0; i < Grid.RowCount; i++)
            {
                string value = Grid.Rows[i].Cells["WidthColumn"].Value.ToString();
                DataGridViewComboBoxCell box = (DataGridViewComboBoxCell)Grid.Rows[i].Cells["WidthColumn"];
                if (!box.Items.Contains(value))
                {
                    box.Items.Add(value);
                }
            }

        }

	}

}

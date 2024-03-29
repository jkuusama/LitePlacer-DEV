﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;

namespace LitePlacer
{
	class TapesClass
	{
        private DataGridView Grid;
        private NozzleCalibrationClass Nozzle;
		private FormMain MainForm;
		private Camera DownCamera;
		private CNC Cnc;

        public TapesClass(DataGridView grd, NozzleCalibrationClass ndl, Camera cam, CNC c, FormMain MainF)
		{
            Grid = grd;
			Nozzle = ndl;
			DownCamera = cam;
			MainForm = MainF;
			Cnc = c;
		}

		// ========================================================================================
		// ClearAll(): Resets TapeNumber positions and pickup/place Z's.
		public void ClearAll()
		{
            DialogResult dialogResult = MainForm.ShowMessageBox(
                "All tape locations will be reset to 1. Are you sure?",
                "Reset counts?", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                for (int tape = 0; tape < Grid.Rows.Count; tape++)
                {
                    Grid.Rows[tape].Cells["NextPart_Column"].Value = "1";
                    Grid.Rows[tape].Cells["Next_X_Column"].Value = Grid.Rows[tape].Cells["FirstX_Column"].Value;
                    Grid.Rows[tape].Cells["Next_Y_Column"].Value = Grid.Rows[tape].Cells["FirstY_Column"].Value;
                }
            }

        }

		// ========================================================================================
		// Reset(): Resets one tape position and pickup/place Z's.
		public void Reset(int tape)
		{
			Grid.Rows[tape].Cells["NextPart_Column"].Value = "1";
            // fix #22 reset next coordinates
            Grid.Rows[tape].Cells["Next_X_Column"].Value = Grid.Rows[tape].Cells["FirstX_Column"].Value;
			Grid.Rows[tape].Cells["Next_Y_Column"].Value = Grid.Rows[tape].Cells["FirstY_Column"].Value;
		}

		// ========================================================================================
		// IdValidates_m(): Checks that tape with description of "Id" exists.
		// TapeNumber is set to the corresponding row of the grid.
		public bool IdValidates_m(string Id, out int Tape)
		{
			Tape = -1;
			foreach (DataGridViewRow Row in Grid.Rows)
			{
				Tape++;
				if (Row.Cells["Id_Column"].Value.ToString() == Id)
				{
					return true;
				}
			}
			MainForm.ShowMessageBox(
				"Did not find tape " + Id.ToString(CultureInfo.InvariantCulture),
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
        public double FastXpos { get; set; }       // True position for the fist hole to be used
        public double FastYpos { get; set; }
        public double FastXstep { get; set; }       // step sizes for one hole to next
        public double FastYstep { get; set; }

        // ========================================================================================
        // PrepareForFastPlacement_m: Called before starting fast placement

        /* 
            A problem with a partial first hole remains! TODO: Handle case where first hole is the first in tape an dis cut in half
        */


        public bool PrepareForFastPlacement_m(string TapeID, int ComponentCount)
        {
            int TapeNum;
            if (!IdValidates_m(TapeID, out TapeNum))
            {
                FastParametersOk = false;
                return false;
            }
            if (MainForm.UseCoordinatesDirectly(TapeNum))
            {
                return true;
            }

            // get pitch
            double pitch;
            bool PitchIsTwo = false;
            if (!double.TryParse(Grid.Rows[TapeNum].Cells["Pitch_Column"].Value.ToString().Replace(',', '.'), out pitch))
            {
                MainForm.ShowMessageBox(
                    "Bad data at Pitch column, tape ID: " + Grid.Rows[TapeNum].Cells["Id_Column"].Value.ToString(),
                    "Data error",
                    MessageBoxButtons.OK);
                return false;
            }
            if ((pitch < 2.01) && (pitch > 1.99))
            {
                PitchIsTwo = true;
            }

            int FirstpartNo;
            if (!int.TryParse(Grid.Rows[TapeNum].Cells["NextPart_Column"].Value.ToString(), out FirstpartNo))
            {
                MainForm.ShowMessageBox(
                    "Bad data at next column",
                    "Sloppy programmer error",
                    MessageBoxButtons.OK);
                FastParametersOk = false;
                return false;
            }

            int LastPartNo = FirstpartNo + ComponentCount - 1;

            double FirstXpos;
            double FirstYpos;

            // measure holes:

            // **TODO:
            // If FirstpartNo == 1, the tape hole might not be there. Solution:
            /*
            if (FirstpartNo == 1)
            {
                - measure second hole (resA)
                - if last hole is >3, measure it, else measure hole #4 (resB)
                - use resA and resB to interpolate first hole position
                - calculate FastXstep && FastYstep
            }
            else
            {
                - do the below
            }
            */

             if (!GetPartHole_m(TapeNum, FirstpartNo, out FirstXpos, out FirstYpos))
            {
                FastParametersOk = false;
                return false;
            }

            double LastXpos = FirstXpos;
            double LastYpos = FirstYpos;

            if (LastPartNo != FirstpartNo)
            {
                if (!GetPartHole_m(TapeNum, LastPartNo, out LastXpos, out LastYpos))
                {
                    FastParametersOk = false;
                    return false;
                }
            }

            if (ComponentCount <= 1)
            {
                FastXstep = 0.0;
                FastYstep = 0.0;
            }
            else
            {
                if (PitchIsTwo)
                {
                    int starthole = (FirstpartNo + 1) / 2;
                    int lasthole = (LastPartNo + 1) / 2;
                    int HoleIncrement = lasthole - starthole;
                    if (HoleIncrement == 0)
                    {
                        FastXstep = 0.0;
                        FastYstep = 0.0;
                    }
                    else
                    {
                        FastXstep = (LastXpos - FirstXpos) / (double)HoleIncrement;
                        FastYstep = (LastYpos - FirstYpos) / (double)HoleIncrement;
                    }
                }
                else
                {
                    // normal case
                    FastXstep = (LastXpos - FirstXpos) / (double)(ComponentCount - 1);
                    FastYstep = (LastYpos - FirstYpos) / (double)(ComponentCount - 1);
                }
            }

            FastXpos = FirstXpos;
            FastYpos = FirstYpos;
            MainForm.DisplayText("Fast parameters:");
            MainForm.DisplayText("First X: " + FastXpos.ToString(CultureInfo.InvariantCulture)
                + ", Y: " + FastYpos.ToString(CultureInfo.InvariantCulture));
            MainForm.DisplayText("Last X: " + LastXpos.ToString(CultureInfo.InvariantCulture)
                + ", Y: " + LastYpos.ToString(CultureInfo.InvariantCulture));
            MainForm.DisplayText("Step X: " + FastXstep.ToString(CultureInfo.InvariantCulture)
                + ", Y: " + FastYstep.ToString(CultureInfo.InvariantCulture));

            return true;
        }

        // ========================================================================================
        // IncrementTape_Fast(): Updates count and next hole locations for a tape
        // Like IncrementTape(), but just using the fast parametersheader description
        public bool IncrementTape_Fast_m(int TapeNum)
        {
            // get current part number
            int pos;
            if (!int.TryParse(Grid.Rows[TapeNum].Cells["NextPart_Column"].Value.ToString(), out pos))
            {
                MainForm.ShowMessageBox(
                    "Bad data at next column",
                    "Sloppy programmer error",
                    MessageBoxButtons.OK);
                return false;
            }

            // get pitch
            double pitch = 0;
            if (!double.TryParse(Grid.Rows[TapeNum].Cells["Pitch_Column"].Value.ToString().Replace(',', '.'), out pitch))
            {
                MainForm.ShowMessageBox(
                    "Bad data at Pitch column, tape ID: " + Grid.Rows[TapeNum].Cells["Id_Column"].Value.ToString(),
                    "Data error",
                    MessageBoxButtons.OK);
                return false;
            }

            // Increment hole location, except if pitch is 2 and part number is odd
            if (!(
                ((pitch < 2.01) && (pitch > 1.99)) // pitch=2
                && 
                ((pos % 2) == 1)
                ))
            {
                FastXpos += FastXstep;
                FastYpos += FastYstep;
                Grid.Rows[TapeNum].Cells["Next_X_Column"].Value = FastXpos.ToString("0.000", CultureInfo.InvariantCulture);
                Grid.Rows[TapeNum].Cells["Next_Y_Column"].Value = FastYpos.ToString("0.000", CultureInfo.InvariantCulture);
            }
            pos += 1;
            Grid.Rows[TapeNum].Cells["NextPart_Column"].Value = pos.ToString(CultureInfo.InvariantCulture);
            return true;

    }

        // ========================================================================================
        // GetTapeParameters_m(): 
        private bool GetTapeParameters_m(int Tape, out double OffsetX, out double OffsetY, out double Pitch)
        {
            OffsetX = 0.0; 
            OffsetY = 0.0;
            Pitch = 0.0;
            // Check for values
            if (!double.TryParse(Grid.Rows[Tape].Cells["OffsetX_Column"].Value.ToString().Replace(',', '.'), out OffsetX))
            {
                MainForm.ShowMessageBox(
                    "Bad data at Tape " + Grid.Rows[Tape].Cells["ID_Column"].Value.ToString() + ", OffsetX",
                    "Tape data error",
                    MessageBoxButtons.OK
                );
                return false;
            }
            if (!double.TryParse(Grid.Rows[Tape].Cells["OffsetY_Column"].Value.ToString().Replace(',', '.'), out OffsetY))
            {
                MainForm.ShowMessageBox(
                    "Bad data at Tape " + Grid.Rows[Tape].Cells["ID_Column"].Value.ToString() + ", OffsetY",
                    "Tape data error",
                    MessageBoxButtons.OK
                );
                return false;
            }
            if (!double.TryParse(Grid.Rows[Tape].Cells["Pitch_Column"].Value.ToString().Replace(',', '.'), out Pitch))
            {
                MainForm.ShowMessageBox(
                    "Bad data at Tape " + Grid.Rows[Tape].Cells["ID_Column"].Value.ToString() + ", Pitch",
                    "Tape data error",
                    MessageBoxButtons.OK
                );
                return false;
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
            double A = 0.0;
            if (!double.TryParse(Grid.Rows[TapeNum].Cells["FirstX_Column"].Value.ToString().Replace(',', '.'), out X))
            {
                MainForm.ShowMessageBox(
                    "Bad data at Tape " + TapeNum.ToString(CultureInfo.InvariantCulture) + ", X",
                    "Tape data error",
                    MessageBoxButtons.OK
                );
                return false;
            }
            if (!double.TryParse(Grid.Rows[TapeNum].Cells["FirstY_Column"].Value.ToString().Replace(',', '.'), out Y))
            {
                MainForm.ShowMessageBox(
                    "Bad data at Tape " + TapeNum.ToString(CultureInfo.InvariantCulture) + ", Y",
                    "Tape data error",
                    MessageBoxButtons.OK
                );
                return false;
            }

            // Get the hole location guess
            double dW;
            double Pitch;
            double FromHole;
            if (!GetTapeParameters_m(TapeNum, out dW, out FromHole, out Pitch))
            {
                return false;
            }
            if (Math.Abs(Pitch-2.0)<0.01) // if pitch ==2
            {
                PartNum = (PartNum +1)/ 2;
                Pitch = 4.0;
            }
            double dist = (double)(PartNum-1) * Pitch; // This many mm's from start
            switch (Grid.Rows[TapeNum].Cells["Orientation_Column"].Value.ToString())
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
                        "Bad data at Tape #" + TapeNum.ToString(CultureInfo.InvariantCulture) + ", Orientation",
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
            if (!MainForm.CNC_XYA_m(X, Y, Cnc.CurrentA))
            {
                return false;
            };

            // get hole exact location:
            bool ok = true;
            do
            {
                ok = true;
                if (!MainForm.GoToFeatureLocation_m(0.2, out X, out Y, out A))
                {
                    ok = false;
                    string nl = Environment.NewLine;
                    string answer = MainForm.NonModalMessageBox(
                        "Tape hole recognition failed." + nl +
                        "Jog machine to position and/or tune the algorithm." + nl +
                        "Click \"Retry\" to try again," + nl +
                        "click \"Cancel\" to continue without success", "Tape hole not found",
                        "Retry", "", "Cancel");
                    if (answer == "Cancel")
                    {
                        return false;
                    }
                    if (!SetCurrentTapeMeasurement_m(TapeNum)) 
                    {
                        return false;
                    }
                    Thread.Sleep(100);
                }
            } 
            while (!ok);

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
            double dL=2.0;   // Part center pos from hole, tape lenght direction. -2mm on all standard tapes
			double Pitch;  // Distance from one part to another

            if (!GetTapeParameters_m(Tape, out dW, out dL, out Pitch))
	        {
		        return false;
	        }
            int pos;
			if (!int.TryParse(Grid.Rows[Tape].Cells["NextPart_Column"].Value.ToString(), out pos))
			{
				MainForm.ShowMessageBox(
					"Bad data at Tape " + Tape.ToString(CultureInfo.InvariantCulture) + ", Next",
					"Tape data error",
					MessageBoxButtons.OK
				);
				return false;
			}
            // if pitch == 2 and part# is even, DL=0
            if (Math.Abs(Pitch - 2) < 0.01)
            {
                if ((pos % 2) == 0)
                {
                    dL = 0.0;
                }
            }

			// TapeNumber orientation: 
			// +Y: Holeside of tape is right, part is dW(mm) to left, dL(mm) down from hole, A= 0
			// +X: Holeside of tape is down, part is dW(mm) up, dL(mm) to left from hole, A= -90
			// -Y: Holeside of tape is left, part is dW(mm) to right, dL(mm) up from hole, A= -180
			// -X: Holeside of tape is up, part is dW(mm) down, dL(mm) to right from hole, A=-270
			switch (Grid.Rows[Tape].Cells["Orientation_Column"].Value.ToString())
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
					PartX = X + dL;
					PartY = Y - dW;
					A = -270.0;
					break;

				default:
					MainForm.ShowMessageBox(
						"Bad data at Tape #" + Tape.ToString(CultureInfo.InvariantCulture) + ", Orientation",
						"Tape data error",
						MessageBoxButtons.OK
					);
					return false;
			}
            // rotation:
            if (Grid.Rows[Tape].Cells["Rotation_Column"].Value == null)
			{
				MainForm.ShowMessageBox(
					"Bad data at tape " + Grid.Rows[Tape].Cells["Id_Column"].Value.ToString() +" rotation",
					"Assertion error",
					MessageBoxButtons.OK
				);
				return false;
			}
			switch (Grid.Rows[Tape].Cells["Rotation_Column"].Value.ToString())
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
						"Bad data at Tape " + Grid.Rows[Tape].Cells["Id_Column"].Value.ToString() + " rotation",
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
            MainForm.DisplayText("Part position: " + Grid.Rows[Tape].Cells["Id_Column"].Value.ToString() +
                ", part #" + pos.ToString(CultureInfo.InvariantCulture) +
                ": X= " + PartX.ToString(CultureInfo.InvariantCulture) +
                ", Y= " + PartY.ToString(CultureInfo.InvariantCulture) +
                ", A= " + A.ToString(CultureInfo.InvariantCulture));
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
            if (!GetTapeParameters_m(Tape, out dW, out dL, out Pitch))
            {
                return false;
            }

            int pos;
            if (!int.TryParse(Grid.Rows[Tape].Cells["NextPart_Column"].Value.ToString(), out pos))
			{
				MainForm.ShowMessageBox(
                    "Bad data at Tape " + Grid.Rows[Tape].Cells["Id_Column"].Value.ToString() + ", next",
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
            switch (Grid.Rows[Tape].Cells["Orientation_Column"].Value.ToString())
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
            Grid.Rows[Tape].Cells["Next_X_Column"].Value = HoleX.ToString(CultureInfo.InvariantCulture);
            Grid.Rows[Tape].Cells["Next_Y_Column"].Value = HoleY.ToString(CultureInfo.InvariantCulture);
            // increment next count
            pos++;
            Grid.Rows[Tape].Cells["NextPart_Column"].Value = pos.ToString(CultureInfo.InvariantCulture);
            return true;
        }

        // ========================================================================================
        // UpdateNextCoordinates(): Updates next coordinates for a given tape based on new next coordinate number
        public bool UpdateNextCoordinates(int Tape, int NextNo)
        {
            double dW;	// Part center pos from hole, tape width direction. Varies.
            double dL;   // Part center pos from hole, tape lenght direction. -2mm on all standard tapes
            double Pitch;  // Distance from one part to another
            if (!GetTapeParameters_m(Tape, out dW, out dL, out Pitch))
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

            // determine first hole coordinates
            double Hole1X;
            double Hole1Y;

            NumberStyles style = NumberStyles.AllowDecimalPoint;
            CultureInfo culture = CultureInfo.InvariantCulture;
            if (Grid.Rows[Tape].Cells["FirstX_Column"].Value == null)
            {
                return false;
            }
            string s = Grid.Rows[Tape].Cells["FirstX_Column"].Value.ToString();           
            if (!double.TryParse(s, style, culture, out Hole1X))
            {
                return false;
            }
            if (Grid.Rows[Tape].Cells["FirstY_Column"].Value == null)
            {
                return false;
            }
            s = Grid.Rows[Tape].Cells["FirstY_Column"].Value.ToString();
            if (!double.TryParse(s, style, culture, out Hole1Y))
            {
                return false;
            }

            double NextX = Hole1X;
            double NextY = Hole1Y;

            // calculate next coordinates based on tape orientation, 1st hole position and offset from above
            switch (Grid.Rows[Tape].Cells["Orientation_Column"].Value.ToString())
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

            Grid.Rows[Tape].Cells["Next_X_Column"].Value = NextX.ToString("0.000", CultureInfo.InvariantCulture);
            Grid.Rows[Tape].Cells["Next_Y_Column"].Value = NextY.ToString("0.000", CultureInfo.InvariantCulture);

            return true;
        }

        // ========================================================================================
        // GotoNextPartByMeasurement_m(): Takes Nozzle to exact location of the part, tape and part rotation taken in to account.
        // The hole position is measured on each call using tape holes and knowledge about tape width and pitch (see EIA-481 standard).
        // Id tells the tape name. 
        // The caller needs the hole coordinates and tape number later in the process, but they are measured and returned here.
        public bool GotoNextPartByMeasurement_m(int TapeNumber, out double HoleX, out double HoleY)
		{
            HoleX = 0;
            HoleY = 0;
			// Go to next hole approximate location:
			if (!SetCurrentTapeMeasurement_m(TapeNumber))  // having the measurement setup here helps with the automatic gain lag
            {
                return false;
            }

            double NextX = 0;
            double NextY = 0;
            if (!double.TryParse(Grid.Rows[TapeNumber].Cells["Next_X_Column"].Value.ToString().Replace(',', '.'), out NextX))
			{
				MainForm.ShowMessageBox(
                    "Bad data at Tape " + Grid.Rows[TapeNumber].Cells["Id_Column"].Value.ToString() + ", Next X",
					"Tape data error",
					MessageBoxButtons.OK
				);
				return false;
			}

            if (!double.TryParse(Grid.Rows[TapeNumber].Cells["Next_Y_Column"].Value.ToString().Replace(',', '.'), out NextY))
			{
				MainForm.ShowMessageBox(
                    "Bad data at Tape " + Grid.Rows[TapeNumber].Cells["Id_Column"].Value.ToString() + ", Next Y",
					"Tape data error",
					MessageBoxButtons.OK
				);
				return false;
			}
			// Go there:
            if (!MainForm.CNC_XYA_m(NextX, NextY, Cnc.CurrentA))
			{
				return false;
			};

            // Get hole exact location:
            // We want to find the hole less than 2mm from where we think it should be. (Otherwise there is a risk
            // of picking a wrong hole.)
            double A = 0.0;
            bool ok = true;
            do
            {
                ok = true;
                if (!MainForm.GoToFeatureLocation_m(0.5, out HoleX, out HoleY, out A))
                {
                    ok = false;
                    string nl = Environment.NewLine;
                    string answer = MainForm.NonModalMessageBox(
                        "Tape hole recognition failed." + nl +
                        "Jog machine to position and/or tune the algorithm." + nl +
                        "Click \"Retry\" to try again," + nl +
                        "click \"Cancel\" to continue without success", "Tape hole not found",
                        "Retry", "", "Cancel");
                    if (answer == "Cancel")
                    {
                        return false;
                    }
                    if (!SetCurrentTapeMeasurement_m(TapeNumber)) 
                    {
                        return false;
                    }
                    Thread.Sleep(100);
                }
            }
            while (!ok);

			// The hole locations are:
            HoleX = Cnc.CurrentX + HoleX;
            HoleY = Cnc.CurrentY + HoleY;

			// ==================================================
			// find the part location and go there:
            double PartX = 0.0;
            double PartY = 0.0;

            if (!GetPartLocationFromHolePosition_m(TapeNumber, HoleX, HoleY, out PartX, out PartY, out A))
            {
                MainForm.ShowMessageBox(
                    "Can't find tape hole",
                    "Tape error",
                    MessageBoxButtons.OK
                );
            }

			// Now, PartX, PartY, A tell the position of the part. Take Nozzle there:
			if (!Nozzle.Move_m(PartX, PartY, A))
			{
				return false;
			}

			return true;
		}	// end GotoNextPartByMeasurement_m


		// ========================================================================================
		// SetCurrentTapeMeasurement_m(): sets the camera measurement parameters according to the tape type.
		private bool SetCurrentTapeMeasurement_m(int row)
		{
            VideoAlgorithmsCollection.FullAlgorithmDescription TapeAlg = new VideoAlgorithmsCollection.FullAlgorithmDescription();
            string TapeAlgName = Grid.Rows[row].Cells["Type_Column"].Value.ToString();
            if (!MainForm.VideoAlgorithms.FindAlgorithm(TapeAlgName, out TapeAlg))
            {
                MainForm.DisplayText("SetCurrentTapeMeasurement_m: *** Tape algorithm (" + TapeAlgName + ") not found", KnownColor.Red, true);
                return false;
            }
            MainForm.DisplayText("SetCurrentTapeMeasurement_m: using alg " + TapeAlgName);
            DownCamera.BuildMeasurementFunctionsList(TapeAlg.FunctionList);
            DownCamera.MeasurementParameters = TapeAlg.MeasurementParameters;
            return true;
		}

	}
}

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
using System.Web.Script.Serialization;
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
    public partial class FormMain   {
        // =================================================================================
        //CAD data reading functions: Tries to understand different pick and place file formats
        // =================================================================================


        // =================================================================================
        // CADdataToMMs_m(): Data was in inches, convert to mms

        private bool CADdataToMMs_m() {
            double val;
            foreach (DataGridViewRow Row in CadData_GridView.Rows) {
                if (!double.TryParse(Row.Cells["X_nominal"].Value.ToString(), out val)) {
                    ShowMessageBox(
                        "Problem with " + Row.Cells["Component"].Value.ToString() + " X coordinate data",
                        "Bad data",
                        MessageBoxButtons.OK);
                    return false;
                };
                Row.Cells["X_nominal"].Value = Math.Round((val / 2.54), 3).ToString();
                if (!double.TryParse(Row.Cells["Y_nominal"].Value.ToString(), out val)) {
                    ShowMessageBox(
                        "Problem with " + Row.Cells["Component"].Value.ToString() + " Y coordinate data",
                        "Bad data",
                        MessageBoxButtons.OK);
                    return false;
                };
                Row.Cells["Y_nominal"].Value = Math.Round((val / 2.54), 3).ToString();
            }
            return true;
        }

        // =================================================================================
        // ParseKiCadData_m()
        // =================================================================================
        private bool ParseKiCadData_m(String[] AllLines) {
            // Convert KiCad data to regular CSV
            int i = 0;
            bool inches = false;
            // Skip headers until find one starting with "## "
            while (!(AllLines[i].StartsWith("## "))) {
                i++;
            };

            // inches vs mms
            if (AllLines[i++].Contains("inches")) {
                inches = true;
            }
            i++; // skip the "Side" line
            List<string> KiCadLines = new List<string>();
            KiCadLines.Add(AllLines[i++].Substring(2));  // add header, skip the "# " start
            // add rest of the lines
            while (!(AllLines[i]).StartsWith("## End")) {
                KiCadLines.Add(AllLines[i++]);
            };
            // parse the data
            string[] KicadArr = KiCadLines.ToArray();
            if (!ParseCadData_m(KicadArr, true)) {
                return false;
            };
            // convert to mm'f if needed
            if (inches) {
                return (CADdataToMMs_m());
            } else {
                return true;
            }
        }

        // =================================================================================
        // ParseCadData_m(): main function called from file open
        // =================================================================================

        // =================================================================================
        // FindDelimiter_m(): Tries to find the difference with comma and semicolon separated files  
        bool FindDelimiter_m(String Line, out char delimiter) {
            int commas = 0;
            foreach (char c in Line) {
                if (c == ',') {
                    commas++;
                }
            };
            int semicolons = 0;
            foreach (char c in Line) {
                if (c == ';') {
                    semicolons++;
                }
            };
            if ((commas == 0) && (semicolons > 4)) {
                delimiter = ';';
                return true;
            };
            if ((semicolons == 0) && (commas > 4)) {
                delimiter = ',';
                return true;
            };

            ShowMessageBox(
                "File header parse fail",
                "Data format error",
                MessageBoxButtons.OK
            );
            delimiter = ',';
            return false;
        }

        private bool ParseCadData_m(String[] AllLines, bool KiCad) {
            int ComponentIndex;
            int ValueIndex;
            int FootPrintIndex;
            int X_Nominal_Index;
            int Y_Nominal_Index;
            int RotationIndex;
            int LayerIndex = -1;
            bool LayerDataPresent = false;
            int i;
            int LineIndex = 0;

            // Parse header. Skip empty lines and comment lines (starting with # or "//")
            foreach (string s in AllLines) {
                if (s == "") {
                    LineIndex++;
                    continue;
                }
                if (s[0] == '#') {
                    LineIndex++;
                    continue;
                };
                if ((s.Length > 1) && (s[0] == '/') && (s[1] == '/')) {
                    LineIndex++;
                    continue;
                };
                break;
            };

            char delimiter;
            if (KiCad) {
                delimiter = ' ';
            } else {
                if (!FindDelimiter_m(AllLines[0], out delimiter)) {
                    return false;
                };
            }

            List<String> Headers = SplitCSV(AllLines[LineIndex++], delimiter);

            for (i = 0; i < Headers.Count; i++) {
                if ((Headers[i] == "Designator") ||
                    (Headers[i] == "designator") ||
                    (Headers[i] == "Part") ||
                    (Headers[i] == "part") ||
                    (Headers[i] == "RefDes") ||
                    (Headers[i] == "Ref") ||
                    (Headers[i] == "Component") ||
                    (Headers[i] == "component")
                  ) {
                    break;
                }
            }
            if (i >= Headers.Count) {
                ShowMessageBox("Component/Designator/Name not found in header line", "Syntax error", MessageBoxButtons.OK);
                return false;
            }
            ComponentIndex = i;

            for (i = 0; i < Headers.Count; i++) {
                if ((Headers[i] == "Value") ||
                    (Headers[i] == "value") ||
                    (Headers[i] == "Val") ||
                    (Headers[i] == "val") ||
                    (Headers[i] == "Comment") ||
                    (Headers[i] == "comment")
                  ) {
                    break;
                }
            }
            if (i >= Headers.Count) {
                ShowMessageBox("Component value/comment not found in header line", "Syntax error", MessageBoxButtons.OK);
                return false;
            }
            ValueIndex = i;

            for (i = 0; i < Headers.Count; i++) {
                if ((Headers[i] == "Footprint") ||
                    (Headers[i] == "footprint") ||
                    (Headers[i] == "Package") ||
                    (Headers[i] == "package") ||
                    (Headers[i] == "Pattern") ||
                    (Headers[i] == "pattern")
                  ) {
                    break;
                }
            }
            if (i >= Headers.Count) {
                ShowMessageBox("Component footprint/pattern not found in header line", "Syntax error", MessageBoxButtons.OK);
                return false;
            }
            FootPrintIndex = i;

            for (i = 0; i < Headers.Count; i++) {
                if ((Headers[i] == "X") ||
                    (Headers[i] == "x") ||
                    (Headers[i] == "X (mm)") ||
                    (Headers[i] == "x (mm)") ||
                    (Headers[i] == "PosX") ||
                    (Headers[i] == "Ref X") ||
                    (Headers[i] == "ref x")
                  ) {
                    break;
                }
            }
            if (i >= Headers.Count) {
                ShowMessageBox("Component X not found in header line", "Syntax error", MessageBoxButtons.OK);
                return false;
            }
            X_Nominal_Index = i;

            for (i = 0; i < Headers.Count; i++) {
                if ((Headers[i] == "Y") ||
                    (Headers[i] == "y") ||
                    (Headers[i] == "Y (mm)") ||
                    (Headers[i] == "y (mm)") ||
                    (Headers[i] == "PosY") ||
                    (Headers[i] == "Ref Y") ||
                    (Headers[i] == "ref y")
                  ) {
                    break;
                }
            }
            if (i >= Headers.Count) {
                ShowMessageBox("Component Y not found in header line", "Syntax error", MessageBoxButtons.OK);
                return false;
            }
            Y_Nominal_Index = i;

            for (i = 0; i < Headers.Count; i++) {
                if ((Headers[i] == "Rotation") ||
                    (Headers[i] == "rotation") ||
                    (Headers[i] == "Rot") ||
                    (Headers[i] == "rot") ||
                    (Headers[i] == "Rotate")
                  ) {
                    break;
                }
            }
            if (i >= Headers.Count) {
                ShowMessageBox("Component rotation not found in header line", "Syntax error", MessageBoxButtons.OK);
                return false;
            }
            RotationIndex = i;


            for (i = 0; i < Headers.Count; i++) {
                if ((Headers[i] == "Layer") ||
                    (Headers[i] == "layer") ||
                    (Headers[i] == "Side") ||
                    (Headers[i] == "side") ||
                    (Headers[i] == "TB") ||
                    (Headers[i] == "tb")
                  ) {
                    LayerIndex = i;
                    LayerDataPresent = true;
                    break;
                }
            }

            // clear and rebuild the data tables
            CadData_GridView.Rows.Clear();
            JobData_GridView.Rows.Clear();
            foreach (DataGridViewColumn column in JobData_GridView.Columns) {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;   // disable manual sort
            }

            // Parse data
            List<String> Line;
            string peek;

            for (i = LineIndex; i < AllLines.Count(); i++)   // for each component
            {
                peek = AllLines[i];
                // Skip: empty lines and comment lines (starting with # or "//")
                if (
                        (AllLines[i] == "")  // empty lines
                        ||
                        (AllLines[i] == "\"\"")  // empty lines ("")
                        ||
                        (AllLines[i][0] == '#')  // comment lines starting with #
                        ||
                        ((AllLines[i].Length > 1) && (AllLines[i][0] == '/') && (AllLines[i][1] == '/'))  // // comment lines starting with //
                    ) {
                    continue;
                }

                Line = SplitCSV(AllLines[i], delimiter);
                // If layer is indicated and the component is not on this layer, skip it
                if (LayerDataPresent) {
                    if (Bottom_checkBox.Checked) {
                        if ((Line[LayerIndex] == "Top") ||
                            (Line[LayerIndex] == "top") ||
                            (Line[LayerIndex] == "F.Cu") ||
                            (Line[LayerIndex] == "T") ||
                            (Line[LayerIndex] == "t")) {
                            continue;
                        }
                    } else {
                        if ((Line[LayerIndex] == "Bottom") ||
                            (Line[LayerIndex] == "bottom") ||
                            (Line[LayerIndex] == "B") ||
                            (Line[LayerIndex] == "b") ||
                            (Line[LayerIndex] == "B.Cu") ||
                            (Line[LayerIndex] == "Bot") ||
                            (Line[LayerIndex] == "bot")) {
                            continue;
                        }
                    }
                }
                CadData_GridView.Rows.Add();
                int Last = CadData_GridView.RowCount - 1;
                CadData_GridView.Rows[Last].Cells["Component"].Value = Line[ComponentIndex];
                CadData_GridView.Rows[Last].Cells["Value_Footprint"].Value = Line[ValueIndex] + "  |  " + Line[FootPrintIndex];
                if (LayerDataPresent) {
                    if (Bottom_checkBox.Checked) {
                        CadData_GridView.Rows[Last].Cells["X_nominal"].Value = "-" + Line[X_Nominal_Index].Replace("mm", "");
                    } else {
                        CadData_GridView.Rows[Last].Cells["X_nominal"].Value = Line[X_Nominal_Index].Replace("mm", "");
                    }
                } else {
                    CadData_GridView.Rows[Last].Cells["X_nominal"].Value = Line[X_Nominal_Index].Replace("mm", "");
                }
                CadData_GridView.Rows[Last].Cells["Y_nominal"].Value = Line[Y_Nominal_Index].Replace("mm", "");
                CadData_GridView.Rows[Last].Cells["X_nominal"].Value = CadData_GridView.Rows[Last].Cells["X_nominal"].Value.ToString().Replace(",", ".");
                CadData_GridView.Rows[Last].Cells["Y_nominal"].Value = CadData_GridView.Rows[Last].Cells["Y_nominal"].Value.ToString().Replace(",", ".");
                CadData_GridView.Rows[Last].Cells["Rotation"].Value = Line[RotationIndex];
                CadData_GridView.Rows[Last].Cells["X_Machine"].Value = "Nan";   // will be set later 
                CadData_GridView.Rows[Last].Cells["Y_Machine"].Value = "Nan";
                CadData_GridView.Rows[Last].Cells["Rotation_machine"].Value = "Nan";
            }   // end "for each component..."

            // Disable manual sorting
            foreach (DataGridViewColumn column in CadData_GridView.Columns) {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            CadData_GridView.ClearSelection();
            // Check, that our data is good:
            if (!ValidateCADdata_m())
                return false;

            return true;
        }   // end ParseCadData

        // =================================================================================
        // Helper function for ParseCadData() (and some others, hence public static)

        public static List<String> SplitCSV(string Line, char delimiter) {
            // input lines can be "xxx","xxxx","xx"; output is array: xxx  xxxxx  xx
            // or xxx,xxxx,xx; output is array: xxx  xxxx  xx
            // or xxx,"xx,xx",xxxx; output is array: xxx  xx,xx  xxxx

            List<String> Tokens = new List<string>();

            while (Line != "") {
                // skip the delimiter(s)
                while (Line[0] == delimiter) {
                    Line = Line.Substring(1);
                };
                // add token
                if (Line[0] == '"') {
                    // token is "xxx"
                    Line = Line.Substring(1);   // skip the first "
                    Tokens.Add(Line.Substring(0, Line.IndexOf('"')));
                    Line = Line.Substring(Line.IndexOf('"') + 1);
                } else {
                    // token does not have "" 's
                    if (Line.IndexOf(delimiter) < 0) {
                        Tokens.Add(Line);   // last element
                        Line = "";
                    } else {
                        Tokens.Add(Line.Substring(0, Line.IndexOf(delimiter)));
                        Line = Line.Substring(Line.IndexOf(delimiter));
                    }
                }
            }
            return (Tokens);
        }


    }
}

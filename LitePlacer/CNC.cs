using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace LitePlacer {

    [Serializable]
    public class StatusReport {
        public Sr sr { get; set; }
    }

    [Serializable]
    public class Sr {
        // mpox, posy, ...: Position
        // NOTE: Some firmware versions use mpox, mpoy,... some use posx, posy, ... 
        // This should be reflected in the public variable names
        private double _posx;
        public double posx // <======================== here
        {
            get { return _posx; }
            set {
                _posx = value;
                CNC.setCurrX(_posx);
                Global.Instance.mainForm.ValueUpdater("posx", _posx.ToString("0.000", CultureInfo.InvariantCulture));
            }
        }

        private double _posy;
        public double posy // <======================== and here
        {
            get { return _posy; }
            set {
                _posy = value;
                CNC.setCurrY(_posy);
                Global.Instance.mainForm.ValueUpdater("posy", _posy.ToString("0.000", CultureInfo.InvariantCulture));
            }
        }

        private double _posz;
        public double posz // <======================== and here
        {
            get { return _posz; }
            set {
                _posz = value;
                CNC.setCurrZ(_posz);
                Global.Instance.mainForm.ValueUpdater("posz", _posz.ToString("0.000", CultureInfo.InvariantCulture));
            }
        }

        private double _posa;
        public double posa // <======================== and here
        {
            get { return _posa; }
            set {
                _posa = value;
                CNC.setCurrA(_posa);
                Global.Instance.mainForm.ValueUpdater("posa", _posa.ToString("0.000", CultureInfo.InvariantCulture));
            }
        }

    }

    public class CNC {
        private static FormMain MainForm;
        private SerialComm Com;
        public bool JoggingBusy;
        public bool AbortPlacement;

        static ManualResetEventSlim _readyEvent = new ManualResetEventSlim(false);

        public void ShowSimpleMessageBox(string msg) {
            Global.Instance.mainForm.ShowSimpleMessageBox(msg);
        }

        public CNC(FormMain mf) {
            MainForm = mf;
            Com = new SerialComm { serialDelegate = InterpretLine };
            Connect(Properties.Settings.Default.CNC_SerialPort);
        }


        public bool Connected {            get {                return Com.IsOpen;            }        }

        public void Close() {
            if (Connected) Com.Close();
            _readyEvent.Set();
            MainForm.UpdateCncConnectionStatus();
        }

        public bool Connect(String name) {
            // For now, just see that the port opens. 
            // TODO: check that there isTinyG, not just any comm port.
            // TODO: check/set default values

            if (Connected) Com.Close();
            Com.Open(name);
            _readyEvent.Set();
            return Com.IsOpen;
        }


        public bool RawWrite(string command) {
            if (!Com.IsOpen) {
                MainForm.DisplayText("###" + command + " discarded, com not open");
                return false;
            }
            Com.Write(command);
            return true;
        }

        // Square compensation:
        // The machine will be only approximately square. Fortunately, the squareness is easy to measure with camera.
        // User measures correction value, that we apply to movements and reads.
        // For example, correction value is +0.002, meaning that for every unit of +Y movement, 
        // the machine actually also unintentionally moves 0.002 units to +X. 
        // Therefore, for each movement when the user wants to go to (X, Y),
        // we really go to (X - 0.002*Y, Y)

        // CurrentX/Y is the corrected value that user sees and uses, and reflects a square machine
        // TrueX/Y is what the TinyG actually uses.

        public static double SquareCorrection { get; set; }
        private static double CurrX;
        private static double _trueX;

        public PartLocation XYLocation {
            get { return new PartLocation(CurrentX, CurrentY); }
            set { CurrentX = value.X; CurrentY = value.Y; }
        }

        public PartLocation XYALocation {
            get { return new PartLocation(CurrentX, CurrentY, CurrentA); }
            set { CurrentX = value.X; CurrentY = value.Y; CurrentA = value.A; }
        }

        public double TrueX {
            get { return (_trueX); }
            set { _trueX = value; }
        }

        public double CurrentX {
            get { return (CurrX); }
            set { CurrX = value; }
        }

        public static void setCurrX(double x) {
            _trueX = x;
            CurrX = x - CurrY * SquareCorrection;
            // MainForm.DisplayText("CNC.setCurrX: x= " + x.ToString() + ", CurrX= " + CurrX.ToString());
        }

        private static double CurrY;
        public double CurrentY {
            get { return (CurrY); }
            set { CurrY = value; }
        }

        public static void setCurrY(double y) {
            CurrY = y;
            CurrX = _trueX - CurrY * SquareCorrection;
            // MainForm.DisplayText("CNC.setCurrY: TrueX= " + TrueX.ToString() + ", CurrX= " + CurrX.ToString());
        }

        private static double CurrZ;
        public double CurrentZ {
            get { return (CurrZ); }
            set { CurrZ = value; }
        }
        public static void setCurrZ(double z) {
            CurrZ = z;
        }

        private static double CurrA;
        public double CurrentA {
            get {
                return (CurrA);
            }
            set {
                CurrA = value;
            }
        }
        public static void setCurrA(double a) {
            CurrA = a;
        }



        public bool SlackCompensation { get; set; }
        private double SlackCompensationDistance = 0.4;

        public string SmallMovementString = "G1 F200 ";

        public void XY(double X, double Y) {
            if (!SlackCompensation) {
                XY_move(X, Y);
            } else {
                XY_move(X - SlackCompensationDistance, Y - SlackCompensationDistance);
                XY_move(X, Y);
            }
        }

        private void XY_move(double X, double Y) {
            string command;
            double dX = Math.Abs(X - CurrentX);
            double dY = Math.Abs(Y - CurrentY);
            if ((dX < 0.004) && (dY < 0.004)) {
                MainForm.DisplayText(" -- zero XY movement command --");
                MainForm.DisplayText("ReadyEvent: zero movement command");
                _readyEvent.Set();
                return;   // already there
            }
            if ((dX < 1) && (dY < 1)) {
                command = SmallMovementString;
            } else {
                command = "G0 ";
            }
            X = X + SquareCorrection * Y;
            X = Math.Round(X, 3);
            command = command + "X" + X.ToString(CultureInfo.InvariantCulture) + " Y" + Y.ToString(CultureInfo.InvariantCulture);
            _readyEvent.Reset();
            //Com.Write(command);
            Com.Write("{\"gc\":\"" + command + "\"}");
            _readyEvent.Wait();
        }

        public void XYA(double X, double Y, double Am) {
            if (!SlackCompensation) {
                XYA_move(X, Y, Am);
            } else {
                XYA_move(X - SlackCompensationDistance, Y - SlackCompensationDistance, Am - 10);
                XYA_move(X, Y, Am);
            }
        }

        private void XYA_move(double X, double Y, double Am) {
            string command;
            double dX = Math.Abs(X - CurrentX);
            double dY = Math.Abs(Y - CurrentY);
            double dA = Math.Abs(Am - CurrentA);
            if ((dX < 0.004) && (dY < 0.004) && (dA < 0.01)) {
                MainForm.DisplayText(" -- zero XYA movement command --");
                MainForm.DisplayText("ReadyEvent: zero movement command");
                _readyEvent.Set();
                return;   // already there
            }
            if (((dX > 1) && (dY > 1)) && (dA > 5)) {
                // normal case
                X = X + SquareCorrection * Y;
                command = "G0 " + "X" + X.ToString(CultureInfo.InvariantCulture) +
                                  " Y" + Y.ToString(CultureInfo.InvariantCulture) +
                                  " A" + Am.ToString(CultureInfo.InvariantCulture);
                _readyEvent.Reset();
                //Com.Write(command);
                MainForm.DisplayText(command);
                Com.Write("{\"gc\":\"" + command + "\"}");
                _readyEvent.Wait();
            } else {
                // either XY or A (or both) is a small movement
                X = X + SquareCorrection * Y;
                if ((dX < 1.1) && (dY < 1.1)) {
                    command = SmallMovementString + "X" + X.ToString(CultureInfo.InvariantCulture) + " Y" + Y.ToString(CultureInfo.InvariantCulture);
                } else {
                    command = "G0 " + "X" + X.ToString(CultureInfo.InvariantCulture) + " Y" + Y.ToString(CultureInfo.InvariantCulture);
                }
                _readyEvent.Reset();
                //Com.Write(command);
                MainForm.DisplayText(command);
                Com.Write("{\"gc\":\"" + command + "\"}");
                _readyEvent.Wait();
                A(Am);
            }
        }


        public void Z(double Z) {
            string command;
            double dZ = Math.Abs(Z - CurrentZ);
            if (dZ < 0.005) {
                MainForm.DisplayText(" -- zero Z movement command --");
                MainForm.DisplayText("ReadyEvent: zero movement command");
                _readyEvent.Set();
                return;   // already there
            }
            if (dZ < 1.1) {
                command = "G1 Z" + Z.ToString(CultureInfo.InvariantCulture);
            } else {
                command = "G0 Z" + Z.ToString(CultureInfo.InvariantCulture);
            }
            _readyEvent.Reset();
            //Com.Write(command);
            MainForm.DisplayText(command);
            Com.Write("{\"gc\":\"" + command + "\"}");
            _readyEvent.Wait();
        }

        public void A(double A) {
            if (Math.Abs(A - CurrentA) < 0.01) {
                MainForm.DisplayText(" -- zero A movement command --");
                _readyEvent.Set();
                return;   // already there
            }
            string command;
            if (Math.Abs(A - CurrentA) < 5) {
                command = "G1 F3000 A" + A.ToString(CultureInfo.InvariantCulture);
            } else {
                command = "G0 A" + A.ToString(CultureInfo.InvariantCulture);
            }
            _readyEvent.Reset();
            //Com.Write(command);
            MainForm.DisplayText(command, Color.Red);
            Com.Write("{\"gc\":\"" + command + "\"}");
            _readyEvent.Wait();
        }

        public bool IgnoreError { get; set; }

        public void InterpretLine(string line) {
            // This is called from SerialComm dataReceived, and runs in a separate thread than UI            
            MainForm.DisplayText(line, Color.Gray);

            if (line.Contains("SYSTEM READY")) {
                Close();
                MainForm.ShowMessageBox(
                    "TinyG Reset.",
                    "System Reset",
                    MessageBoxButtons.OK);
                MainForm.UpdateCncConnectionStatus();
                return;
            }

            if (line.StartsWith("{\"r\":{\"msg")) {
                line = line.Substring(13);
                int i = line.IndexOf('"');
                line = line.Substring(0, i);
                MainForm.ShowMessageBox(
                    "TinyG Message:",
                    line,
                    MessageBoxButtons.OK);
                return;
            }


            if (line.StartsWith("{\"er\":")) {
                if (line.Contains("File not open") && IgnoreError) {
                    MainForm.DisplayText("### Igored file not open error ###");
                    return;
                }
                // Close();
                MainForm.UpdateCncConnectionStatus();
                MainForm.ShowMessageBox(
                    "TinyG error. Review situation and restart if needed.",
                    "TinyG Error",
                    MessageBoxButtons.OK);
                return;
            }


            if (line.StartsWith("{\"r\":{}")) {
                // ack for g code command
                return;
            }

            /* Special homing handling is not needed in this firmware version
            if (Homing)
            {
                if (line.StartsWith("{\"sr\":"))
                {
                    // Status report
                    NewStatusReport(line);
				}

                if (line.Contains("\"home\":1"))
                {
                    _readyEvent.Set();
                    MainForm.DisplayText("ReadyEvent home");
                }
                return; 
            }
            */

            if (line.StartsWith("tinyg [mm] ok>")) {
                MainForm.DisplayText("ReadyEvent ok");
                _readyEvent.Set();
                return;
            }


            if (line.StartsWith("{\"sr\":")) {
                // Status report
                NewStatusReport(line);
                if (line.Contains("\"stat\":3")) {
                    MainForm.DisplayText("ReadyEvent stat");
                    _readyEvent.Set();
                }
                return;
            }

            if (line.StartsWith("{\"r\":{\"sr\"")) {
                // Status enquiry response, remove the wrapper:
                line = line.Substring(5);
                int i = line.IndexOf("}}");
                line = line.Substring(0, i + 2);
                NewStatusReport(line);
                _readyEvent.Set();
                MainForm.DisplayText("ReadyEvent r:sr");
                return;
            }

            if (line.StartsWith("{\"r\":{\"me") || line.StartsWith("{\"r\":{\"md")) {
                // response to motor power on/off commands
                _readyEvent.Set();
                return;
            }

            if (line.StartsWith("{\"r\":{\"sys\":")) {
                // response to reading settings for saving them
                // remove the wrapper:
                line = line.Substring(5);
                int i = line.IndexOf("}}");
                line = line.Substring(0, i + 2);
                Properties.Settings.Default.TinyG_sys = line;
                _readyEvent.Set();
                MainForm.DisplayText("ReadyEvent sys group");
                return;
            }

            if (line.StartsWith("{\"r\":{\"x\":")) {
                // response to reading settings for saving them
                // remove the wrapper:
                line = line.Substring(5);
                int i = line.IndexOf("}}");
                line = line.Substring(0, i + 2);
                Properties.Settings.Default.TinyG_x = line;
                _readyEvent.Set();
                MainForm.DisplayText("ReadyEvent x group");
                return;
            }

            if (line.StartsWith("{\"r\":{\"y\":")) {
                // response to reading settings for saving them
                // remove the wrapper:
                line = line.Substring(5);
                int i = line.IndexOf("}}");
                line = line.Substring(0, i + 2);
                Properties.Settings.Default.TinyG_y = line;
                _readyEvent.Set();
                MainForm.DisplayText("ReadyEvent y group");
                return;
            }

            if (line.StartsWith("{\"r\":{\"z\":")) {
                // response to reading settings for saving them
                // remove the wrapper:
                line = line.Substring(5);
                int i = line.IndexOf("}}");
                line = line.Substring(0, i + 2);
                Properties.Settings.Default.TinyG_z = line;
                _readyEvent.Set();
                MainForm.DisplayText("ReadyEvent z group");
                return;
            }

            if (line.StartsWith("{\"r\":{\"a\":")) {
                // response to reading settings for saving them
                // remove the wrapper:
                line = line.Substring(5);
                int i = line.IndexOf("}}");
                line = line.Substring(0, i + 2);
                Properties.Settings.Default.TinyG_a = line;
                _readyEvent.Set();
                MainForm.DisplayText("ReadyEvent a group");
                return;
            }

            if (line.StartsWith("{\"r\":{\"1\":")) {
                // response to reading settings for saving them
                // remove the wrapper:
                line = line.Substring(5);
                int i = line.IndexOf("}}");
                line = line.Substring(0, i + 2);
                Properties.Settings.Default.TinyG_m1 = line;
                _readyEvent.Set();
                MainForm.DisplayText("ReadyEvent m1 group");
                return;
            }

            if (line.StartsWith("{\"r\":{\"2\":")) {
                // response to reading settings for saving them
                // remove the wrapper:
                line = line.Substring(5);
                int i = line.IndexOf("}}");
                line = line.Substring(0, i + 2);
                Properties.Settings.Default.TinyG_m2 = line;
                _readyEvent.Set();
                MainForm.DisplayText("ReadyEvent m2 group");
                return;
            }

            if (line.StartsWith("{\"r\":{\"3\":")) {
                // response to reading settings for saving them
                // remove the wrapper:
                line = line.Substring(5);
                int i = line.IndexOf("}}");
                line = line.Substring(0, i + 2);
                Properties.Settings.Default.TinyG_m3 = line;
                _readyEvent.Set();
                MainForm.DisplayText("ReadyEvent m3 group");
                return;
            }

            if (line.StartsWith("{\"r\":{\"4\":")) {
                // response to reading settings for saving them
                // remove the wrapper:
                line = line.Substring(5);
                int i = line.IndexOf("}}");
                line = line.Substring(0, i + 2);
                Properties.Settings.Default.TinyG_m4 = line;
                _readyEvent.Set();
                MainForm.DisplayText("ReadyEvent m4 group");
                return;
            }

            if (line.StartsWith("{\"r\":")) {
                // response to setting a setting or reading motor settings for saving them
                // Replace {"1 with {"motor1 so that the field name is valid
                line = line.Replace("{\"1", "{\"motor1");
                line = line.Replace("{\"2", "{\"motor2");
                line = line.Replace("{\"3", "{\"motor3");
                line = line.Replace("{\"4", "{\"motor4");
                NewSetting(line);
                _readyEvent.Set();
                MainForm.DisplayText("<== r", Color.Green);
            }

        }  // end InterpretLine()

        // =================================================================================
        // TinyG JSON stuff
        // =================================================================================

        // =================================================================================
        // Status report

        public StatusReport Status;
        private JavaScriptSerializer serializer = new JavaScriptSerializer();
        public void NewStatusReport(string line) {
            Status = serializer.Deserialize<StatusReport>(line);

        }





        // =================================================================================
        // Settings

        public Response Settings;
        public void NewSetting(string line) {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Settings = serializer.Deserialize<Response>(line);
        }

        public class Resp {

            // =========================================================
            // The individual settings we care about and do something
            // when they change.

            // *jm: jerk max
            private string _xjm = "";
            public string xjm {
                get { return _xjm; }
                set {
                    _xjm = value;
                    MainForm.ValueUpdater("xjm", _xjm);
                }
            }

            private string _yjm = "";
            public string yjm {
                get { return _yjm; }
                set {
                    _yjm = value;
                    MainForm.ValueUpdater("yjm", _yjm);
                }
            }

            private string _zjm = "";
            public string zjm {
                get { return _zjm; }
                set {
                    _zjm = value;
                    MainForm.ValueUpdater("zjm", _zjm);
                }
            }

            private string _ajm = "";
            public string ajm {
                get { return _ajm; }
                set {
                    _ajm = value;
                    MainForm.ValueUpdater("ajm", _ajm);
                }
            }

            // *vm: velocity max
            private string _xvm = "";
            public string xvm {
                get { return _xvm; }
                set {
                    _xvm = value;
                    MainForm.ValueUpdater("xvm", _xvm);
                }
            }

            private string _yvm = "";
            public string yvm {
                get { return _yvm; }
                set {
                    _yvm = value;
                    MainForm.ValueUpdater("yvm", _yvm);
                }
            }

            private string _zvm = "";
            public string zvm {
                get { return _zvm; }
                set {
                    _zvm = value;
                    MainForm.ValueUpdater("zvm", _zvm);
                }
            }

            private string _avm = "";
            public string avm {
                get { return _avm; }
                set {
                    _avm = value;
                    MainForm.ValueUpdater("avm", _avm);
                }
            }

            // *mi: motor microsteps 
            // Note, that InterpretLine() replaces "1" with "motor1" so we can use valid names
            private string _motor1mi = "";
            public string motor1mi {
                get { return _motor1mi; }
                set {
                    _motor1mi = value;
                    MainForm.ValueUpdater("1mi", _motor1mi);
                }
            }

            private string _motor2mi = "";
            public string motor2mi {
                get { return _motor2mi; }
                set {
                    _motor2mi = value;
                    MainForm.ValueUpdater("2mi", _motor2mi);
                }
            }

            private string _motor3mi = "";
            public string motor3mi {
                get { return _motor3mi; }
                set {
                    _motor3mi = value;
                    MainForm.ValueUpdater("3mi", _motor3mi);
                }
            }

            private string _motor4mi = "";
            public string motor4mi {
                get { return _motor4mi; }
                set {
                    _motor4mi = value;
                    MainForm.ValueUpdater("4mi", _motor4mi);
                }
            }

            // *tr: motor travel per rev. 
            private string _motor1tr = "";
            public string motor1tr {
                get { return _motor1tr; }
                set {
                    _motor1tr = value;
                    MainForm.ValueUpdater("1tr", _motor1tr);
                }
            }

            private string _motor2tr = "";
            public string motor2tr {
                get { return _motor2tr; }
                set {
                    _motor2tr = value;
                    MainForm.ValueUpdater("2tr", _motor2tr);
                }
            }

            private string _motor3tr = "";
            public string motor3tr {
                get { return _motor3tr; }
                set {
                    _motor3tr = value;
                    MainForm.ValueUpdater("3tr", _motor3tr);
                }
            }

            private string _motor4tr = "";
            public string motor4tr {
                get { return _motor4tr; }
                set {
                    _motor4tr = value;
                    MainForm.ValueUpdater("4tr", _motor4tr);
                }
            }

            // *sa: motor step angle 
            private string _motor1sa = "";
            public string motor1sa {
                get { return _motor1sa; }
                set {
                    _motor1sa = value;
                    MainForm.ValueUpdater("1sa", _motor1sa);
                }
            }

            private string _motor2sa = "";
            public string motor2sa {
                get { return _motor2sa; }
                set {
                    _motor2sa = value;
                    MainForm.ValueUpdater("2sa", _motor2sa);
                }
            }

            private string _motor3sa = "";
            public string motor3sa {
                get { return _motor3sa; }
                set {
                    _motor3sa = value;
                    MainForm.ValueUpdater("3sa", _motor3sa);
                }
            }

            private string _motor4sa = "";
            public string motor4sa {
                get { return _motor4sa; }
                set {
                    _motor4sa = value;
                    MainForm.ValueUpdater("4sa", _motor4sa);
                }
            }

            private string _xjh = "";
            public string xjh {
                get { return _xjh; }
                set {
                    _xjh = value;
                    MainForm.ValueUpdater("xjh", _xjh);
                }
            }

            private string _yjh = "";
            public string yjh {
                get { return _yjh; }
                set {
                    _yjh = value;
                    MainForm.ValueUpdater("yjh", _yjh);
                }
            }

            private string _zjh = "";
            public string zjh {
                get { return _zjh; }
                set {
                    _zjh = value;
                    MainForm.ValueUpdater("zjh", _zjh);
                }
            }

            private string _xsv = "";
            public string xsv {
                get { return _xsv; }
                set {
                    _xsv = value;
                    MainForm.ValueUpdater("xsv", _xsv);
                }
            }

            private string _ysv = "";
            public string ysv {
                get { return _ysv; }
                set {
                    _ysv = value;
                    MainForm.ValueUpdater("ysv", _ysv);
                }
            }

            private string _zsv = "";
            public string zsv {
                get { return _zsv; }
                set {
                    _zsv = value;
                    MainForm.ValueUpdater("zsv", _zsv);
                }
            }

            private string _xsn = "";
            public string xsn {
                get { return _xsn; }
                set {
                    _xsn = value;
                    MainForm.ValueUpdater("xsn", _xsn);
                }
            }

            private string _ysn = "";
            public string ysn {
                get { return _ysn; }
                set {
                    _ysn = value;
                    MainForm.ValueUpdater("ysn", _ysn);
                }
            }

            private string _zsn = "";
            public string zsn {
                get { return _zsn; }
                set {
                    _zsn = value;
                    MainForm.ValueUpdater("zsn", _zsn);
                }
            }

            private string _xsx = "";
            public string xsx {
                get { return _xsx; }
                set {
                    _xsx = value;
                    MainForm.ValueUpdater("xsx", _xsx);
                }
            }

            private string _ysx = "";
            public string ysx {
                get { return _ysx; }
                set {
                    _ysx = value;
                    MainForm.ValueUpdater("ysx", _ysx);
                }
            }

            private string _zsx = "";
            public string zsx {
                get { return _zsx; }
                set {
                    _zsx = value;
                    MainForm.ValueUpdater("zsx", _zsx);
                }
            }

        }   // end class Resp

        public class Response {
            public Resp r { get; set; }
            public List<int> f { get; set; }
        }


        // ADDITIONAL COMMANDS 
        public bool Zdown() {
            ZGuardOff();
            return CNC_Z_m(Properties.Settings.Default.General_ZtoPCB);
        }


        public bool Zup() {
            ZGuardOn();
            return CNC_Z_m(0);
        }



        public double _z_offset;  // this is how far from zero the z-head should be to speed-up movements
        public double z_offset {
            get { return _z_offset; }
            set {
                if (value < 0) value = 0;
                if (value > 20) {
                    ShowSimpleMessageBox("Attempted to set z_offset > 20mm - too dangerous, setting to 20mm");
                    value = 20;
                }
                // adjust where we are if a new value was entered and we were at the old position
                if (_z_offset != value && CurrentZ == _z_offset) CNC_Z_m(value);
                _z_offset = value;
            }
        }



        public void CNC_Park() {
            Zup();
            CNC_XY_m(Global.GeneralParkLocation);
        }

        public bool CNC_Home_m(string axis) {
            if (!CNC_Write_m("{\"gc\":\"G28.2 " + axis + "0\"}", 10000)) {
                ShowSimpleMessageBox("Homing operation mechanical step failed, CNC issue");
                return false;
            }
            Global.Instance.DisplayText("Homing " + axis + " done.", Color.DarkSeaGreen);
            return true;
        }

        // =================================================================================
        // CNC_Write_m
        // Sends a command to CNC, doesn't return until the response is handled
        // by the CNC class. (See _readyEvent )
        // =================================================================================
        private const int CNC_MoveTimeout = 3000; // timeout for X,Y,Z,A movements; 2x ms. (3000= 6s timeout)

        public void CNC_RawWrite(string s) {
            // This for operations that cause conflicts with event firings. Caller does waiting, if needed.
            RawWrite(s);
        }

        bool CNC_BlockingWriteDone;
        
        private void CNC_BlockingWrite_thread(string cmd) {
            _readyEvent.Reset();
            Com.Write(cmd);
            _readyEvent.Wait();
            CNC_BlockingWriteDone = true;
        }

        public bool CNC_Write_m(string s, int Timeout = 250) {
            if (!Com.IsOpen) {
                MainForm.DisplayText("** PORT CLOSED ** Ignoring command " + s, Color.Red);
                return false;
            }
            CNC_BlockingWriteDone = false;
            Thread t = new Thread(() => CNC_BlockingWrite_thread(s));
            t.IsBackground = true;
            t.Start();

            int i = 0;
            while (!CNC_BlockingWriteDone) {
                Thread.Sleep(2);
                Application.DoEvents();
                i++;
                if (i > Timeout) {
                    _readyEvent.Set();  // terminates the CNC_BlockingWrite_thread
                    Global.Instance.mainForm.ShowMessageBox(
                        "Debug: CNC_BlockingWrite: Timeout on command " + s,
                        "Timeout",
                        MessageBoxButtons.OK);
                    CNC_BlockingWriteDone = true;
                    JoggingBusy = false;
                    return false;
                }
            }
            return true;
        }

        private bool CNC_MoveIsSafe_m(PartLocation p) {
            if ((p.X < -3.0) || (p.X > Properties.Settings.Default.General_MachineSizeX) || (p.Y < -3.0) || (p.Y > Properties.Settings.Default.General_MachineSizeY)) {
                ShowSimpleMessageBox("Attempt to move outside safe limits " + p);
                return false;
            }
            if (CNC_NeedleIsDown_m()) {
                ZGuardOn();
                CNC_Z_m(0);
            }
            return true;
        }



        private bool _Zguard = true;
        public void ZGuardOn() {
            _Zguard = true;
        }
        public void ZGuardOff() {
            _Zguard = false;
        }

        private bool CNC_NeedleIsDown_m() {
            if ((CurrentZ > z_offset + 1) && _Zguard) {
                Global.Instance.DisplayText("Needle down error.");
                /* ShowMessageBox(
                    "Attempt to move while needle is down.",
                    "Danger to Needle",
                    MessageBoxButtons.OK);*/
                return true;
            }
            return false;
        }

        private void CNC_BlockingXY_thread(double X, double Y) {
            _readyEvent.Reset();
            XY(X, Y);
            _readyEvent.Wait();
            CNC_BlockingWriteDone = true;
        }

        private void CNC_BlockingXYA_thread(double X, double Y, double A) {
            _readyEvent.Reset();
            XYA(X, Y, A);
            _readyEvent.Wait();
            CNC_BlockingWriteDone = true;
        }

        public bool CNC_XYA_m(double X, double Y, double A) { return CNC_XY_m(new PartLocation(X, Y, A), true); }
        public bool CNC_XY_m(double X, double Y) { return CNC_XY_m(new PartLocation(X, Y), false); }
        public bool CNC_XY_m(PartLocation loc) { return CNC_XY_m(loc, false); }
        public bool CNC_XYA_m(PartLocation loc) { return CNC_XY_m(loc, true); }

        public bool CNC_XY_m(PartLocation loc, bool MoveAngle) {
            if (MoveAngle) Global.Instance.DisplayText("CNC_XYA_m, x: " + loc);
            else Global.Instance.DisplayText("CNC_XY_m, x: " + loc);

            if (AbortPlacement) {
                AbortPlacement = false;  // one shot
                ShowSimpleMessageBox("Operation aborted");
                return false;
            }

            if (!CNC_MoveIsSafe_m(loc)) return false;

            if (!Connected) {
                ShowSimpleMessageBox("CNC_XY: Cnc not connected");
                return false;
            }

            CNC_BlockingWriteDone = false;
            Thread t;
            if (MoveAngle) {
                t = new Thread(() => CNC_BlockingXYA_thread(loc.X, loc.Y, loc.A));
            } else {
                t = new Thread(() => CNC_BlockingXY_thread(loc.X, loc.Y));
            }
            t.IsBackground = true;
            t.Start();
            int i = 0;

            while (!CNC_BlockingWriteDone) {
                Thread.Sleep(2);
                Application.DoEvents();
                i++;
                if (i > CNC_MoveTimeout) {
                    _readyEvent.Set();   // causes CNC_Blocking_thread to exit
                }
            }

            CNC_BlockingWriteDone = true;
            if ((i > CNC_MoveTimeout) && Connected) {
                Global.Instance.mainForm.ShowMessageBox(
                           "CNC_XY: Timeout / Cnc connection cut!",
                           "Timeout",
                           MessageBoxButtons.OK);
                Close();
                Global.Instance.mainForm.UpdateCncConnectionStatus();
            }
            Global.Instance.DisplayText("CNC_XY_m ok");
            return (Connected);
        }


        private void CNC_BlockingZ_thread(double z) {
            _readyEvent.Reset();
            Z(z);
            _readyEvent.Wait();
            CNC_BlockingWriteDone = true;
        }

        public bool CNC_Z_m(double Z) {
            if (Z == 0) Z = z_offset; //consider this height = zero

            if (AbortPlacement) {
                AbortPlacement = false;  // one shot
                ShowSimpleMessageBox("Operation aborted");
                return false;
            }

            if (!Connected) {
                ShowSimpleMessageBox("CNC_XY: Cnc not connected");
                return false;
            }

            CNC_BlockingWriteDone = false;
            Thread t = new Thread(() => CNC_BlockingZ_thread(Z));
            t.IsBackground = true;
            t.Start();
            int i = 0;
            while (!CNC_BlockingWriteDone) {
                Thread.Sleep(2);
                Application.DoEvents();
                i++;
                if (i > CNC_MoveTimeout) {
                    _readyEvent.Set();   // causes CNC_Blocking_thread to exit
                }
            }
            if ((i > CNC_MoveTimeout) || !Connected) {
                ShowSimpleMessageBox("CNC_Z: Timeout / Cnc connection cut!");
                Close();
            }
            return (Connected);
        }

        private void CNC_BlockingA_thread(double a) {
            if (Properties.Settings.Default.CNC_SlackCompensation) {
                _readyEvent.Reset();
                A(a - 10); //this is slack compensation for the angle
                _readyEvent.Wait();
            }
            _readyEvent.Reset();
            A(a);
            _readyEvent.Wait();
            CNC_BlockingWriteDone = true;
        }

        public bool CNC_A_m(double A) {
            CNC_BlockingWriteDone = false;
            Thread t = new Thread(() => CNC_BlockingA_thread(A));
            t.IsBackground = true;
            t.Start();
            int i = 0;
            if (!Connected) {
                Global.Instance.mainForm.ShowMessageBox(
                    "CNC_A: Cnc not connected",
                    "Cnc not connected",
                    MessageBoxButtons.OK);
                return false;
            }
            while (!CNC_BlockingWriteDone) {
                Thread.Sleep(2);
                Application.DoEvents();
                i++;
                if (i > CNC_MoveTimeout) {
                    _readyEvent.Set();   // causes CNC_Blocking_thread to exit
                }
            }

            CNC_BlockingWriteDone = true;
            if ((i > CNC_MoveTimeout) && Connected) {
                Global.Instance.mainForm.ShowMessageBox(
                           "CNC_A: Timeout / Cnc connection cut!",
                           "Timeout",
                           MessageBoxButtons.OK);
                Close();
                Global.Instance.mainForm.UpdateCncConnectionStatus();
            }
            return (Connected);
        }


    }  // end Class CNC


}



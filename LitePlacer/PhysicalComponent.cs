using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;


namespace LitePlacer {
    public class PhysicalComponent {
        public PartLocation nominal = new PartLocation();
        public PartLocation machine = new PartLocation();

        public bool IsFiducial {
            get {
                Regex regex = new Regex(Properties.Settings.Default.fiducial_designator_regexp, RegexOptions.IgnoreCase);
                return (regex.Match(Designator).Success);
            }
        }    

        public string Designator { get; set; }
        public string Footprint { get; set; }
        public double X_nominal { get { return nominal.X; } set { nominal.X = value; } }
        public double Y_nominal { get { return nominal.Y; } set { nominal.Y = value; } }
        public double Rotation { get { return nominal.A; } set { nominal.A = value; } }
        public double Rotation_machine { get { return machine.A; } set { machine.A = value; } }
        public double X_machine { get { return machine.X; } set { machine.X = value; } }
        public double Y_machine { get { return machine.Y; } set { machine.Y = value; } }
        public string Method { get; set; }
        public string MethodParameter { get; set; }
        private DataGridViewRow myRow;

        public PhysicalComponent() {
            nominal.physicalComponent = this;
            machine.physicalComponent = this;
        }

        public PhysicalComponent(DataGridViewRow row) {
            if (row == null) throw new Exception("Physical component intialized with null row");
            myRow = row;
            Parse();
        }

        private double myParse(string str) {
            if (str.Equals("Nan")) return double.NaN;
            return double.Parse(str);
        }

        // this should also check for null entries and throw exceoptins XXX TODO
        public void Parse() {
            try {
                var c = myRow.Cells;
                X_nominal = myParse(c["X_nominal"].Value.ToString());
                Y_nominal = myParse(c["Y_nominal"].Value.ToString());
                X_machine = myParse(c["X_machine"].Value.ToString());
                Y_machine = myParse(c["Y_machine"].Value.ToString());
                Rotation = myParse(c["Rotation"].Value.ToString());
                Rotation_machine = myParse(c["Rotation_machine"].Value.ToString());
                Designator = c["Component"].Value.ToString();
                Footprint = c["Value_Footprint"].Value.ToString();
            } 
            catch (Exception e) {
                throw new Exception("Unable to parse row :" + e.ToString());
            }
        }

        public void UpdateRow() {
            try {
                var c = myRow.Cells;
                c["X_nominal"].Value = X_nominal.ToString("F4");
                c["Y_nominal"].Value = Y_nominal.ToString("F4");
                c["X_machine"].Value = X_machine.ToString("F4");
                c["Y_machine"].Value = Y_machine.ToString("F4");
                c["Rotation"].Value = Rotation.ToString("F2");
                c["Rotation_machine"].Value = Rotation_machine.ToString("F2");
            } catch (Exception e) {
                throw new Exception("Unable to update row :" + e);
            }

        }
    }

    public class JobEntry {
        DataGridViewRow myRow;
        public bool IsFiducial {
            get {
                Regex regex = new Regex(Properties.Settings.Default.fiducial_designator_regexp, RegexOptions.IgnoreCase);
                return (regex.Match(ComponentList).Success);
            }
        }    
        public string ComponentCount;
        public string ComponentType;
        public string GroupMethod;
        public string MethodParamAllComponents;
        public string ComponentList;
        public string[] Components { get { return ComponentList.Split(','); } }
        public JobEntry(DataGridViewRow row) {
            if (row == null) throw new Exception("JobEntry tried to be initalized from null row");
            myRow = row;
            Parse();
        }

        public void Parse() {
            try {
                var c = myRow.Cells;
                ComponentCount = c["ComponentCount"].Value.ToString();
                ComponentType = c["ComponentType"].Value.ToString();
                GroupMethod = c["GroupMethod"].Value.ToString();
                MethodParamAllComponents = c["MethodParamAllComponents"].Value.ToString();
                ComponentList = c["ComponentList"].Value.ToString();
            } catch (Exception e) {
                throw new Exception("Unalbe to parse job row :" + e);
            }
        }

        public void UpdateRow() {
            var c = myRow.Cells;
            c["ComponentCount"].Value = ComponentCount;
            c["ComponentType"].Value = ComponentType;
            c["GroupMethod"].Value = GroupMethod;
            c["MethodParamAllComponents"].Value = MethodParamAllComponents;
            c["ComponentList"].Value = ComponentList;
        }
    }
}

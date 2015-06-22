using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using LitePlacer.Properties;

namespace LitePlacer {
    [Serializable]
    public class PhysicalComponent : INotifyPropertyChanged {
        [XmlIgnore]
        public PartLocation nominal = new PartLocation();
        [XmlIgnore]
        public PartLocation machine = new PartLocation();
        [XmlIgnore]
        public JobData JobData = null;

        public bool IsFiducial {
            get {
                Regex regex = new Regex(Settings.Default.fiducial_designator_regexp, RegexOptions.IgnoreCase);
                return (regex.Match(Designator).Success);
            }
        }

        private string _designator="", _footprint="";
        private string _method = "";
        private string _methodParams = "";

        public string Designator {
            get { return _designator; } 
            set {_designator=value; Notify("Designator");} 
        }
        public string Footprint {
            get { return _footprint; }
            set { _footprint = value; Notify("Footprint"); }
        }
        public double X_nominal { 
            get { return nominal.X; }
            set { nominal.X = value; Notify("X_nominal"); } 
        }
        public double Y_nominal { 
            get { return nominal.Y; }
            set { nominal.Y = value; Notify("Y_nominal"); } 
        }
        public double Rotation { 
            get { return nominal.A; }
            set { nominal.A = value; Notify("Rotation"); } 
        }
        public double Rotation_machine { 
            get { return machine.A; }
            set { machine.A = value; Notify("Rotation_machine"); } 
        }
        public double X_machine { 
            get { return machine.X; }
            set { machine.X = value; Notify("X_machine"); } 
        }
        public double Y_machine { 
            get { return machine.Y; }
            set { machine.Y = value; Notify("Y_machine"); } 
        }

        public string Method {
            get { return (JobData != null) ? JobData.Method : _method; }
            set { if (JobData != null) JobData.Method = value; 
                  else _method = value;
                  Notify("Method");
            }
        }
        public string MethodParameters {
            get { return (JobData != null) ? JobData.MethodParameters : _methodParams; }
            set { if (JobData != null) JobData.MethodParameters = value; 
                  else _methodParams = value;
                  Notify("MethodParameters");
            }
        }


        public bool IsFirstInRow {
            get {
                if (JobData != null) {
                    var list = JobData.GetComponents();
                    return (list[0].Equals(this));
                }
                return true;
            }
        }
       
               
        public PhysicalComponent() {
           nominal.physicalComponent = this;
           machine.physicalComponent = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string name) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

    }

}

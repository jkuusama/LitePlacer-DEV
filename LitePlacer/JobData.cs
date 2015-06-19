using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace LitePlacer {
    [Serializable]
    public class JobData : INotifyPropertyChanged {

        private List<PhysicalComponent> components;

        public JobData() {
            components = new List<PhysicalComponent>();
        }
        
       
        public int Count { get { return components.Count; } }

        
        public string ComponentType {
            get { if (components.Count > 0) return components[0].Footprint; return ""; }
        }

        private string _method;
        public string Method { 
            get { return (IsFidducial) ? "Fiducial" : _method; }
            set { _method = value; notify("Method"); } 
        }

        private string _MethodParameters;
        public string MethodParameters {
            get { return _MethodParameters; }
            set { _MethodParameters = value; notify("MethodParameters"); }
        }


        public string ComponentList {
            get { return string.Join(",", components.Select(x => x.Designator).ToArray()); }
            // XXX - need to add setter to be able to dynamically adjust the components list
        }

        public bool IsFidducial {
            get {
                if (components.Count > 0) return components[0].IsFiducial;
                return false;
            }
        }

        private void UpdateComponents() {
            foreach (var x in components) {
                x.JobData = this;
            }
            notify("ComponentList");
            notify("Count");
        }

        public void RemoveComponent(string designator) {
            components.RemoveAll(x => x.Designator.Equals(designator));
            UpdateComponents();
        }

        public void AddComponent(PhysicalComponent x) { components.Add(x); UpdateComponents();}
        public void AddComponent(PhysicalComponent[] x) { components.AddRange(x); UpdateComponents(); }
        public PhysicalComponent[] GetComponents() {return components.ToArray(); }

        public override string ToString() {
            var r = string.Join("\",\"", new List<string> {Count.ToString(), ComponentType, Method, _MethodParameters, ComponentList});
            return "\""+r+"\"";            
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void notify(string name) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}

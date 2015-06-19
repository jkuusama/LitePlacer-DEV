using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace LitePlacer {
    public enum AForgeMethod {
        Grayscale,
        ContrastStretch,
        KillColor,
        KeepColor,
        Invert,
        Zoom,
        EdgeDetect1,
        EdgeDetect2,
        EdgeDetect3,
        EdgeDetect4,
        NoiseReduction1,
        NoiseReduciton2,
        NoiseReduction3,
        Threshold,
        Histogram
    };

    public class AForgeFunctionSet {
        private List<BindingList<AForgeFunction>> list = new List<BindingList<AForgeFunction>>();
        private List<string> names = new List<string>();
        private string dir = AppDomain.CurrentDomain.BaseDirectory;
        private string type;

        public AForgeFunctionSet(string type) {
            this.type = type;
            LoadAll();
            AddMissing();
        }

        public void AddMissing() {
            //list.Clear();
            //names.Clear();
            string[] types = new string[0];

            //default types
            if (type.Equals("UP")) {
                types = new[] {"Needle","Components"};            
            }
            if (type.Equals("DOWN")) {
                types = new[] { "Homing", "Fiducials", "Components", "PaperTape", "ClearPlasticTape", "BlackPlasticTape"};
            }

            foreach (var t in types) {
                if (names.Contains(t)) continue; //skip names that were loaded
                list.Add(new BindingList<AForgeFunction>());
                names.Add(t);
            }
        }

        /// <summary>
        /// Returns the tape types derived from the filters that end in 'Tape' adding in the default types if not saved
        /// </summary>
        /// <returns></returns>
        public static List<string> GetTapeTypes() {
            string[] filenames = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*Tape.aforgeDOWN");
            var types = filenames.Select(x => Path.GetFileNameWithoutExtension(x).Replace("Tape", "")).ToList();
            List<string> defaultTypes = new List<string> { "Paper", "ClearPlastic", "BlackPlastic" };
            return defaultTypes.Union(types).ToList();
        }

        public static BindingList<AForgeFunction> GetFunctionsFromDisk(string name) {
            var filename = AppDomain.CurrentDomain.BaseDirectory + @"\" + name + ".aforge" + "UP";
            if (File.Exists(filename)) return AForgeFunction.LoadList(filename);
            filename = AppDomain.CurrentDomain.BaseDirectory + @"\" + name + ".aforge" + "DOWN";
            if (File.Exists(filename)) return AForgeFunction.LoadList(filename);
            return null;
        }


        public static BindingList<AForgeFunction> GetFunctionsFromDisk(string name, bool UpCamera) {
            var filename = AppDomain.CurrentDomain.BaseDirectory + @"\" + name + ".aforge" + ((UpCamera) ? "UP" : "DOWN");
            return AForgeFunction.LoadList(filename);
        }


        public void LoadAll() {
            list.Clear();
            names.Clear();
            string[] files = Directory.GetFiles(dir, "*.aforge"+type);
            foreach (var file in files) {
                list.Add(AForgeFunction.LoadList(file));
                names.Add(Path.GetFileNameWithoutExtension(file));
            }
        }

        public void SaveAll() {
            if (list.Count != names.Count) throw new Exception("FunctionSet missmatch");
            for (int i=0; i<list.Count; i++) {
                if (names[i].Length == 0) continue;
                AForgeFunction.SaveList(list[i], dir+ @"\" + names[i] + ".aforge"+type);
            }
        }

        public BindingList<AForgeFunction> GetSet(string name) {
            for (int i=0; i<names.Count; i++) 
                if (names[i].Equals( name )) return list[i];
            return null;
        }


        public string[] GetNames() {
            return names.ToArray();
        }

        public void Save(string name, BindingList<AForgeFunction> l) {
            int i;
            for (i = 0; i < names.Count; i++) {
                if (names[i].Equals(name)) {
                    list[i] = l;
                    break;
                }
            }
            if (i == names.Count) {
                //add it because it doesn't exist
                names.Add(name);
                list.Add(l);
            }
            // save it
            AForgeFunction.SaveList(l, dir + @"\" + names[i] + ".aforge"+type);
        }
            



       
    }


    public class AForgeFunction : INotifyPropertyChanged {
        public AForgeMethod _method = AForgeMethod.Threshold;
        public bool _enabled;
        public int _parameter_int, _r, _g, _b;
        public double _parameter_double;


        public AForgeMethod Method {
            get { return _method; }
            set { _method = value; }
        }
        public bool Enabled {
            get { return _enabled; }
            set { _enabled = value; notify("Enabled"); Console.WriteLine("new val = " + _enabled); }
        }
       /* public int parameter_int {
            get { return _parameter_int; }
            set { _parameter_int = value; notify("parameter_int"); }
        }*/
        public double parameter_double {
            get { return _parameter_double; }
            set { _parameter_double = value; notify("parameter_double"); }
        }
        public int R {
            get { return _r; }
            set { _r = value; notify("R"); }
        }
        public int G {
            get { return _g; }
            set { _g = value; notify("G"); }
        }
        public int B {
            get { return _b; }
            set { _b = value; notify("B"); }
        }

        public AForgeFunction Clone() {
            return new AForgeFunction( ToString());
        }


        public AForgeFunction() { }
        public AForgeFunction(string loadString) {
            try {
                string[] x = loadString.Split(',');
                _method = (AForgeMethod)Enum.Parse(typeof(AForgeMethod), x[0]);
              //  parameter_int = int.Parse(x[1]);
                parameter_double = double.Parse(x[1]);
                R = int.Parse(x[2]);
                G = int.Parse(x[3]);
                B = int.Parse(x[4]);
            } catch (Exception e) {
                throw new Exception("Unable to parse string : " + e);
            }
        }

        public override string ToString() {
            return String.Format("{0},{1},{2},{3},{4}",
               // _method.ToString(), parameter_int, parameter_double, R, G, B);
               _method, parameter_double, R, G, B);
        }

        public static void SaveList(BindingList<AForgeFunction> list, string filename) {
            File.WriteAllLines(filename, list.Select(x => x.ToString()).ToArray());
        }

        public static BindingList<AForgeFunction> LoadList(string filename) {
            BindingList<AForgeFunction> list = new BindingList<AForgeFunction>();
            foreach (var x in File.ReadAllLines(filename)) {
                list.Add( new AForgeFunction(x) );
            }
            return list;
        }

        public static BindingList<AForgeFunction> Clone(BindingList<AForgeFunction> list) {
            var l = new BindingList<AForgeFunction>();
            foreach (var x in list) {
                l.Add(x.Clone());
            }
            return l;
        }



        public event PropertyChangedEventHandler PropertyChanged;
        private void notify(string name) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

    }
}

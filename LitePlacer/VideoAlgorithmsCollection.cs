using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// Terminology:
// AForgeFunction: single video processing function, such as "invert"
// Algorithm: Named List of AForgeFunctions, with measurement parameters, such as "fiducials"

namespace LitePlacer
{
    public class VideoAlgorithmsCollection
    {
        // Full algorithm: Name, list of functions and collection of measurement parameters
        public class FullAlgorithmDescription
        {
            public string Name = "unitialized!";
            public List<AForgeFunctionDefinition> FunctionList = new List<AForgeFunctionDefinition>();
            public MeasurementParametersClass MeasurementParameters = new MeasurementParametersClass();
        }

        // All algorithms: List of algorithms that the user has set up
        public List<FullAlgorithmDescription> AllAlgorithms = new List<FullAlgorithmDescription>();

        // CurrentAlgorithm: The algorithm that is currently shown in UI
        public FullAlgorithmDescription CurrentAlgorithm;

        // CurrentFunction: Index to CurrentAlgorithm.FunctionList indicating which function 
        // is currently being edited in UI
        private int CurrFunctIndex = -1;
        public int CurrentFunctionIndex
        {
            get
            {
                return CurrFunctIndex;
            }
            set
            {
                if (CurrentAlgorithm == null)
                {
                    return;
                }
                CurrFunctIndex = value;
            }
        }

        public bool AlgorithmExists(string AlgorithmName)
        {
            foreach (FullAlgorithmDescription Algorithm in AllAlgorithms)
            {
                if (Algorithm.Name == AlgorithmName)
                {
                    return true;
                }
            }
            return false;
        }

        public bool FindAlgorithm(string AlgorithmName, out FullAlgorithmDescription Result)
        {
            Result = new FullAlgorithmDescription();
            foreach (FullAlgorithmDescription Algorithm in AllAlgorithms)
            {
                if (Algorithm.Name == AlgorithmName)
                {
                    Result = Algorithm;
                    return true;
                }
            }
            // not found, return empty algorithm:
            Result.FunctionList.Clear();
            Result.MeasurementParameters = new MeasurementParametersClass();
            return false;
        }

        public bool SelectedAlgorithmChanged(string AlgorithmName)
        {
            return FindAlgorithm(AlgorithmName, out CurrentAlgorithm);
        }


        public void CurrentFunction_NewInt(int val)
        {
            if (CurrentAlgorithm != null)
            {
                if (CurrentAlgorithm.FunctionList.Count != 0)
                {
                    if ((CurrentFunctionIndex >= 0) && ((CurrentFunctionIndex < CurrentAlgorithm.FunctionList.Count)))
                    {
                        CurrentAlgorithm.FunctionList[CurrentFunctionIndex].parameterInt = val;
                    }
                }
            }
        }

        public void CurrentFunction_NewDouble(double val)
        {
            if (CurrentAlgorithm != null)
            {
                if (CurrentAlgorithm.FunctionList.Count != 0)
                {
                    if ((CurrentFunctionIndex >= 0) && ((CurrentFunctionIndex < CurrentAlgorithm.FunctionList.Count)))
                    {
                        CurrentAlgorithm.FunctionList[CurrentFunctionIndex].parameterDouble = val;
                    }
                }
            }
        }

        public void CurrentFunction_NewDoubleParA(double val)
        {
            if (CurrentAlgorithm != null)
            {
                if (CurrentAlgorithm.FunctionList.Count != 0)
                {
                    if ((CurrentFunctionIndex >= 0) && ((CurrentFunctionIndex < CurrentAlgorithm.FunctionList.Count)))
                    {
                        CurrentAlgorithm.FunctionList[CurrentFunctionIndex].parameterDoubleA = val;
                    }
                }
            }
        }

        public void CurrentFunction_NewDoubleParB(double val)
        {
            if (CurrentAlgorithm != null)
            {
                if (CurrentAlgorithm.FunctionList.Count != 0)
                {
                    if ((CurrentFunctionIndex >= 0) && ((CurrentFunctionIndex < CurrentAlgorithm.FunctionList.Count)))
                    {
                        CurrentAlgorithm.FunctionList[CurrentFunctionIndex].parameterDoubleB = val;
                    }
                }
            }
        }

        public void CurrentFunction_NewDoubleParC(double val)
        {
            if (CurrentAlgorithm != null)
            {
                if (CurrentAlgorithm.FunctionList.Count != 0)
                {
                    if ((CurrentFunctionIndex >= 0) && ((CurrentFunctionIndex < CurrentAlgorithm.FunctionList.Count)))
                    {
                        CurrentAlgorithm.FunctionList[CurrentFunctionIndex].parameterDoubleC = val;
                    }
                }
            }
        }

        public void CurrentFunction_NewR(int val)
        {
            if (CurrentAlgorithm != null)
            {
                if (CurrentAlgorithm.FunctionList.Count != 0)
                {
                    if ((CurrentFunctionIndex >= 0) && ((CurrentFunctionIndex < CurrentAlgorithm.FunctionList.Count)))
                    {
                        CurrentAlgorithm.FunctionList[CurrentFunctionIndex].R = val;
                    }
                }
            }
        }


        public void CurrentFunction_NewG(int val)
        {
            if (CurrentAlgorithm != null)
            {
                if (CurrentAlgorithm.FunctionList.Count != 0)
                {
                    if ((CurrentFunctionIndex >= 0) && ((CurrentFunctionIndex < CurrentAlgorithm.FunctionList.Count)))
                    {
                        CurrentAlgorithm.FunctionList[CurrentFunctionIndex].G = val;
                    }
                }
            }
        }


        public void CurrentFunction_NewB(int val)
        {
            if (CurrentAlgorithm != null)
            {
                if (CurrentAlgorithm.FunctionList.Count != 0)
                {
                    if ((CurrentFunctionIndex >= 0) && ((CurrentFunctionIndex < CurrentAlgorithm.FunctionList.Count)))
                    {
                        CurrentAlgorithm.FunctionList[CurrentFunctionIndex].B = val;
                    }
                }
            }
        }


    }

    public class AForgeFunctionDefinition
    {
        public string Name = "Not set!";
        public bool Active = false;
        public int parameterInt = 0;           // general parameters. Some functions take one int,
        public double parameterDouble = 0.0;   // some take a float,
        public int R = 0;                       // and some need R, B, G values.
        public int G = 0;                       // Some need many of these.
        public int B = 0;
        public double parameterDoubleA = 0.0;
        public double parameterDoubleB = 0.0;
        public double parameterDoubleC = 0.0;
    }

    public class MeasurementParametersClass
    {
        public bool SearchRounds = false;
        public bool SearchRectangles = false;
        public bool SearchComponentOutlines = false;
        public bool SearchComponentPads = false;
        public double Xmin = 0.0;
        public double Xmax = 0.0;
        public double Ymin = 0.0;
        public double Ymax = 0.0;
        public double XUniqueDistance = 0.0;
        public double YUniqueDistance = 0.0;
    }

}

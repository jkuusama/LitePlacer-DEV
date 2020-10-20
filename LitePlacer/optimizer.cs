using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Optimization;
using System;
using System.Collections.Generic;

namespace LitePlacer
{
    class CircleOptimizer
    {
        List<AForge.IntPoint> edgePoints;

        public void optimize(List<AForge.IntPoint> points, ref AForge.DoublePoint center, ref double radius)
        {
            edgePoints = points;
            double[] X = new double[edgePoints.Count];
            double[] Y = new double[edgePoints.Count];
            Tuple<double, double, double> result;
            for (int i = 0; i < edgePoints.Count; i++)
            {
                X[i] = i;
                Y[i] = 0;
            }
            AForge.DoublePoint Center = center;
            double Radius = radius;
            try
            {
                result = MathNet.Numerics.Fit.Curve(X, Y, CostFunction, radius, Center.X, Center.Y, 1E-03, 5000);
            }
            catch (Exception)
            {
                return;
            }
            center.X = result.Item2;
            center.Y = result.Item3;
            radius = result.Item1;
        }

        public double CostFunction(double radius, double centerx, double centery, double x)
        {
            AForge.DoublePoint center = new AForge.DoublePoint(centerx, centery);
            AForge.DoublePoint point = edgePoints[(int)x]; //abusing x as an index
            return center.DistanceTo(point) - radius;
        }
    }

    class homographyOptimizer
    {
        private int homographyPow = 1;
        private Vector<double> nomX;
        private Vector<double> nomY;
        private Vector<double> measuredX;
        private Vector<double> measuredY;
        private Vector<double> pX;
        private Vector<double> pY;

        public homographyOptimizer()
        {
            pX = null;
            pY = null;
        }
        /// <summary>
        /// calculate parameters with given Fiducials
        /// </summary>
        /// <param name="nomx"></param>
        /// <param name="nomy"></param>
        /// <param name="measuredx"></param>
        /// <param name="measuredy"></param>
        /// <returns>success</returns>
        public bool optimize(double[] nomx, double[] nomy, double[] measuredx, double[] measuredy)
        {
            nomX = Vector<double>.Build.Dense(nomx);
            nomY = Vector<double>.Build.Dense(nomy);
            measuredX = Vector<double>.Build.Dense(measuredx);
            measuredY = Vector<double>.Build.Dense(measuredy);

            //try to use the maximum possible potency for this number of fiducials
            double bestError = double.MaxValue;
            NonlinearMinimizationResult bestResult = null;
            for (int i = 1; i < 12; i++)
            {
                homographyPow = i;

                int pCount = 1 + 2 * homographyPow;
                pX = new DenseVector(pCount);
                pX[1] = 1.0;

                IObjectiveModel obj = ObjectiveFunction.NonlinearModel(homographyFunction, homographyDerivation, nomX, measuredX);
                LevenbergMarquardtMinimizer solver = new LevenbergMarquardtMinimizer(maximumIterations: 10000);
                NonlinearMinimizationResult result = solver.FindMinimum(obj, pX);
                double error = (result.ModelInfoAtMinimum.ModelValues - measuredX).AbsoluteMaximum();
                if (bestResult == null)
                {
                    bestResult = result;
                    bestError = error;
                }
                else if (error < bestError && Math.Abs(result.MinimizingPoint[1] - 1) < 0.1)
                {
                    bestResult = result;
                    bestError = error;
                }
                else
                {
                    pX = result.MinimizingPoint; //if performance gets worse due to too many parameters, take previous reulst
                    break;
                }
            }

            if (bestError == double.MaxValue)
            {
                pX = null;
                pY = null;
                return false;
            }

            bestError = double.MaxValue;
            bestResult = null;
            for (int i = 1; i < 12; i++)
            {
                homographyPow = i;

                int pCount = 1 + 2 * homographyPow;
                pY = new DenseVector(pCount);
                pY[homographyPow + 1] = 1.0;

                IObjectiveModel obj = ObjectiveFunction.NonlinearModel(homographyFunction, homographyDerivation, nomY, measuredY);
                LevenbergMarquardtMinimizer solver = new LevenbergMarquardtMinimizer(maximumIterations: 10000);
                NonlinearMinimizationResult result = solver.FindMinimum(obj, pY);
                double error = (result.ModelInfoAtMinimum.ModelValues - measuredY).AbsoluteMaximum();
                if (bestResult == null)
                {
                    bestResult = result;
                    bestError = error;
                }
                else if (error < bestError && Math.Abs(result.MinimizingPoint[homographyPow + 1] - 1) < 0.1)
                {
                    bestResult = result;
                    bestError = error;
                }
                else
                {
                    pY = result.MinimizingPoint;
                    break;
                }
            }

            if (bestError == double.MaxValue)
            {
                pX = null;
                pY = null;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Apply optimized parameters to nominal Position
        /// </summary>
        /// <param name="nominalPos"></param>
        /// <returns>calculated position</returns>
        public AForge.Point transformPoint(AForge.Point nominalPos)
        {
            if (pX == null || pY == null)
                return nominalPos;
            AForge.Point measuredPos = new AForge.Point();
            measuredPos.X = (float)GetValue(pX, nominalPos.X, nominalPos.Y);
            measuredPos.Y = (float)GetValue(pY, nominalPos.X, nominalPos.Y);
            return measuredPos;
        }

        private double GetValue(Vector<double> p, double a, double b)
        {
            double y = p[0];
            for (int j = 1; j <= homographyPow; j++)
            {
                y += Math.Pow(a, j) * p[0 + j];
            }
            for (int j = 1; j <= homographyPow; j++)
            {
                y += Math.Pow(b, j) * p[homographyPow + j];
            }
            return y;
        }

        private Vector<double> homographyFunction(Vector<double> p, Vector<double> x)
        {
            var y = CreateVector.Dense<double>(x.Count);
            for (int i = 0; i < x.Count; i++)
            {
                y[i] = GetValue(p, nomX[i], nomY[i]);
            }
            return y;
        }

        private Matrix<double> homographyDerivation(Vector<double> p, Vector<double> x)
        {
            var prime = Matrix<double>.Build.Dense(x.Count, p.Count);
            for (int i = 0; i < x.Count; i++)
            {
                prime[i, 0] = 1;
                for (int j = 1; j <= homographyPow; j++)
                {
                    prime[i, 0 + j] = Math.Pow(nomX[i], j);
                }
                for (int j = 1; j <= homographyPow; j++)
                {
                    prime[i, homographyPow + j] = Math.Pow(nomY[i], j);
                }
            }
            return prime;
        }
    }
}

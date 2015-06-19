using System;
using System.Collections.Generic;
using System.Linq;
using Emgu.CV;
using Emgu.CV.CvEnum;

namespace LitePlacer {
    // back computes a rigid transformation 
    // based on http://nghiaho.com/?page_id=671
    public class LeastSquaresMapping {
        List<PartLocation> source;
        List<PartLocation> dest;
        private Matrix<double> Rotation;
        private Matrix<double> Offset;
        Matrix<double> source_centroid, dest_centroid;

        public double Angle { get { return Math.Acos(Rotation[0, 0]) * 180d / Math.PI; } }

        public LeastSquaresMapping(List<PartLocation> from, List<PartLocation> to) { 
            source = from;
            dest = to;
            Recompute();
            
        }

        public void Recompute() {
            if (source == null || dest == null || source.Count != dest.Count)
                throw new Exception("Input data null or not equal in length");

            // determine locations of centroids
            source_centroid = PartLocation.Average(source).ToMatrix() ;
            dest_centroid = PartLocation.Average(dest).ToMatrix();

            // compute covariance matrix
            Matrix<double> H = new Matrix<double>(2, 2);
            H.SetZero();
            for (int i = 0; i < source.Count; i++) {
                var a = source[i].ToMatrix() - source_centroid;
                var b = dest[i].ToMatrix() - dest_centroid;
                H += a * b.Transpose();
            }

            /* perform svd  where A =  U W VT
             *  A  IntPtr  Source MxN matrix
             *  W  IntPtr  Resulting singular value matrix (MxN or NxN) or vector (Nx1).
             *  U  IntPtr  Optional left orthogonal matrix (MxM or MxN). If CV_SVD_U_T is specified, the number of rows and columns in the sentence above should be swapped
             *  V  IntPtr  Optional right orthogonal matrix (NxN)
             */
     
            Matrix<double> U = new Matrix<double>(2, 2); 
            Matrix<double> W = new Matrix<double>(2, 2);            
            Matrix<double> V = new Matrix<double>(2, 2);
            CvInvoke.cvSVD(H.Ptr, W.Ptr, U.Ptr, V.Ptr, SVD_TYPE.CV_SVD_DEFAULT);

            // compute rotational matrix R=V*UT
            Rotation = V * U.Transpose();

            // find translation
            Offset = dest_centroid - ( Rotation * source_centroid);

            Console.WriteLine("Transpose:\n" + Offset.Data);
            Console.WriteLine("Rotation:\n" + Rotation.Data);
        }

        /// <summary>
        /// Map a source point to a destination point based on the calibrated inputs
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public PartLocation Map(PartLocation from) {
            if (Rotation == null || Offset == null) throw new Exception("LeastSquareMapping not intialized");

            var x = from.ToMatrix();
            var y = Rotation * (x-source_centroid) + Offset; //shift point to center, apply rotation, then shift to the destination
            var p = new PartLocation(y);
            p.A = from.A + Angle;
            return p;
        }

        /// <summary>
        /// The RMS error of all the source to dest points
        /// </summary>
        /// <returns></returns>
        public double RMSError() {
            double rms_error = 0;
            for (int i = 0; i < source.Count; i++) {
                var b = Map(source[i]);
                rms_error += Math.Pow(b.DistanceTo(dest[i]), 2);
            }
            rms_error = Math.Sqrt(rms_error);
            return rms_error;
        }

        /// <summary>
        /// The furthest distance a fiducial moved
        /// </summary>
        /// <returns></returns>
        public double MaxFiducialMovement() {
            List<double> distances = new List<double>();
            for (int i = 0; i < source.Count; i++) {
                distances.Add(Map(source[i]).DistanceTo(dest[i]));
            }
            return distances.Max();
        }
                

    }
}

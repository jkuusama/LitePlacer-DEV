using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Drawing;
using System.Threading;
using System.Drawing.Imaging;

using AForge;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;



namespace LitePlacer
{
    public class Camera
    {
        private VideoCaptureDevice VideoSource = null;
        private FormMain MainForm;

        public Camera(FormMain MainF)
        {
            MainForm = MainF;
        }

        public bool Active { get; set; }

        public bool IsRunning()
        {
            if (VideoSource != null)
            {
                return (VideoSource.IsRunning);
            };
            return false;
        }

        public void SignalToStop()      // Asks nicely
        {
            VideoSource.SignalToStop();
        }

        public void NakedStop()         // Tries to force it (but still doesn't always work, just like with children)
        {
            VideoSource.Stop();
        }

        public void Close()
        {
            if (!(VideoSource == null))
            {
                if (!VideoSource.IsRunning)
                {
                    return;
                }
                VideoSource.SignalToStop();
                VideoSource.WaitForStop();  // problem?
                VideoSource.NewFrame -= new NewFrameEventHandler(Video_NewFrame);
                VideoSource = null;
                MainForm.DisplayText(Id + " stop: " + MonikerString);
                MonikerString = "Stopped";
            }
        }

        public void DisplayPropertyPage()
        {
            VideoSource.DisplayPropertyPage(IntPtr.Zero);

        }

        // Program has been crashing due to access of ImageBox.  As a shared resource it needs
        // to be accessed in protected regions.  The lock _locker is to be used for this purpose.
        // The OnPaint method in PictureBox runs in a second task.  By extending the PictureBox
        // class, overriding the OnPaint method and putting the call into a region protected
        // by _locker we can stop the crashing.
        public class ProtectedPictureBox : System.Windows.Forms.PictureBox
        {
            protected override void OnPaint(PaintEventArgs e)
            {
                lock (_locker)
                {
                    base.OnPaint(e);
                }
            }
        }

        // _locker must be a static Camera class variable to be available to the overridden OnPaint method
        private static object _locker = new object();

        // Image= PictureBox in UI, the final shown image
        // Frame= picture from camera

        // All processing and returned values are in Frame content
        // All references to PictureBox must be replace with ProtectedPictureBox
        ProtectedPictureBox _imageBox;
        public ProtectedPictureBox ImageBox
        {
            get
            {
                return (_imageBox);
            }
            set
            {
                lock (_locker)
                {
                    _imageBox = value;
                }
            }
        }

        public int FrameCenterX { get; set; }
        public int FrameCenterY { get; set; }
        public int FrameSizeX { get; set; }
        public int FrameSizeY { get; set; }
        public int DesiredX { get; set; }
        public int DesiredY { get; set; }

        public string MonikerString = "unconnected";
        public string Id = "unconnected";

        public bool ReceivingFrames { get; set; }

        // =================================================================================================
        public void ListResolutions(string MonikerStr)
        {
            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            // create video source
            if (videoDevices == null)
            {
                MainForm.DisplayText("No Cameras.", KnownColor.Purple);
                return;
            }
            if (videoDevices.Count == 0)
            {
                MainForm.DisplayText("No Cameras.", KnownColor.Purple);
                return;
            }
            VideoCaptureDevice source = new VideoCaptureDevice(MonikerStr);
            int tries = 0;
            while (tries < 4)
            {
                if (source == null)
                {
                    Thread.Sleep(20);
                    tries++;
                    break;
                }
                if (source.VideoCapabilities.Length > 0)
                {
                    for (int i = 0; i < source.VideoCapabilities.Length; i++)
                    {
                        MainForm.DisplayText("X: " + source.VideoCapabilities[i].FrameSize.Width.ToString() +
                            ", Y: " + source.VideoCapabilities[i].FrameSize.Height.ToString());
                    }
                    return;
                }
            }
            // if we didn't return from above:
            MainForm.DisplayText("Could not get resolution info.", KnownColor.Purple);
        }

        // =================================================================================================
        public bool Start(string cam, string MonikerStr)
        {
            try
            {
                MainForm.DisplayText(cam + " start, moniker= " + MonikerStr);

                // enumerate video devices
                FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                // create the video source (check that the camera exists is already done
                VideoSource = new VideoCaptureDevice(MonikerStr);
                int tries = 0;
                while (tries < 4)
                {
                    if (VideoSource != null)
                    {
                        break;
                    }
                    else
                    {
                        Thread.Sleep(10);
                        tries++;
                    }
                }
                if (tries>=4)
                {
                    MainForm.DisplayText("Could not get resolution info");
                    return false;
                }
                if (VideoSource.VideoCapabilities.Length <= 0)
                {
                    MainForm.DisplayText("Could not get resolution info");
                    return false;
                }

                bool fine = false;
                for (int i = 0; i < VideoSource.VideoCapabilities.Length; i++)
                {
                    if ((VideoSource.VideoCapabilities[i].FrameSize.Width== DesiredX)
                        &&
                        (VideoSource.VideoCapabilities[i].FrameSize.Height == DesiredY))
                    {
                        VideoSource.VideoResolution = VideoSource.VideoCapabilities[i];
                        fine = true;
                        break;
                    }
                }
                if (!fine)
                {
                    MainForm.DisplayText("Desired resolution not available");
                    return false;
                }

                VideoSource.NewFrame += new NewFrameEventHandler(Video_NewFrame);
                ReceivingFrames = false;

                // try ten times to start
                tries = 0;
                while (tries < 80)  // 4s maximum to a camera to start
                {
                    // VideoSource.Start() checks running status, is safe to call multiple times
                    tries++;
                    VideoSource.Start();
                    if (!ReceivingFrames)
                    {
                        // 50 ms pause, processing events so that videosource has a chance
                        for (int i = 0; i < 5; i++)
                        {
                            Thread.Sleep(10);
                            Application.DoEvents();
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                if (!ReceivingFrames)
                {
                    MainForm.DisplayText("Camera started, but is not sending video");
                    return false;
                }
                MainForm.DisplayText("*** Camera started: " + tries.ToString(), KnownColor.Purple);

                // We managed to start the camera using desired resolution
                FrameSizeX = DesiredX;
                FrameSizeY = DesiredY;
                FrameCenterX = FrameSizeX / 2;
                FrameCenterY = FrameSizeY / 2;
                PauseProcessing = false;
                return true;
            }
            catch (System.Exception excep)
            {
                MessageBox.Show(excep.Message);
                return false;
            }
        }

        public List<string> GetDeviceList()
        {
            List<string> Devices = new List<string>();

            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo device in videoDevices)
            {
                Devices.Add(device.Name);
            }
            return (Devices);
        }

        public List<string> GetMonikerStrings()
        {
            List<string> Monikers = new List<string>();

            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo device in videoDevices)
            {
                Monikers.Add(device.MonikerString);
            }
            return (Monikers);
        }



        // ==========================================================================================================
        // Video processing and measurements are done by appying AForge functions one by one to a videoframe.
        // To do this, lists of functions are maintained.
        // ==========================================================================================================
        // The list of functions processing the image used in measurements:
        List<AForgeFunction> MeasurementFunctions = new List<AForgeFunction>();
        // The list of functions processing the image shown to user:
        List<AForgeFunction> DisplayFunctions = new List<AForgeFunction>();

        enum DataGridViewColumns { Function, Active, Int, Double, R, G, B };

        public delegate void AForge_op(ref Bitmap frame, int par_int, double par_d, int par_R, int par_G, int par_B);
        class AForgeFunction
        {
            public AForge_op func { get; set; }
            public int parameter_int { get; set; }              // general parameters. Some functions take one int,
            public double parameter_double { get; set; }        // some take a float,
            public int R { get; set; }              // and some need R, B, G values.
            public int G { get; set; }
            public int B { get; set; }
        }


        private List<AForgeFunction> BuildFunctionsList(DataGridView Grid)
        {
            List<AForgeFunction> NewList = new List<AForgeFunction>();
            int temp_i;
            double temp_d;
            int FunctionCol = (int)DataGridViewColumns.Function;
            int ActiveCol = (int)DataGridViewColumns.Active;
            int IntCol = (int)DataGridViewColumns.Int;
            int DoubleCol = (int)DataGridViewColumns.Double;
            int R_col = (int)DataGridViewColumns.R;
            int G_col = (int)DataGridViewColumns.G;
            int B_col = (int)DataGridViewColumns.B;

            NewList.Clear();
            MainForm.DisplayText("BuildFunctionsList:");

            foreach (DataGridViewRow Row in Grid.Rows)
            {
                AForgeFunction f = new AForgeFunction();
                // newly created rows are not complete yet
                if (Row.Cells[FunctionCol].Value == null)
                {
                    continue;
                }
                if (Row.Cells[ActiveCol].Value == null)
                {
                    continue;
                }
                // skip inactive rows
                if (Row.Cells[ActiveCol].Value.ToString() == "False")
                {
                    continue;
                }

                if (Row.Cells[ActiveCol].Value.ToString() == "false")
                {
                    continue;
                }

                switch (Row.Cells[FunctionCol].Value.ToString())
                {
                    case "Grayscale":
                        f.func = GrayscaleFunc;
                        break;

                    case "Contrast scretch":
                        f.func = Contrast_scretchFunc;
                        break;

                    case "Kill color":
                        f.func = KillColor_Func;
                        break;

                    case "Keep color":
                        f.func = KeepColor_Func;
                        break;

                    case "Invert":
                        f.func = InvertFunct;
                        break;

                    case "Meas. zoom":
                        f.func = Meas_ZoomFunc;
                        break;

                    case "Edge detect":
                        f.func = Edge_detectFunc;
                        break;

                    case "Noise reduction":
                        f.func = NoiseReduction_Funct;
                        break;

                    case "Threshold":
                        f.func = ThresholdFunct;
                        break;

                    case "Histogram":
                        f.func = HistogramFunct;
                        break;

                    case "Blur":
                        f.func = BlurFunct;
                        break;

                    case "Gaussian blur":
                        f.func = GaussianBlurFunct;
                        break;

                    default:
                        continue;
                        // break; 
                }
                string msg = Row.Cells[FunctionCol].Value.ToString();
                msg += " / ";
                if (Row.Cells[IntCol].Value != null)
                {
                    int.TryParse(Row.Cells[IntCol].Value.ToString(), out temp_i);
                    f.parameter_int = temp_i;
                    msg += temp_i.ToString();
                }
                msg += " / ";
                if (Row.Cells[DoubleCol].Value != null)
                {
                    double.TryParse(Row.Cells[DoubleCol].Value.ToString().Replace(',', '.'), out temp_d);
                    f.parameter_double = temp_d;
                    msg += temp_d.ToString();
                }
                msg += " / ";
                if (Row.Cells[R_col].Value != null)
                {
                    int.TryParse(Row.Cells[R_col].Value.ToString(), out temp_i);
                    f.R = temp_i;
                    msg += temp_i.ToString();
                }
                msg += " / ";
                if (Row.Cells[G_col].Value != null)
                {
                    int.TryParse(Row.Cells[G_col].Value.ToString(), out temp_i);
                    f.G = temp_i;
                    msg += temp_i.ToString();
                }
                msg += " / ";
                if (Row.Cells[B_col].Value != null)
                {
                    int.TryParse(Row.Cells[B_col].Value.ToString(), out temp_i);
                    f.B = temp_i;
                    msg += temp_i.ToString();
                }
                msg += " / ";
                NewList.Add(f);
                MainForm.DisplayText(msg);
            };
            return NewList;
        }

        public void BuildDisplayFunctionsList(DataGridView Grid)
        {
            List<AForgeFunction> NewList = BuildFunctionsList(Grid);    // Get the list
                                                                        // Stop video
            bool pause = PauseProcessing;
            if (VideoSource != null)
            {
                if (ReceivingFrames)
                {
                    // stop video
                    PauseProcessing = true;  // ask for stop
                    paused = false;
                    while (!paused)
                    {
                        Thread.Sleep(10);  // wait until really stopped
                    };
                }
            }
            // copy new list
            DisplayFunctions.Clear();
            DisplayFunctions = NewList;
            PauseProcessing = pause;  // restart video is it was running
        }

        public void ClearDisplayFunctionsList()
        {
            // Stop video
            bool pause = PauseProcessing;
            if (VideoSource != null)
            {
                if (ReceivingFrames)
                {
                    // stop video
                    PauseProcessing = true;  // ask for stop
                    paused = false;
                    while (!paused)
                    {
                        Thread.Sleep(10);  // wait until really stopped
                    };
                }
            }
            DisplayFunctions.Clear();
            PauseProcessing = pause;  // restart video is it was running
        }


        // ==========================================================================================================
        // Measurements are done by taking one frame and processing that:
        private bool CopyFrame = false;     // Tells we need to take a frame from the stream 
        private Bitmap TemporaryFrame;      // The frame is stored here.

        // The caller builds the MeasurementFunctions list:

        public void BuildMeasurementFunctionsList(DataGridView Grid)
        {
            MeasurementFunctions = BuildFunctionsList(Grid);
        }

        // And calls xx_measure() funtion. (Any function doing measurement from video frames.)
        // The xxx_measure funtion calls GetMeasurementFrame() function, that takes a frame from the stream, 
        // processes it with the MeasurementFunctions list and returns the processed frame:

        private Bitmap GetMeasurementFrame()
        {
            // Take a snapshot:
            CopyFrame = true;
            int tries = 100;
            while (tries > 0)
            {
                tries--;
                if (!CopyFrame)
                {
                    break;
                }
                Thread.Sleep(10);
                Application.DoEvents();
            }
            if (CopyFrame)
            {
                // failed!
                Graphics g = Graphics.FromImage(TemporaryFrame);
                g.Clear(Color.Black);
                g.Dispose();
                MainForm.DisplayText("*** GetMeasurementFrame() failed!", KnownColor.Purple);
                return TemporaryFrame;
            }

            if (MeasurementFunctions != null)
            {
                foreach (AForgeFunction f in MeasurementFunctions)
                {
                    f.func(ref TemporaryFrame, f.parameter_int, f.parameter_double, f.R, f.B, f.G);
                }
            }
            return TemporaryFrame;
        }

        // Since the measured image might be zoomed in, we need the value, so that we can convert to real measurements (public for debug)
        public double GetMeasurementZoom()
        {
            double zoom = 1.0;
            foreach (AForgeFunction f in MeasurementFunctions)
            {
                if (f.func == Meas_ZoomFunc)
                {
                    zoom = zoom * f.parameter_double;
                }
            }
            return zoom;
        }

        // UI also needs the zoom factor from the DisplayFunctions
        public double GetDisplayZoom()
        {
            double zoom = 1.0;
            foreach (AForgeFunction f in DisplayFunctions)
            {
                if (f.func == Meas_ZoomFunc)
                {
                    zoom = zoom * f.parameter_double;
                }
            }
            return zoom;
        }
        // ==========================================================================================================
        // Members we need for our drawing functions
        // ==========================================================================================================

        // ==========================================================================================================
        // Zoom
        public bool Zoom { get; set; }          // If image is zoomed or not
        private double _ZoomFactor = 1.0;
        public double ZoomFactor                // If it is, this much
        {
            get
            {
                return _ZoomFactor;
            }
            set
            {
                _ZoomFactor = value;
            }
        }

        public bool Mirror { get; set; }                    // If image is mirrored (On upcam, more logical)
        public int Threshold { get; set; }                  // Threshold for all the "draw" functions
        public bool GrayScale { get; set; }                 // If image is converted to grayscale 
        public bool Invert { get; set; }                    // If image is inverted (makes most sense on grayscale, looking for black stuff on light background)
        public bool DrawCross { get; set; }         // If crosshair cursor is drawn
        public bool DrawArrow { get; set; }         // If arrow is drawn
        public double ArrowAngle { get; set; }      // to which angle
        public bool DrawSidemarks { get; set; }     // If marks on the side of the image are drawn
        public double SideMarksX { get; set; }      // How many marks on top and bottom (X) and sides (Y)
        public double SideMarksY { get; set; }      // (double, so you can do "SidemarksX= workarea_in_mm / 100;" to get mark every 10cm
        public bool DrawDashedCross { get; set; }   // If a dashed crosshaircursor is drawn (so that the center remains visible)
        public bool DrawGrid { get; set; }          // Draws aiming grid for parts alignment
        public bool FindCircles { get; set; }       // Find and highlight circles in the image
        public bool FindRectangles { get; set; }    // Find and draw regtangles in the image
        public bool FindComponent { get; set; }     // Finds a component and identifies its center
        public bool Draw_Snapshot { get; set; }     // Draws the snapshot on the image 
        public bool PauseProcessing { get; set; }   // Drawing the video slows everything down. This can pause it for measurements.
        private bool paused = true;                 // set in video processing indicating it is safe to change processing function list
        public bool TestAlgorithm { get; set; }
        public bool DrawBox { get; set; }           // Draws a box on the image that is used for scale setting

        private int boxSizeX;
        public int BoxSizeX                         // The box size
        {
            get
            {
                return (boxSizeX);
            }
            set
            {
                boxSizeX = value;
                BoxRotationDeg = boxRotation; // force recalculation of corner points

            }
        }
        private int boxSizeY;
        public int BoxSizeY
        {
            get
            {
                return (boxSizeY);
            }
            set
            {
                boxSizeY = value;
                BoxRotationDeg = boxRotation; // force recalculation of corner points

            }
        }

        private double boxRotation = 0;
        private double boxRotationRad = 0;
        private System.Drawing.Point[] BoxPoints = new System.Drawing.Point[4];
        public double BoxRotationDeg        // The box is drawn rotated this much
        {
            get
            {
                return (boxRotation);
            }
            set
            {
                boxRotation = value;
                // Calculate corner points
                BoxPoints[0].X = BoxSizeX / 2;
                BoxPoints[0].Y = BoxSizeY / 2;
                BoxPoints[1].X = -BoxSizeX / 2;
                BoxPoints[1].Y = BoxSizeY / 2;
                BoxPoints[2].X = -BoxSizeX / 2;
                BoxPoints[2].Y = -BoxSizeY / 2;
                BoxPoints[3].X = BoxSizeX / 2;
                BoxPoints[3].Y = -BoxSizeY / 2;
                // now, rotate them:
                boxRotationRad = -boxRotation / (180 / Math.PI);  // to radians, and counter-clockwise
                for (int i = 0; i < 4; i++)
                {
                    // If you rotate point (px, py) around point (ox, oy) by angle theta you'll get:
                    // p'x = cos(theta) * (px-ox) - sin(theta) * (py-oy) + ox
                    // p'y = sin(theta) * (px-ox) + cos(theta) * (py-oy) + oy
                    // ox, oy= 0 ==>
                    // p'x = cos(theta) * (px) - sin(theta) * (py)
                    // p'y = sin(theta) * (px) + cos(theta) * (py)
                    double pX = BoxPoints[i].X;
                    double pY = BoxPoints[i].Y;
                    BoxPoints[i].X = (int)Math.Round(Math.Cos(boxRotationRad) * pX - Math.Sin(boxRotationRad) * pY);
                    BoxPoints[i].Y = (int)Math.Round(Math.Sin(boxRotationRad) * pX + Math.Cos(boxRotationRad) * pY);
                }
            }
        }



        // =========================================================
        // Convert list of AForge.NET's points to array of .NET points
        private System.Drawing.Point[] ToPointsArray(List<IntPoint> points)
        {
            System.Drawing.Point[] array = new System.Drawing.Point[points.Count];

            for (int i = 0, n = points.Count; i < n; i++)
            {
                array[i] = new System.Drawing.Point(points[i].X, points[i].Y);
            }

            return array;
        }


        // ==========================================================================================================
        // ==========================================================================================================

        // Each frame goes through Video_NewFrame()

        // ==========================================================================================================
        // ==========================================================================================================

        Bitmap frame;
        private void Video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            ReceivingFrames = true;
            // Working with a copy of the frame to avoid conflict.  Protecting the region where the copy is made
            lock (_locker)
            {
                frame = (Bitmap)eventArgs.Frame.Clone();
            }
            if (CopyFrame)
            {
                if (TemporaryFrame != null)
                {
                    TemporaryFrame.Dispose();
                }
                TemporaryFrame = (Bitmap)frame.Clone();
                CopyFrame = false;
            };

            if (PauseProcessing)
            {
                //if (ImageBox.Image != null)
                //{
                //    ImageBox.Image.Dispose();
                //}
                frame.Dispose();
                paused = true;
                return;
            };

            if (!Active)
            {
                //if (ImageBox.Image != null)
                //{
                //    ImageBox.Image.Dispose();
                //}
                //ImageBox.Image = (Bitmap)frame.Clone();
                frame.Dispose();
                return;
            }


            if (DisplayFunctions != null)
            {
                foreach (AForgeFunction f in DisplayFunctions)
                {
                    f.func(ref frame, f.parameter_int, f.parameter_double, f.R, f.B, f.G);
                }
            }

            if (FindCircles)
            {
                DrawCirclesFunct(frame);
            };

            if (FindRectangles)
            {
                frame = DrawRectanglesFunct(frame);
            };

            if (FindComponent)
            {
                frame = DrawComponentsFunct(frame);
            };

            if (Draw_Snapshot)
            {
                frame = Draw_SnapshotFunct(frame);
            };

            if (Mirror)
            {
                frame = MirrorFunct(frame);
            };

            if (DrawBox)
            {
                DrawBoxFunct(frame);
            };

            if (Zoom)
            {
                ZoomFunct(ref frame, ZoomFactor);
            };

            if (DrawCross)
            {
                DrawCrossFunct(ref frame);
            };

            if (DrawSidemarks)
            {
                DrawSidemarksFunct(ref frame);
            };

            if (DrawDashedCross)
            {
                DrawDashedCrossFunct(frame);
            };

            if (DrawGrid)
            {
                DrawGridFunct(frame);
            };

            if (DrawArrow)
            {
                DrawArrowFunct(frame);
            };

            lock (_locker)
            {
                if (ImageBox.Image != null)
                {
                    ImageBox.Image.Dispose();
                }
                ImageBox.Image = (Bitmap)frame.Clone();
            }

            frame.Dispose();
            GC.Collect();
        } // end Video_NewFrame

        // see http://www.codeproject.com/Questions/689320/object-is-currently-in-use-elsewhere about the locker

        // ==========================================================================================================
        // Functions compatible with lists:
        // ==========================================================================================================
        // Note, that each function needs to keep the image in RGB, otherwise drawing fill fail 

        // ========================================================= 

        private void NoiseReduction_Funct(ref Bitmap frame, int par_int, double par_d, int par_R, int par_G, int par_B)
        {
            frame = Grayscale.CommonAlgorithms.RMY.Apply(frame);    // Make gray
            switch (par_int)
            {
                case 1:
                    BilateralSmoothing Bil_filter = new BilateralSmoothing();
                    Bil_filter.KernelSize = 7;
                    Bil_filter.SpatialFactor = 10;
                    Bil_filter.ColorFactor = 30;
                    Bil_filter.ColorPower = 0.5;
                    Bil_filter.ApplyInPlace(frame);
                    break;

                case 2:
                    Median M_filter = new Median();
                    M_filter.ApplyInPlace(frame);
                    break;

                case 3:
                    Mean Meanfilter = new Mean();
                    // apply the MirrFilter
                    Meanfilter.ApplyInPlace(frame);
                    break;

                default:
                    Median Median_filter = new Median();
                    Median_filter.ApplyInPlace(frame);
                    break;
            }
            GrayscaleToRGB RGBfilter = new GrayscaleToRGB();    // back to color format
            frame = RGBfilter.Apply(frame);
        }

        // =========================================================
        private void Edge_detectFunc(ref Bitmap frame, int par_int, double par_d, int par_R, int par_G, int par_B)
        {
            frame = Grayscale.CommonAlgorithms.RMY.Apply(frame);    // Make gray
            switch (par_int)
            {
                case 1:
                    SobelEdgeDetector SobelFilter = new SobelEdgeDetector();
                    SobelFilter.ApplyInPlace(frame);
                    break;

                case 2:
                    DifferenceEdgeDetector DifferenceFilter = new DifferenceEdgeDetector();
                    DifferenceFilter.ApplyInPlace(frame);
                    break;

                case 3:
                    HomogenityEdgeDetector HomogenityFilter = new HomogenityEdgeDetector();
                    HomogenityFilter.ApplyInPlace(frame);
                    break;

                case 4:
                    CannyEdgeDetector Cannyfilter = new CannyEdgeDetector();
                    // apply the MirrFilter
                    Cannyfilter.ApplyInPlace(frame);
                    break;

                default:
                    HomogenityEdgeDetector filter = new HomogenityEdgeDetector();
                    filter.ApplyInPlace(frame);
                    break;
            }
            GrayscaleToRGB RGBfilter = new GrayscaleToRGB();    // back to color format
            frame = RGBfilter.Apply(frame);
        }

        // =========================================================
        private void InvertFunct(ref Bitmap frame, int par_int, double par_d, int par_R, int par_G, int par_B)
        {
            Invert filter = new Invert();
            filter.ApplyInPlace(frame);
        }

        // =========================================================
        private void HistogramFunct(ref Bitmap frame, int par_int, double par_d, int par_R, int par_G, int par_B)
        {
            // create MirrFilter
            HistogramEqualization filter = new HistogramEqualization();
            // process image
            filter.ApplyInPlace(frame);
        }


        // =========================================================
        private void BlurFunct(ref Bitmap frame, int par_int, double par_d, int par_R, int par_G, int par_B)
        {
            // create filter
            Blur filter = new Blur();
            // apply the filter
            filter.ApplyInPlace(frame);
        }


        // =========================================================
        private void GaussianBlurFunct(ref Bitmap frame, int par_int, double par_d, int par_R, int par_G, int par_B)
        {
            // create filter with kernel size equal to 11
            GaussianBlur filter = new GaussianBlur(par_d, 11);
            // apply the filter
            filter.ApplyInPlace(frame);
        }


        // =========================================================
        private void KillColor_Func(ref Bitmap frame, int par_int, double par_d, int par_R, int par_G, int par_B)
        {
            // create MirrFilter
            EuclideanColorFiltering filter = new EuclideanColorFiltering();
            // set center colol and radius
            filter.CenterColor = new RGB((byte)par_R, (byte)par_G, (byte)par_B);
            filter.Radius = (short)par_int;
            filter.FillOutside = false;
            // apply the MirrFilter
            filter.ApplyInPlace(frame);
        }

        // =========================================================
        private void KeepColor_Func(ref Bitmap frame, int par_int, double par_d, int par_R, int par_G, int par_B)
        {
            // create MirrFilter
            EuclideanColorFiltering filter = new EuclideanColorFiltering();
            // set center colol and radius
            filter.CenterColor = new RGB((byte)par_R, (byte)par_G, (byte)par_B);
            filter.Radius = (short)par_int;
            filter.FillOutside = true;
            // apply the MirrFilter
            filter.ApplyInPlace(frame);
        }

        // =========================================================
        private void ThresholdFunct(ref Bitmap frame, int par_int, double par_d, int par_R, int par_G, int par_B)
        {
            frame = Grayscale.CommonAlgorithms.RMY.Apply(frame);
            Threshold filter = new Threshold(par_int);
            filter.ApplyInPlace(frame);
            GrayscaleToRGB toColFilter = new GrayscaleToRGB();
            frame = toColFilter.Apply(frame);
        }


        // ========================================================= Contrast_scretchFunc
        private void GrayscaleFunc(ref Bitmap frame, int par_int, double par_d, int par_R, int par_G, int par_B)
        {
            Grayscale toGrFilter = new Grayscale(0.2125, 0.7154, 0.0721);       // create grayscale MirrFilter (BT709)
            Bitmap fr = toGrFilter.Apply(frame);
            GrayscaleToRGB toColFilter = new GrayscaleToRGB();
            frame = toColFilter.Apply(fr);
        }

        private void Contrast_scretchFunc(ref Bitmap frame, int par_int, double par_d, int par_R, int par_G, int par_B)
        {
            ContrastStretch filter = new ContrastStretch();
            // process image
            filter.ApplyInPlace(frame);
        }

        // =========================================================
        private void Meas_ZoomFunc(ref Bitmap frame, int par_int, double par_d, int par_R, int par_G, int par_B)
        {
            ZoomFunct(ref frame, par_d);
        }

        // ==========================================================================================================
        // Components:
        // ==========================================================================================================
        private List<Shapes.Component> FindComponentsFunct(Bitmap bitmap)
        {
            // Locating objects
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.FilterBlobs = true;
            blobCounter.MinHeight = 8;
            blobCounter.MinWidth = 8;
            blobCounter.ProcessImage(bitmap);
            Blob[] blobs = blobCounter.GetObjectsInformation();

            // create convex hull searching algorithm
            GrahamConvexHull hullFinder = new GrahamConvexHull();
            ClosePointsMergingOptimizer optimizer1 = new ClosePointsMergingOptimizer();
            FlatAnglesOptimizer optimizer2 = new FlatAnglesOptimizer();

            List<Shapes.Component> Components = new List<Shapes.Component>();

            // process each blob
            foreach (Blob blob in blobs)
            {
                List<IntPoint> leftPoints, rightPoints, edgePoints = new List<IntPoint>();
                if ((blob.Rectangle.Height > 400) && (blob.Rectangle.Width > 600))
                {
                    break;  // The whole image could be a blob, discard that
                }
                // get blob's edge points
                blobCounter.GetBlobsLeftAndRightEdges(blob,
                    out leftPoints, out rightPoints);

                edgePoints.AddRange(leftPoints);
                edgePoints.AddRange(rightPoints);

                // blob's convex hull
                List<IntPoint> Outline = hullFinder.FindHull(edgePoints);
                optimizer1.MaxDistanceToMerge = 4;
                optimizer2.MaxAngleToKeep = 170F;
                Outline = optimizer2.OptimizeShape(Outline);
                Outline = optimizer1.OptimizeShape(Outline);

                // find Longest line segment
                float dist = 0;
                LineSegment Longest = new LineSegment(Outline[0], Outline[1]);
                LineSegment line;
                dist = Longest.Length;
                int LongestInd = 0;
                for (int i = 1; i < Outline.Count; i++)
                {
                    if (i != Outline.Count - 1)
                    {
                        line = new LineSegment(Outline[i], Outline[i + 1]);
                    }
                    else
                    {
                        // last iteration
                        if (Outline[i] == Outline[0])
                        {
                            break;
                        }
                        line = new LineSegment(Outline[i], Outline[0]);
                    }
                    if (line.Length > dist)
                    {
                        Longest = line;
                        dist = line.Length;
                        LongestInd = i;
                    }
                }
                // Get the center point of it
                AForge.Point LongestCenter = new AForge.Point();
                LongestCenter.X = (float)Math.Round((Longest.End.X - Longest.Start.X) / 2.0 + Longest.Start.X);
                LongestCenter.Y = (float)Math.Round((Longest.End.Y - Longest.Start.Y) / 2.0 + Longest.Start.Y);
                AForge.Point NormalStart = new AForge.Point();
                AForge.Point NormalEnd = new AForge.Point();
                // Find normal: 
                // start= longest.start rotated +90deg relative to center
                // end= longest.end rotated -90deg and relative to center
                // If you rotate point (px, py) around point (ox, oy) by angle theta you'll get:
                // p'x = cos(theta) * (px-ox) - sin(theta) * (py-oy) + ox
                // p'y = sin(theta) * (px-ox) + cos(theta) * (py-oy) + oy
                // cos90 = 0, sin90= 1 => 
                // p'x= -(py-oy) + ox= oy-py+ox, p'y= (px-ox)+ oy
                NormalStart.X = LongestCenter.Y - Longest.Start.Y + LongestCenter.X;
                NormalStart.Y = (Longest.Start.X - LongestCenter.X) + LongestCenter.Y;
                // cos-90=0, sin-90= -1 =>
                // p'x= (py-oy) + ox
                // p'y= -(px-ox)+oy= ox-px+oy
                NormalEnd.X = (Longest.Start.Y - LongestCenter.Y) + LongestCenter.X;
                NormalEnd.Y = LongestCenter.X - Longest.Start.X + LongestCenter.Y;
                // Make line out of the points
                Line Normal = Line.FromPoints(NormalStart, NormalEnd);

                // Find the furthest intersection to the normal (skip the Longest)
                AForge.Point InterSection = new AForge.Point();
                AForge.Point Furthest = new AForge.Point();
                bool FurhtestAssinged = false;
                LineSegment seg;
                dist = 0;
                for (int i = 0; i < Outline.Count; i++)
                {
                    if (i == LongestInd)
                    {
                        continue;
                    }
                    if (i != Outline.Count - 1)
                    {
                        seg = new LineSegment(Outline[i], Outline[i + 1]);
                    }
                    else
                    {
                        // last iteration
                        if (Outline[i] == Outline[0])
                        {
                            break;
                        }
                        seg = new LineSegment(Outline[i], Outline[0]);
                    }
                    if (seg.GetIntersectionWith(Normal) == null)
                    {
                        continue;
                    }
                    InterSection = (AForge.Point)seg.GetIntersectionWith(Normal);
                    if (InterSection.DistanceTo(LongestCenter) > dist)
                    {
                        Furthest = InterSection;
                        FurhtestAssinged = true;
                        dist = InterSection.DistanceTo(LongestCenter);
                    }
                }
                // Check, if there is a edge point that is close to the normal even further
                AForge.Point fPoint = new AForge.Point();
                for (int i = 0; i < Outline.Count; i++)
                {
                    fPoint.X = Outline[i].X;
                    fPoint.Y = Outline[i].Y;
                    if (Normal.DistanceToPoint(fPoint) < 1.5)
                    {
                        if (fPoint.DistanceTo(LongestCenter) > dist)
                        {
                            Furthest = fPoint;
                            FurhtestAssinged = true;
                            dist = fPoint.DistanceTo(LongestCenter);
                        }
                    }
                }
                AForge.Point ComponentCenter = new AForge.Point();
                if (FurhtestAssinged)
                {
                    // Find the midpoint of LongestCenter and Furthest: This is the centerpoint of component
                    ComponentCenter.X = (float)Math.Round((LongestCenter.X - Furthest.X) / 2.0 + Furthest.X);
                    ComponentCenter.Y = (float)Math.Round((LongestCenter.Y - Furthest.Y) / 2.0 + Furthest.Y);
                    // Alignment is the angle of longest
                    double Alignment;
                    if (Math.Abs(Longest.End.X - Longest.Start.X) < 0.001)
                    {
                        Alignment = 0;
                    }
                    else
                    {
                        Alignment = Math.Atan((Longest.End.Y - Longest.Start.Y) / (Longest.End.X - Longest.Start.X));
                        Alignment = Alignment * 180.0 / Math.PI; // in deg.
                    }
                    Components.Add(new Shapes.Component(ComponentCenter, Alignment, Outline, Longest, NormalStart, NormalEnd));
                }
            }
            return Components;
        }

        private Bitmap DrawComponentsFunct(Bitmap bitmap)
        {
            List<Shapes.Component> Components = FindComponentsFunct(bitmap);

            Graphics g = Graphics.FromImage(bitmap);
            Pen OrangePen = new Pen(Color.DarkOrange, 1);
            Pen RedPen = new Pen(Color.DarkRed, 2);
            Pen BluePen = new Pen(Color.Blue, 2);
            Shapes.Component Component;
            System.Drawing.Point p1 = new System.Drawing.Point();
            System.Drawing.Point p2 = new System.Drawing.Point();

            for (int i = 0, n = Components.Count; i < n; i++)
            {
                Component = Components[i];

                // Move Component.Longest start to ComponentCenter, draw it
                float dx = Component.Center.X - Component.Longest.Start.X;
                float dy = Component.Center.Y - Component.Longest.Start.Y;
                p1.X = (int)Math.Round(Component.Longest.Start.X + dx);
                p1.Y = (int)Math.Round(Component.Longest.Start.Y + dy);
                p2.X = (int)Math.Round(Component.Longest.End.X + dx);
                p2.Y = (int)Math.Round(Component.Longest.End.Y + dy);
                g.DrawLine(RedPen, p1, p2);

                // Move Component.Longest end to ComponentCenter, draw Component.Longest
                dx = Component.Center.X - Component.Longest.End.X;
                dy = Component.Center.Y - Component.Longest.End.Y;
                p1.X = (int)Math.Round(Component.Longest.Start.X + dx);
                p1.Y = (int)Math.Round(Component.Longest.Start.Y + dy);
                p2.X = (int)Math.Round(Component.Longest.End.X + dx);
                p2.Y = (int)Math.Round(Component.Longest.End.Y + dy);
                g.DrawLine(RedPen, p1, p2);

                //  Move Normal start to ComponentCenter, draw it
                dx = Component.Center.X - Component.NormalStart.X;
                dy = Component.Center.Y - Component.NormalStart.Y;
                p1.X = (int)Math.Round(Component.NormalStart.X + dx);
                p1.Y = (int)Math.Round(Component.NormalStart.Y + dy);
                p2.X = (int)Math.Round(Component.NormalEnd.X + dx);
                p2.Y = (int)Math.Round(Component.NormalEnd.Y + dy);
                g.DrawLine(RedPen, p1, p2);

                //  Move Component.Normal end to ComponentCenter, draw it
                dx = Component.Center.X - Component.NormalEnd.X;
                dy = Component.Center.Y - Component.NormalEnd.Y;
                p1.X = (int)Math.Round(Component.NormalStart.X + dx);
                p1.Y = (int)Math.Round(Component.NormalStart.Y + dy);
                p2.X = (int)Math.Round(Component.NormalEnd.X + dx);
                p2.Y = (int)Math.Round(Component.NormalEnd.Y + dy);
                g.DrawLine(RedPen, p1, p2);

                // draw outline
                g.DrawPolygon(OrangePen, ToPointsArray(Component.Outline));

                // draw Component.Longest
                p1.X = (int)Math.Round(Component.Longest.Start.X);
                p1.Y = (int)Math.Round(Component.Longest.Start.Y);
                p2.X = (int)Math.Round(Component.Longest.End.X);
                p2.Y = (int)Math.Round(Component.Longest.End.Y);
                g.DrawLine(BluePen, p1, p2);
            }
            g.Dispose();
            OrangePen.Dispose();
            RedPen.Dispose();
            BluePen.Dispose();
            return (bitmap);
        }

        public int GetClosestComponent(out double X, out double Y, out double A, double MaxDistance)
        // Sets X, Y position of the closest component to the frame center in pixels, 
        // A to rotation in degrees, 
        // return value is number of components found
        {
            Bitmap image = GetMeasurementFrame();
            List<Shapes.Component> RawComponents = FindComponentsFunct(image);
            image.Dispose();
            List<Shapes.Component> GoodComponents = new List<Shapes.Component>();

            X = 0.0;
            Y = 0.0;
            A = 0.0;
            if (RawComponents.Count == 0)
            {
                return (0);
            }
            // Remove those that are more than MaxDistance away from frame center
            foreach (Shapes.Component Component in RawComponents)
            {
                X = (Component.Center.X - FrameCenterX);
                Y = (Component.Center.Y - FrameCenterY);
                if ((X * X + Y * Y) < (MaxDistance * MaxDistance))
                {
                    GoodComponents.Add(Component);
                }
            }
            if (GoodComponents.Count == 0)
            {
                return (0);
            }
            // Find the closest
            X = (GoodComponents[0].Center.X - FrameCenterX);
            Y = (GoodComponents[0].Center.Y - FrameCenterY);
            A = GoodComponents[0].Alignment;
            double dist = X * X + Y * Y;  // we return X and Y, so we don't neet to take square roots to use right distance value
            double dX, dY;
            for (int i = 0; i < GoodComponents.Count; i++)
            {
                dX = GoodComponents[i].Center.X - FrameCenterX;
                dY = GoodComponents[i].Center.Y - FrameCenterY;
                if ((dX * dX + dY * dY) < dist)
                {
                    dist = dX * dX + dY * dY;
                    X = dX;
                    Y = dY;
                    A = GoodComponents[i].Alignment;
                }
            }
            double zoom = GetMeasurementZoom();
            X = X / zoom;
            Y = Y / zoom;
            return (GoodComponents.Count);
        }

        // ==========================================================================================================
        // Circles:
        // ==========================================================================================================
        private List<Shapes.Circle> FindCirclesFunct(Bitmap bitmap)
        {
            // locating objects
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.FilterBlobs = true;
            blobCounter.MinHeight = 5;
            blobCounter.MinWidth = 5;
            blobCounter.ProcessImage(bitmap);
            Blob[] blobs = blobCounter.GetObjectsInformation();

            List<Shapes.Circle> Circles = new List<Shapes.Circle>();

            for (int i = 0, n = blobs.Length; i < n; i++)
            {
                SimpleShapeChecker shapeChecker = new SimpleShapeChecker();
                List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(blobs[i]);
                AForge.Point center;
                float radius;

                // is circle ?
                if (shapeChecker.IsCircle(edgePoints, out center, out radius))
                {
                    if (radius > 3)  // MirrFilter out some noise
                    {
                        Circles.Add(new Shapes.Circle(center.X, center.Y, radius));
                    }
                }
            }
            return (Circles);
        }

        // =========================================================
        private void DrawCirclesFunct(Bitmap bitmap)
        {
            List<Shapes.Circle> Circles = FindCirclesFunct(bitmap);

            if (Circles.Count == 0)
            {
                return;
            }
            // Find smallest
            int Smallest = FindSmallestCircle(Circles);

            // find closest to center
            int Closest = FindClosestCircle(Circles);

            Graphics g = Graphics.FromImage(bitmap);
            Pen OrangePen = new Pen(Color.DarkOrange, 2);
            Pen AquaPen = new Pen(Color.Aqua, 2);
            Pen LimePen = new Pen(Color.Lime, 2);
            Pen MagentaPen = new Pen(Color.Magenta, 2);

            if (Closest == Smallest)
            {
                g.DrawEllipse(MagentaPen,
                    (float)(Circles[Closest].X - Circles[Closest].Radius), (float)(Circles[Closest].Y - Circles[Closest].Radius),
                    (float)(Circles[Closest].Radius * 2), (float)(Circles[Closest].Radius * 2));
            }
            else
            {
                g.DrawEllipse(LimePen,
                    (float)(Circles[Closest].X - Circles[Closest].Radius), (float)(Circles[Closest].Y - Circles[Closest].Radius),
                    (float)(Circles[Closest].Radius * 2), (float)(Circles[Closest].Radius * 2));
                g.DrawEllipse(AquaPen,
                    (float)(Circles[Smallest].X - Circles[Smallest].Radius), (float)(Circles[Smallest].Y - Circles[Smallest].Radius),
                    (float)(Circles[Smallest].Radius * 2), (float)(Circles[Smallest].Radius * 2));
            }


            for (int i = 0, n = Circles.Count; i < n; i++)
            {
                if ((i != Closest) && (i != Smallest))
                {
                    g.DrawEllipse(OrangePen,
                       (float)(Circles[i].X - Circles[i].Radius), (float)(Circles[i].Y - Circles[i].Radius),
                       (float)(Circles[i].Radius * 2), (float)(Circles[i].Radius * 2));
                }
            }
            g.Dispose();
            OrangePen.Dispose();
            AquaPen.Dispose();
            LimePen.Dispose();
            MagentaPen.Dispose();
        }

        // =========================================================
        private int FindSmallestCircle(List<Shapes.Circle> Circles)
        {
            int Smallest = 0;
            double SmallRadius = Circles[0].Radius;
            for (int i = 0, n = Circles.Count; i < n; i++)
            {
                if (Circles[i].Radius < SmallRadius)
                {
                    Smallest = i;
                    SmallRadius = Circles[i].Radius;
                }
            }

            return Smallest;
        }

        // =========================================================
        private int FindClosestCircle(List<Shapes.Circle> Circles)
        {
            int closest = 0;
            double X = (Circles[0].X - FrameCenterX);
            double Y = (Circles[0].Y - FrameCenterY);
            double dist = X * X + Y * Y;  // we are interested only which one is closest, don't neet to take square roots to get distance right
            double dX, dY;
            for (int i = 0; i < Circles.Count; i++)
            {
                dX = Circles[i].X - FrameCenterX;
                dY = Circles[i].Y - FrameCenterY;
                if ((dX * dX + dY * dY) < dist)
                {
                    dist = dX * dX + dY * dY;
                    closest = i;
                }
            }
            return closest;
        }

        // =========================================================
        //public bool SizeLimited { get; set; } = false;
        //public double MaxSize { get; set; } = 1000;
        //public double MinSize { get; set; } = 1000;
        public bool SizeLimited  = false;
        public double MaxSize = 1000;
        public double MinSize = 1000;

        public List<Shapes.Circle> GetMeasurementCircles(double MaxDistance)
        {
            // returns a list of circles for measurements, filters for distance (which we always do) and size, if needed

            Bitmap image = GetMeasurementFrame();
            List<Shapes.Circle> Circles = FindCirclesFunct(image);
            List<Shapes.Circle> GoodCircles= new  List<Shapes.Circle>();
            image.Dispose();

            double X = 0.0;
            double Y = 0.0;
            MaxDistance = MaxDistance * GetMeasurementZoom();
            double MaxS = MaxSize * GetMeasurementZoom();
            double MinS = MinSize * GetMeasurementZoom();
            // Remove those that are more than MaxDistance away from frame center
            foreach (Shapes.Circle Circle in Circles)
            {
                X = (Circle.X - FrameCenterX);
                Y = (Circle.Y - FrameCenterY);
                if ((X * X + Y * Y) <= (MaxDistance * MaxDistance))
                {
                    if (SizeLimited)
                    {
                        if ((Circle.Radius >= MinS) && (Circle.Radius <= MaxS))
                        {
                            GoodCircles.Add(Circle);
                        }
                    }
                    else
                    {
                        GoodCircles.Add(Circle);
                    }
                }
            }
            return (GoodCircles);
        }

        // =========================================================
        public int GetClosestCircle(out double X, out double Y, double MaxDistance)
        // Sets X, Y position of the closest circle to the frame center in pixels, return value is number of circles found
        {
            List<Shapes.Circle> Circles = GetMeasurementCircles(MaxDistance);
            X = 0.0;
            Y = 0.0;
            if (Circles.Count == 0)
            {
                return (0);
            }
            // Find the closest
            int closest = FindClosestCircle(Circles);
            double zoom = GetMeasurementZoom();
            X = (Circles[closest].X - FrameCenterX);
            Y = (Circles[closest].Y - FrameCenterY);
            X = X / zoom;
            Y = Y / zoom;
            return (Circles.Count);
        }

        // =========================================================
        public int GetSmallestCircle(out double X, out double Y, out double radius, double MaxDistance)
        // Sets X, Y position of the smallest circle to the frame center in pixels, return value is number of circles found
        {
            List<Shapes.Circle> Circles = GetMeasurementCircles(MaxDistance);
            X = 0.0;
            Y = 0.0;
            radius = 0.0;
            if (Circles.Count == 0)
            {
                return (0);
            }
            // Find the smallest
            int smallest = FindSmallestCircle(Circles);
            double zoom = GetMeasurementZoom();
            X = (Circles[smallest].X - FrameCenterX);
            Y = (Circles[smallest].Y - FrameCenterY);
            radius = Circles[smallest].Radius / zoom;
            X = X / zoom;
            Y = Y / zoom;
            return (Circles.Count);
        }

        // ==========================================================================================================
        // Rectangles:
        // ==========================================================================================================
        private List<Shapes.Rectangle> FindRectanglesFunct(Bitmap bitmap)
        {
            // step 1 - turn background to black (done)

            // step 2 - locating objects
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.FilterBlobs = true;
            blobCounter.MinHeight = 3;
            blobCounter.MinWidth = 3;
            blobCounter.ProcessImage(bitmap);
            Blob[] blobs = blobCounter.GetObjectsInformation();

            // step 3 - check objects' type and do what you do:
            List<Shapes.Rectangle> Rectangles = new List<Shapes.Rectangle>();

            for (int i = 0, n = blobs.Length; i < n; i++)
            {
                SimpleShapeChecker QuadChecker = new SimpleShapeChecker();
                List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(blobs[i]);
                List<IntPoint> cornerPoints;

                // fine tune ShapeChecker
                QuadChecker.AngleError = 7;  // default 7
                QuadChecker.LengthError = 0.1F;  // default 0.1 (10%)
                QuadChecker.MinAcceptableDistortion = 0.1F;  // in pixels, default 0.5 
                QuadChecker.RelativeDistortionLimit = 0.05F;  // default 0.03 (3%)

                // use the Outline checker to extract the corner points
                if (QuadChecker.IsQuadrilateral(edgePoints, out cornerPoints))
                {
                    // only do things if the corners form a rectangle
                    SimpleShapeChecker RectangleChecker = new SimpleShapeChecker();
                    RectangleChecker.AngleError = 7;  // default 7
                    RectangleChecker.LengthError = 0.00F;  // default 0.1 (10%)
                    RectangleChecker.MinAcceptableDistortion = 1F;  // in pixels, default 0.5 
                    RectangleChecker.RelativeDistortionLimit = 0.05F;  // default 0.03 (3%)
                    if (RectangleChecker.CheckPolygonSubType(cornerPoints) == PolygonSubType.Rectangle)
                    //if (true)
                    {
                        List<IntPoint> corners = PointsCloud.FindQuadrilateralCorners(edgePoints);
                        float x0 = corners[0].X;
                        float y0 = corners[0].Y;
                        float x2 = corners[2].X;
                        float y2 = corners[2].Y;
                        AForge.Point C = new AForge.Point();
                        C.X = (float)(((x2 - x0) / 2.0) + x0);
                        C.Y = (float)(((y2 - y0) / 2.0) + y0);

                        AForge.Point a1 = corners[0];
                        AForge.Point a2 = corners[1];
                        AForge.Point b1 = corners[0];
                        AForge.Point b2 = corners[1];
                        b2.Y = b1.Y;
                        float angle = GeometryTools.GetAngleBetweenLines(a1, a2, b1, b2);

                        Rectangles.Add(new Shapes.Rectangle(C, angle, corners));
                    }
                }
            }
            return (Rectangles);
        }
        // =========================================================
        private Bitmap DrawRectanglesFunct(Bitmap image)
        {
            List<Shapes.Rectangle> Rectangles = FindRectanglesFunct(image);
            int closest = FindClosestRectangle(Rectangles);
            Graphics g = Graphics.FromImage(image);
            Pen OrangePen = new Pen(Color.DarkOrange, 2);
            Pen LimePen = new Pen(Color.Lime, 2);

            for (int i = 0, n = Rectangles.Count; i < n; i++)
            {
                if (i==closest)
                {
                    g.DrawPolygon(LimePen, ToPointsArray(Rectangles[i].Corners));
                }
                else
                {
                    g.DrawPolygon(OrangePen, ToPointsArray(Rectangles[i].Corners));
                }
            }
            g.Dispose();
            LimePen.Dispose();
            OrangePen.Dispose();
            return (image);
        }

        // =========================================================
        private int FindClosestRectangle(List<Shapes.Rectangle> Rectangles)
        {
            if (Rectangles.Count==0)
            {
                return 0;
            }
            int closest = 0;
            float X = (Rectangles[0].Center.X - FrameCenterX);
            float Y = (Rectangles[0].Center.Y - FrameCenterY);
            float dist = X * X + Y * Y;  // we are interested only which one is closest, don't neet to take square roots to get distance right
            float dX, dY;
            for (int i = 0; i < Rectangles.Count; i++)
            {
                dX = Rectangles[i].Center.X - FrameCenterX;
                dY = Rectangles[i].Center.Y - FrameCenterY;
                if ((dX * dX + dY * dY) < dist)
                {
                    dist = dX * dX + dY * dY;
                    closest = i;
                }
            }
            return closest;
        }

        // =========================================================
        public int GetClosestRectangle(out double X, out double Y, double MaxDistance)
        // Sets X, Y position of the closest circle to the frame center in pixels, return value is number of circles found
        {
            List<Shapes.Rectangle> Rectangles = GetMeasurementRectangles(MaxDistance);
            X = 0.0;
            Y = 0.0;
            if (Rectangles.Count == 0)
            {
                return (0);
            }
            // Find the closest
            int closest = FindClosestRectangle(Rectangles);
            double zoom = GetMeasurementZoom();
            X = (Rectangles[closest].Center.X - FrameCenterX);
            Y = (Rectangles[closest].Center.Y - FrameCenterY);
            X = X / zoom;
            Y = Y / zoom;
            return (Rectangles.Count);
        }

        public List<Shapes.Rectangle> GetMeasurementRectangles(double MaxDistance)
        {
            // returns a list of circles for measurements, filters for distance (which we always do) and size, if needed

            Bitmap image = GetMeasurementFrame();
            List<Shapes.Rectangle> Rectangles = FindRectanglesFunct(image);
            List<Shapes.Rectangle> GoodRectangles = new  List<Shapes.Rectangle>();
            image.Dispose();

            double X = 0.0;
            double Y = 0.0;
            MaxDistance = MaxDistance * GetMeasurementZoom();
            double MaxS = MaxSize * GetMeasurementZoom();
            double MinS = MinSize * GetMeasurementZoom();
            // Remove those that are more than MaxDistance away from frame center
            foreach (Shapes.Rectangle Rectangle in Rectangles)
            {
                X = (Rectangle.Center.X - FrameCenterX);
                Y = (Rectangle.Center.Y - FrameCenterY);
                if ((X * X + Y * Y) <= (MaxDistance * MaxDistance))
                {
                    GoodRectangles.Add(Rectangle);
                }
            }
            return (GoodRectangles);
        }



        // ==========================================================================================================

        private Bitmap MirrorFunct(Bitmap frame)
		{
			Mirror Mfilter = new Mirror(false, true);
			// apply the MirrFilter
			Mfilter.ApplyInPlace(frame);
			return (frame);
		}


		// =========================================================
		private Bitmap TestAlgorithmFunct(Bitmap frame)
		{
			frame = Grayscale.CommonAlgorithms.RMY.Apply(frame);
			Invert filter = new Invert();
			filter.ApplyInPlace(frame);
			return (frame);
		}

		// =========================================================
		private void ZoomFunct(ref Bitmap frame, double Factor)
		{
			if (Factor < 0.1)
			{
				return;
			}
			int centerX = frame.Width / 2;
			int centerY = frame.Height / 2;
			int OrgSizeX = frame.Width;
			int OrgSizeY = frame.Height;

			int fromX = centerX - (int)(centerX / Factor);
			int fromY = centerY - (int)(centerY / Factor);
			int SizeX = (int)(OrgSizeX / Factor);
			int SizeY = (int)(OrgSizeY / Factor);
			Crop CrFilter = new Crop(new Rectangle(fromX, fromY, SizeX, SizeY));
			frame = CrFilter.Apply(frame);
			ResizeBilinear RBfilter = new ResizeBilinear(OrgSizeX, OrgSizeY);
			frame = RBfilter.Apply(frame);
		}

        // =========================================================
        private void RotateByFrameCenter(int x, int y, out int px, out int py)
        {
            double theta = boxRotationRad; 
            // If you rotate point (px, py) around point (ox, oy) by angle theta you'll get:
            // p'x = cos(theta) * (px-ox) - sin(theta) * (py-oy) + ox
            // p'y = sin(theta) * (px-ox) + cos(theta) * (py-oy) + oy
            px = (int)(Math.Cos(theta) * (x - FrameCenterX) - Math.Sin(theta) * (y - FrameCenterY) + FrameCenterX);
            py = (int)(Math.Sin(theta) * (x - FrameCenterX) + Math.Cos(theta) * (y - FrameCenterY) + FrameCenterY);
        }

        private void DrawGridFunct(Bitmap img)
        {
            Pen RedPen = new Pen(Color.Red, 1);
            Pen GreenPen = new Pen(Color.Green, 1);
            Pen BluePen = new Pen(Color.Blue, 1);
            Graphics g = Graphics.FromImage(img);
            int x1, x2, y1, y2;
            int step = 40;
            // vertical
            int i = 0;
            while (i< FrameSizeX)
            {
                // FrameCenterX, 0 to FrameCenterX, FrameSizeY
                RotateByFrameCenter(FrameCenterX + i, 0, out x1, out y1);
                RotateByFrameCenter(FrameCenterX + i, FrameSizeY, out x2, out y2);
                g.DrawLine(RedPen, x1, y1, x2, y2);
                RotateByFrameCenter(FrameCenterX - i, 0, out x1, out y1);
                RotateByFrameCenter(FrameCenterX - i, FrameSizeY, out x2, out y2);
                g.DrawLine(RedPen, x1, y1, x2, y2);
                i = i + step;
                RotateByFrameCenter(FrameCenterX + i, 0, out x1, out y1);
                RotateByFrameCenter(FrameCenterX + i, FrameSizeY, out x2, out y2);
                g.DrawLine(GreenPen, x1, y1, x2, y2);
                RotateByFrameCenter(FrameCenterX - i, 0, out x1, out y1);
                RotateByFrameCenter(FrameCenterX - i, FrameSizeY, out x2, out y2);
                g.DrawLine(GreenPen, x1, y1, x2, y2);
                i = i + step;
                RotateByFrameCenter(FrameCenterX + i, 0, out x1, out y1);
                RotateByFrameCenter(FrameCenterX + i, FrameSizeY, out x2, out y2);
                g.DrawLine(BluePen, x1, y1, x2, y2);
                RotateByFrameCenter(FrameCenterX - i, 0, out x1, out y1);
                RotateByFrameCenter(FrameCenterX - i, FrameSizeY, out x2, out y2);
                g.DrawLine(BluePen, x1, y1, x2, y2);
                i = i + step;
            }
            // horizontal
            i = 0;
            while (i < FrameSizeY)
            {
                // 0, FrameCenterY to FrameSizeX, FrameCenterY
                RotateByFrameCenter(0, FrameCenterY + i, out x1, out y1);
                RotateByFrameCenter(FrameSizeX, FrameCenterY + i, out x2, out y2);
                g.DrawLine(RedPen, x1, y1, x2, y2);
                RotateByFrameCenter(0, FrameCenterY - i, out x1, out y1);
                RotateByFrameCenter(FrameSizeX, FrameCenterY - i, out x2, out y2);
                g.DrawLine(RedPen, x1, y1, x2, y2);
                i = i + step;
                RotateByFrameCenter(0, FrameCenterY + i, out x1, out y1);
                RotateByFrameCenter(FrameSizeX, FrameCenterY + i, out x2, out y2);
                g.DrawLine(GreenPen, x1, y1, x2, y2);
                RotateByFrameCenter(0, FrameCenterY - i, out x1, out y1);
                RotateByFrameCenter(FrameSizeX, FrameCenterY - i, out x2, out y2);
                g.DrawLine(GreenPen, x1, y1, x2, y2);
                i = i + step;
                RotateByFrameCenter(0, FrameCenterY + i, out x1, out y1);
                RotateByFrameCenter(FrameSizeX, FrameCenterY + i, out x2, out y2);
                g.DrawLine(BluePen, x1, y1, x2, y2);
                RotateByFrameCenter(0, FrameCenterY - i, out x1, out y1);
                RotateByFrameCenter(FrameSizeX, FrameCenterY - i, out x2, out y2);
                g.DrawLine(BluePen, x1, y1, x2, y2);
                i = i + step;
            }

            RedPen.Dispose();
            GreenPen.Dispose();
            BluePen.Dispose();
            g.Dispose();
        }

        // =========================================================
        private void DrawDashedCrossFunct(Bitmap img)
        {
            Pen pen = new Pen(Color.SlateGray, 1);
			Graphics g = Graphics.FromImage(img);
			int step = FrameSizeY / 40;
			int i = step / 2;
			while (i < FrameSizeY)
			{
				g.DrawLine(pen, FrameCenterX, i, FrameCenterX, i + step);
				i = i + 2 * step;
			}
			step = FrameSizeX / 40;
			i = step / 2;
			while (i < FrameSizeX)
			{
				g.DrawLine(pen, i, FrameCenterY, i + step, FrameCenterY);
				i = i + 2 * step;
			}
            pen.Dispose();
            g.Dispose();
		}
		// =========================================================

		private void DrawCrossFunct(ref Bitmap img)
		{
			Pen pen = new Pen(Color.Red, 1);
			Graphics g = Graphics.FromImage(img);
			g.DrawLine(pen, FrameCenterX, 0, FrameCenterX, FrameSizeY);
			g.DrawLine(pen, 0, FrameCenterY, FrameSizeX, FrameCenterY);
            pen.Dispose();
            g.Dispose();
		}

		// =========================================================

		private void DrawSidemarksFunct(ref Bitmap img)
		{
			Pen pen = new Pen(Color.Red, 2);
			Graphics g = Graphics.FromImage(img);
			int Xinc = Convert.ToInt32(FrameSizeX / SideMarksX);
			int X = Xinc;
			int tick = 6;
			while (X<FrameSizeX)
			{
				g.DrawLine(pen, X, FrameSizeY, X, FrameSizeY - tick);
				g.DrawLine(pen, X, 0, X, tick);
				X += Xinc;
			}
			int Yinc = Convert.ToInt32(FrameSizeY / SideMarksY);
			int Y = Yinc;
			while (Y < FrameSizeY)
			{
				g.DrawLine(pen, FrameSizeX, Y, FrameSizeX - tick, Y);
				g.DrawLine(pen, 0, Y, tick, Y);
				Y += Yinc;
			}
            pen.Dispose();
            g.Dispose();
        }

		// =========================================================
		private void DrawBoxFunct(Bitmap img)
		{
			Pen pen = new Pen(Color.Red, 1);
			Graphics g = Graphics.FromImage(img);
			g.DrawLine(pen, BoxPoints[0].X + FrameCenterX, BoxPoints[0].Y + FrameCenterY,
				BoxPoints[1].X + FrameCenterX, BoxPoints[1].Y + FrameCenterY);

			g.DrawLine(pen, BoxPoints[1].X + FrameCenterX, BoxPoints[1].Y + FrameCenterY,
				BoxPoints[2].X + FrameCenterX, BoxPoints[2].Y + FrameCenterY);

			g.DrawLine(pen, BoxPoints[2].X + FrameCenterX, BoxPoints[2].Y + FrameCenterY,
				BoxPoints[3].X + FrameCenterX, BoxPoints[3].Y + FrameCenterY);

			g.DrawLine(pen, BoxPoints[3].X + FrameCenterX, BoxPoints[3].Y + FrameCenterY,
				BoxPoints[0].X + FrameCenterX, BoxPoints[0].Y + FrameCenterY);
            pen.Dispose();
            g.Dispose();
        }

		private void DrawArrowFunct(Bitmap img)
		{
			Pen pen = new Pen(Color.Blue, 3);
			Graphics g = Graphics.FromImage(img);
            double length= 60;
            double angle1 = (Math.PI / -180.0) * (ArrowAngle + 90); // to radians, -180 to get ccw, +90 to start from up
            double angle2 = (Math.PI / -180.0) * (ArrowAngle - 90); // to radians, -180 to get ccw, -90 to draw from center away
            //Draw end
            g.DrawLine(pen, FrameCenterX, FrameCenterY, (int)(FrameCenterX + Math.Cos(angle2) * length), (int)(FrameCenterY + Math.Sin(angle2) * length));
            // draw head
            System.Drawing.Drawing2D.AdjustableArrowCap bigArrow = new System.Drawing.Drawing2D.AdjustableArrowCap(6, 6);
            pen.CustomEndCap = bigArrow;
            g.DrawLine(pen, FrameCenterX, FrameCenterY, (int)(FrameCenterX + Math.Cos(angle1) * length), (int)(FrameCenterY + Math.Sin(angle1) * length));
            pen.Dispose();
            g.Dispose();
        }


		// =========================================================
		// Snapshot handling
		// =========================================================

		// repeated rotations destroy the image. We'll store the original here and rotate only once.
		public Bitmap SnapshotOriginalImage = new Bitmap(640, 480);

        // This is the image drawn by draw snapshot function (both public so they can be set externally).
		public Bitmap SnapshotImage = new Bitmap(640, 480);

		public double SnapshotRotation = 0.0;  // rotation when snapshot was taken

        public Color SnapshotColor { get; set; }

		public void TakeSnapshot()
		{
            Bitmap image = GetMeasurementFrame();

			Color peek;
			for (int y = 0; y < image.Height; y++)
			{
				for (int x = 0; x < image.Width; x++)
				{
					peek = image.GetPixel(x, y);
					if (peek.R != 0)  // i.e. background
					{
                        image.SetPixel(x, y, SnapshotColor);
					}
				}
			}

			image.MakeTransparent(Color.Black);
			SnapshotImage = new Bitmap(image);
			SnapshotOriginalImage = new Bitmap(image);
            image.Dispose();
		}



		// =========================================================
		public bool rotating = false;
		private bool overlaying = false;

		private Bitmap Draw_SnapshotFunct(Bitmap image)
		{
			if (rotating)
			{
				return (image);
			}
			overlaying = true;
			Graphics g = Graphics.FromImage(image);
			g.DrawImage(SnapshotImage, new System.Drawing.Point(0, 0));
			g.Dispose();
			overlaying = false;
			return (image);
		}

		// =========================================================
		public void RotateSnapshot(double deg)
		{
			while (overlaying)
			{
				Thread.Sleep(10);
			}
			rotating = true;
			// Convert to 24 bpp RGB Image
			Rectangle dimensions = new Rectangle(0, 0, SnapshotOriginalImage.Width, SnapshotOriginalImage.Height);
			Bitmap Snapshot24b = new Bitmap(SnapshotOriginalImage.Width, SnapshotOriginalImage.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			using (Graphics gr = Graphics.FromImage(Snapshot24b))
			{
				gr.DrawImage(SnapshotOriginalImage, dimensions);
                gr.Dispose();
			}

			RotateNearestNeighbor filter = new RotateNearestNeighbor(deg - SnapshotRotation, true);
			Snapshot24b = filter.Apply(Snapshot24b);
			// convert back to 32b, to have transparency
			Snapshot24b.MakeTransparent(Color.Black);
			SnapshotImage = Snapshot24b;
			rotating = false;
		}


	}
}

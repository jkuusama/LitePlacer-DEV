using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

using System.Drawing;
using System.Threading;
using System.Drawing.Imaging;
using System.Diagnostics;

using AForge;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;
using System.Drawing.Text;
using System.ComponentModel;
using System.Windows.Controls;

namespace LitePlacer
{
    public class Camera
    {
        // Program has been crashing due to access of ImageBox.  As a shared resource it needs
        // to be accessed in protected regions.  The lock _locker is to be used for this purpose.
        // The OnPaint method in PictureBox runs in a second task.  By extending the PictureBox
        // class, overriding the OnPaint method and putting the call into a region protected
        // by _locker we can stop the crashing.

#pragma warning disable CA1034 // Nested types should not be visible
        public class ProtectedPictureBox : System.Windows.Forms.PictureBox
#pragma warning restore CA1034 // Nested types should not be visible
        {
            protected override void OnPaint(PaintEventArgs e)
            {
                lock (_locker)
                {
                    base.OnPaint(e);
                }
            }

            // _locker must be a static variable to be available to the overridden OnPaint method
            public static object _locker { get; set; } = new object();
        }

        private VideoCaptureDevice VideoSource = null;
        private FormMain MainForm;
        public AppSettingsV3.CameraSettings Settings;

        public Camera(FormMain MainF, AppSettingsV3.CameraSettings cameraSettings)
        {
            MainForm = MainF;
            Settings = cameraSettings;
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
                MainForm.DisplayText("Stopping " + Id + ": " + Settings.Moniker);
                VideoSource.SignalToStop();
                VideoSource.WaitForStop();  // problem? 
                int i = 0;
                while (VideoSource.IsRunning && i<10)
                {
                    VideoSource.Stop();
                    Thread.Sleep(20);
                    Application.DoEvents();
                }
                if (i>=10)
                {
                    MainForm.DisplayText("*** "+ Id + " refuses to stop! ***" + Settings.Moniker, KnownColor.DarkRed, true);
                }
                VideoSource.NewFrame -= new NewFrameEventHandler(Video_NewFrame);
                VideoSource = null;
                Settings.Moniker = "Stopped";
            }
        }

        public void DisplayPropertyPage()
        {
            VideoSource.DisplayPropertyPage(IntPtr.Zero);

        }

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
                lock (ProtectedPictureBox._locker)
                {
                    _imageBox = value;
                }
            }
        }

        public int FrameCenterX { get; set; }
        public int FrameCenterY { get; set; }
        public int FrameSizeX { get; set; }
        public int FrameSizeY { get; set; }

        public string Id { get; set; } = "unconnected";

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
                        MainForm.DisplayText("X: " + source.VideoCapabilities[i].FrameSize.Width.ToString(CultureInfo.InvariantCulture) +
                            ", Y: " + source.VideoCapabilities[i].FrameSize.Height.ToString(CultureInfo.InvariantCulture));
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

                ////experimental: disable camera autosettings and set fixed values for consistent color and brightness
                object tempSourceObject = null;
                try
                {
                    tempSourceObject = FilterInfo.CreateFilter(MonikerStr);
                    if (tempSourceObject is AForge.Video.DirectShow.Internals.IAMVideoProcAmp)
                    {
                        AForge.Video.DirectShow.Internals.IAMVideoProcAmp pCamControl = (AForge.Video.DirectShow.Internals.IAMVideoProcAmp)tempSourceObject;
                        int hr = 0; VideoProcAmpFlags videoFlag = VideoProcAmpFlags.Manual;
                        int pMin = 0, pMax = 0, pSteppingDelta = 0, pDefault = 0;

                        //set Brightness to default
                        hr &= pCamControl.GetRange(VideoProcAmpProperty.Brightness, out pMin, out pMax, out pSteppingDelta,
                            out pDefault, out videoFlag);
                        hr &= pCamControl.Set(VideoProcAmpProperty.Brightness, pDefault, VideoProcAmpFlags.Manual);

                        //set whitebalance to max, due to cold LED-Ring-Light
                        hr &= pCamControl.GetRange(VideoProcAmpProperty.WhiteBalance, out pMin, out pMax, out pSteppingDelta,
                            out pDefault, out videoFlag);
                        hr &= pCamControl.Set(VideoProcAmpProperty.WhiteBalance, pMax, VideoProcAmpFlags.Manual);

                        //set sharpness to min, for less noise and smoother picture
                        hr &= pCamControl.GetRange(VideoProcAmpProperty.Sharpness, out pMin, out pMax, out pSteppingDelta,
                            out pDefault, out videoFlag);
                        hr &= pCamControl.Set(VideoProcAmpProperty.Sharpness, pMin, VideoProcAmpFlags.Manual);

                        if (hr != 0)
                            MainForm.DisplayText("video settings not adjusted: " + hr.ToString());
                        else
                            MainForm.DisplayText("video settings adjusted");
                    }
                    else
                        MainForm.DisplayText("Camera doesnt support video settings");
                }
                catch
                {
                    MainForm.DisplayText("Failed creating device object for video settings");
                }
                if (tempSourceObject != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(tempSourceObject);
                ////: experimental
                
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
                if (tries >= 4)
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
                    if ((VideoSource.VideoCapabilities[i].FrameSize.Width == Settings.DesiredX)
                        &&
                        (VideoSource.VideoCapabilities[i].FrameSize.Height == Settings.DesiredY))
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
                    if (VideoSource==null)
                    {
                        break;      // this can happen if changing tabs too fast. TODO: True guard for operations during camera switch/start
                    }
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
                MainForm.DisplayText("*** Camera started: " + tries.ToString(CultureInfo.InvariantCulture), KnownColor.Purple);

                // We managed to start the camera using desired resolution
                FrameSizeX = Settings.DesiredX;
                FrameSizeY = Settings.DesiredY;
                FrameCenterX = FrameSizeX / 2;
                FrameCenterY = FrameSizeY / 2;
                PauseProcessing = false;
                return true;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (System.Exception excep)
            {
                MessageBox.Show(excep.Message);
                return false;
            }
#pragma warning restore CA1031 // Do not catch general exception types
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


        // ==========================================================================================================
        // Convert from UI functions (AForgeFunctionDefinition) to processing functions (AForgeFunction)

        private List<AForgeFunction> BuildFunctionsList(List<AForgeFunctionDefinition> UiList)
        {
            List<AForgeFunction> NewList = new List<AForgeFunction>();
            NewList.Clear();
            MainForm.DisplayText("BuildFunctionsList: ");
            foreach (AForgeFunctionDefinition UIfucnt in UiList)
            {
                AForgeFunction f = new AForgeFunction();
                // skip inactive rows
                if (UIfucnt == null)
                {
                    continue;
                }
                if (!UIfucnt.Active)
                {
                    continue;
                }
                switch (UIfucnt.Name)
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

                    case "Erosion":
                        f.func = ErosionFunct;
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

                    case "Hough circles":
                        f.func = HoughCirclesFunct; 
                        break;

                    default:
                        continue;
                        // break; 
                }
                f.parameter_int = UIfucnt.parameterInt;
                f.parameter_double = UIfucnt.parameterDouble;
                f.R = UIfucnt.R;
                f.B = UIfucnt.B;
                f.G = UIfucnt.G;
                NewList.Add(f);
                MainForm.DisplayText(UIfucnt.Name + ", " + f.parameter_int.ToString() + ", " + f.parameter_double.ToString() + ", "
                    + f.R.ToString() + ", " + f.G.ToString() + ", " + f.B.ToString());
            };
            return NewList;
        }

        // ==========================================================================================================
        // For display: Get the function list, transfer to video processing

        public void BuildDisplayFunctionsList(List<AForgeFunctionDefinition> UiList)
        {
            List<AForgeFunction> NewList = BuildFunctionsList(UiList);    // Get the list
            int tries = 0;
            // Stop video
            bool pause = PauseProcessing;
            if (VideoSource != null)
            {
                if (ReceivingFrames)
                {
                    // stop video
                    PauseProcessing = true;  // ask for stop
                    Paused = false;
                    while (!Paused)
                    {
                        Thread.Sleep(10);  // wait until really stopped
                        tries++;
                        if (tries > 40)
                        {
                            break;
                        }
                    };
                }
            }
            // copy new list
            DisplayFunctions.Clear();
            DisplayFunctions = NewList;
            Thread.Sleep(50);  // wait until really stopped
            PauseProcessing = pause;  // restart video is it was running
        }

        public void ClearDisplayFunctionsList()
        {
            if (DisplayFunctions.Count == 0)
            {
                return;
            }
            // Stop video
            bool pause = PauseProcessing;
            int tries = 0;
            if (VideoSource != null)
            {
                if (ReceivingFrames)
                {
                    // stop video
                    PauseProcessing = true;  // ask for stop
                    Paused = false;
                    while (!Paused)
                    {
                        Thread.Sleep(10);  // wait until really stopped
                        tries++;
                        if (tries>40)
                        {
                            break;
                        }
                    };
                }
            }
            DisplayFunctions.Clear();
            PauseProcessing = pause;  // restart video is it was running
        }

        // ==========================================================================================================
        // Members we need for our drawing functions
        // ==========================================================================================================

        // ==========================================================================================================

        public int Threshold { get; set; }                  // Threshold for all the "draw" functions
        public bool GrayScale { get; set; }                 // If image is converted to grayscale 
        public bool Invert { get; set; }                    // If image is inverted (makes most sense on grayscale, looking for black stuff on light background)
        public bool DrawArrow { get; set; }         // If arrow is drawn
        public bool Overlay { get; set; }           // overlay process picture with original
        public double ArrowAngle { get; set; }      // to which angle
        public double SideMarksX { get; set; }      // How many marks on top and bottom (X) and sides (Y)
        public double SideMarksY { get; set; }      // (double, so you can do "SidemarksX= workarea_in_mm / 100;" to get mark every 10cm
        public bool DrawDashedCross { get; set; }   // If a dashed crosshaircursor is drawn (so that the center remains visible)
        public bool FindCircles { get; set; }       // Find and highlight circles in the image
        public bool FindRectangles { get; set; }    // Find and draw regtangles in the image
        public bool FindComponent { get; set; }     // Finds a component and identifies its center
        public bool Draw_Snapshot { get; set; }     // Draws the snapshot on the image 
        public bool PauseProcessing { get; set; }   // Drawing the video slows everything down. This can pause it for measurements.
        public bool Paused = true;                 // set in video processing indicating it is safe to change processing function list
        public bool TestAlgorithm { get; set; }

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
        private System.Drawing.Point[] ToPointsArray(List<AForge.Point> points)
        {
            System.Drawing.Point[] array = new System.Drawing.Point[points.Count];

            for (int i = 0, n = points.Count; i < n; i++)
            {
                array[i] = new System.Drawing.Point((int)points[i].X, (int)points[i].Y);
            }

            return array;
        }


        // ==========================================================================================================
        // ==========================================================================================================

        // Each frame goes through Video_NewFrame()

        // ==========================================================================================================
        // ==========================================================================================================

        volatile int CollectorCount = 0;
        //manually keeping track on how many threads are running, so Threadpool doesnt queue up to many
        volatile int runningThreads = 0;
        //maximum number of Threads set to number of logical Cores
        readonly int maxThreads = Environment.ProcessorCount;
        //if Threadpool fails, fall back to singlethread
        bool multithreaded = true;
        BackgroundWorker backgroundWorker;

        private void Video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            ReceivingFrames = true;

            //Picture origin is up left, machine origin is down left
            //We mirror the picture origin to machine origin
            if (!Settings.MirrorX || !Settings.MirrorY)
            {
                Mirror Mfilter = new Mirror(!Settings.MirrorX, Settings.MirrorY);
                // apply the MirrFilter
                Mfilter.ApplyInPlace(eventArgs.Frame);
            };

            // Take a copy for measurements, if needed:
            if (CopyFrame)
            {
                if (secondFrame)
                { 
                    if (TemporaryFrame != null)
                    {
                        TemporaryFrame.Dispose();
                    }
                    TemporaryFrame = (Bitmap)eventArgs.Frame.Clone();
                    CopyFrame = false;
                    secondFrame = false;
                }
                else
                {
                    secondFrame = true;
                }
            };

            if (PauseProcessing)
            {
                Paused = true;
                return;
            };

            if (!Active)
            {
                return;
            }

            // Working with a copy of the frame to avoid conflict.  Protecting the region where the copy is made
            /*lock (ProtectedPictureBox._locker)
            {*/
                //do multithread by default, fall back to singlethread if Threadpool fails
                //new frames get dropped if maximum processing capacity is reached
                if (multithreaded)
                {
                    try
                    {
                        if (runningThreads < maxThreads)
                        {
                            runningThreads++;
                            ThreadPool.QueueUserWorkItem(processNewFrame, eventArgs.Frame.Clone());
                        }
                    }
                    catch (Exception)
                    {
                        multithreaded = false;
                    }
                }
                else
                {
                    if (backgroundWorker == null)
                    {
                        backgroundWorker = new BackgroundWorker();
                        backgroundWorker.DoWork += new DoWorkEventHandler(processNewFrame);
                    }
                    if (!backgroundWorker.IsBusy)
                        backgroundWorker.RunWorkerAsync(eventArgs.Frame.Clone());
                }                
            //}
        }
        private void processNewFrame(object sender, DoWorkEventArgs e) { processNewFrame(e.Argument); }
        private void processNewFrame(object frameObject)
        {
            Bitmap frame = (Bitmap)frameObject;

            if (Settings.Zoom)
            {
                ZoomFunct(ref frame, Settings.ZoomFactor);
            };

            Bitmap origFrame = null;
            if (Overlay)
            {
                origFrame = (Bitmap)frame.Clone();
            }

            // Even with safeguards, changing DisplayFunctions can cause errors. 
            try
            {
                if (DisplayFunctions != null)
                {
                    foreach (AForgeFunction f in DisplayFunctions)
                    {
                        if(f.func == Meas_ZoomFunc && origFrame != null)
                            f.func(ref origFrame, f.parameter_int, f.parameter_double, f.R, f.G, f.B);
                        f.func(ref frame, f.parameter_int, f.parameter_double, f.R, f.G, f.B);
                    }

                    if (FindCircles)
                    {
                        DrawCirclesFunct(frame);
                    }
                    if (FindRectangles)
                    {
                        frame = DrawRectanglesFunct(frame);
                    }
                    if (FindComponent)
                    {
                        frame = DrawComponentsFunct(frame);
                    }
                }
            }
            catch (System.InvalidOperationException)
            {
                // No need to do anything, next frame fixes it
            }

            if (Settings.DrawBox)
            {
                DrawBoxFunct(ref frame);
            };

            if (Settings.DrawGrid)
            {
                DrawGridFunct(frame);
            };

            if (Settings.DrawCross)
            {
                DrawCrossFunct(ref frame);
            };

            if (Settings.DrawSidemarks)
            {
                DrawSidemarksFunct(ref frame);
            };

            if (DrawDashedCross)
            {
                DrawDashedCrossFunct(frame);
            };

            if (DrawArrow)
            {
                DrawArrowFunct(frame);
            };

            if (Overlay && origFrame != null)
            {
                using (Graphics bitmapGraphics = Graphics.FromImage(origFrame))
                {
                    ColorMatrix matrix = new ColorMatrix();
                    matrix.Matrix33 = 128.0f / 255.0f;
                    ImageAttributes attributes = new ImageAttributes();
                    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                    bitmapGraphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                    bitmapGraphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.GammaCorrected;

                    bitmapGraphics.DrawImage(frame, new Rectangle(0, 0, frame.Width, frame.Height), 0, 0, frame.Width, frame.Height, GraphicsUnit.Pixel, attributes);
                    frame.Dispose();
                    frame = origFrame;
                }
            }

            //Mirror Y for Display only
            Mirror Mfilter = new Mirror(true, false);
            Mfilter.ApplyInPlace(frame);
            lock (ProtectedPictureBox._locker)
            {
                if (ImageBox.Image != null)
                {
                    ImageBox.Image.Dispose();
                }
                ImageBox.Image = (Bitmap)frame.Clone();
            }

            frame.Dispose();
            if (CollectorCount > 20)
            {
                GC.Collect();
                CollectorCount = 0;
            }
            else
            {
                CollectorCount++;
            }
            runningThreads--;
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
        private void ErosionFunct(ref Bitmap frame, int par_int, double par_d, int par_R, int par_G, int par_B)
        {
            Erosion filter = new Erosion();
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

        private void HoughCirclesFunct(ref Bitmap frame, int par_int, double par_d, int par_R, int par_G, int par_B)
        {
            HoughCircleTransformation circleTransform = new HoughCircleTransformation(2* par_int);
            // apply Hough circle transform
            frame = Grayscale.CommonAlgorithms.RMY.Apply(frame);    // Make gray
            circleTransform.ProcessImage(frame);
            frame = circleTransform.ToBitmap();
            GrayscaleToRGB RGBfilter = new GrayscaleToRGB();    // back to color format
            frame = RGBfilter.Apply(frame);

            // get circles using relative intensity
            //HoughCircle[] circles = circleTransform.GetCirclesByRelativeIntensity(par_d);
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


        // =========================================================
        private void GrayscaleFunc(ref Bitmap frame, int par_int, double par_d, int par_R, int par_G, int par_B)
        {
            Grayscale toGrFilter = new Grayscale(0.2125, 0.7154, 0.0721);       // create grayscale MirrFilter (BT709)
            Bitmap fr = toGrFilter.Apply(frame);
            GrayscaleToRGB toColFilter = new GrayscaleToRGB();
            frame = toColFilter.Apply(fr);
        }

        // =========================================================
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
        // Components, newer version:
        // ==========================================================================================================
        // helpers:

        // ===========
        private List<IntPoint> GetBlobsOutline(BlobCounter blobCounter, Blob blob)
        {
            List<IntPoint> leftPoints, rightPoints, edgePoints = new List<IntPoint>();
            // get blob's edge points
            blobCounter.GetBlobsLeftAndRightEdges(blob,
                out leftPoints, out rightPoints);

            edgePoints.AddRange(leftPoints);
            edgePoints.AddRange(rightPoints);
            return edgePoints;
        }

        // ===========
        private List<IntPoint> GetConvexHull(List<IntPoint> edgePoints)
        {
            // create convex hull searching algorithm
            GrahamConvexHull hullFinder = new GrahamConvexHull();
            ClosePointsMergingOptimizer optimizer1 = new ClosePointsMergingOptimizer();
            FlatAnglesOptimizer optimizer2 = new FlatAnglesOptimizer();
            List<IntPoint> Outline = hullFinder.FindHull(edgePoints);
            optimizer1.MaxDistanceToMerge = 4;
            optimizer2.MaxAngleToKeep = 170F;
            Outline = optimizer2.OptimizeShape(Outline);
            Outline = optimizer1.OptimizeShape(Outline);
            if (Outline.Count < 3)
            {
                return null;
            }
            return Outline;
        }

        // ===========
        private double GetAngle(IntPoint p1, IntPoint p2)
        {
            // returns the angle between line (p1,p2) and horizontal axis, in degrees
            double A = 0;
            if (p2.X == p1.X)
            {
                if (p2.Y > p1.Y)
                {
                    return 90;
                }
                else
                {
                    return 270;
                }
            }
            else
            {
                A = Math.Atan(Math.Abs((double)(p1.Y - p2.Y) / (double)(p1.X - p2.X)));
                A = A * 180.0 / Math.PI; // in deg.
            }
            // quadrants: A is now first quadrant solution
            if ((p1.X < p2.X) && (p1.Y <= p2.Y))
            {
                return A;   // 1st q.
            }
            else if ((p1.X > p2.X) && (p1.Y < p2.Y))
            {
                return A + 90; // 2nd q.
            }
            else if ((p1.X > p2.X) && (p1.Y >= p2.Y))
            {
                return A + 180; // 3rd q.
            }
            else
            {
                return 360 - A; // 4th q.
            }
        }

        // ===========
        private double RectangleArea(IntPoint p1, IntPoint p2)
        {
            return Math.Abs(((double)p2.X - (double)p1.X) * ((double)p2.Y - (double)p1.Y));
        }

        // ===========
        private List<IntPoint> ScaleOutline(double scale, List<IntPoint> Outline)
        {
            List<IntPoint> Result = new List<IntPoint>();
            foreach (var p in Outline)
            {
                Result.Add(new IntPoint((int)(p.X * scale), (int)(p.Y * scale)));
            }
            return Result;
        }
        // ===========
        private List<AForge.Point> ScaleOutline(double scale, List<AForge.Point> Outline)
        {
            List<AForge.Point> Result = new List<AForge.Point>();
            foreach (var p in Outline)
            {
                Result.Add(new AForge.Point((float)(p.X * scale), (float)(p.Y * scale)));
            }
            return Result;
        }
        // ===========
        private IntPoint RotatePoint(double angle, IntPoint p, IntPoint o)
        {
            // TODO: put all rotations in one routine

            // If you rotate point (px, py) around point (ox, oy) by angle theta you'll get:
            // p'x = cos(theta) * (px-ox) - sin(theta) * (py-oy) + ox
            // p'y = sin(theta) * (px-ox) + cos(theta) * (py-oy) + oy

            double theta = angle * (Math.PI / 180.0); // to radians
            IntPoint Pout = new IntPoint();
            Pout.X = Convert.ToInt32(Math.Cos(theta) * (p.X - o.X) - Math.Sin(theta) * (p.Y - o.Y) + o.X);
            Pout.Y = Convert.ToInt32(Math.Sin(theta) * (p.X - o.X) + Math.Cos(theta) * (p.Y - o.Y) + o.Y);
            return Pout;
        }

        // ===========
        private List<IntPoint> RotateOutline(double theta, List<IntPoint> Outline, IntPoint RotOrigin)
        {
            // returns the outline, rotated by angle around RotOrigin
            List<IntPoint> Result = new List<IntPoint>();
            IntPoint Pout = new IntPoint();
            foreach (var p in Outline)
            {
                Pout = RotatePoint(theta, p, RotOrigin);
                Result.Add(Pout);
            }
            return Result;
        }

        // ===========
        private List<IntPoint> GetMinimumBoundingRectangle(List<IntPoint> Outline)
        {
            // Using caliber algorithm to find the minimum bounding regtangle
            // (see http://www.datagenetics.com/blog/march12014/index.html):

            // For each line segment,
            // Rotate the hull so that the segment is horizontal.
            // Get the hull bounding rectangle
            // Measure the area
            // If first iteration or the area is smaller than the previous one,
            // store the area, the rectangle and the rotation with point
            // The solution is the stored rectangle, rotated back to original orientation.

            bool first = true;
            IntPoint minXY = new IntPoint();
            IntPoint maxXY = new IntPoint();
            double SmallestArea = 0;
            double AngleOfsolution = 0;
            IntPoint SolutionMin = new IntPoint();
            IntPoint SolutionMax = new IntPoint();
            IntPoint SolutionOrg = new IntPoint();

            for (int i = 1; i < Outline.Count; i++) // For each line segment,
            {
                double angle = GetAngle(Outline[i - 1], Outline[i]);
                List<IntPoint> RotatedOutline = RotateOutline(-angle, Outline, Outline[i - 1]); // Rotate the hull so that the segment is horizontal.
                PointsCloud.GetBoundingRectangle(RotatedOutline, out minXY, out maxXY);    // Get the hull bounding rectangle
                double area = RectangleArea(minXY, maxXY);
                if (first || (area < SmallestArea))
                {
                    SmallestArea = area;        // store the area, the rectangle and the rotation
                    AngleOfsolution = angle;
                    SolutionMin = minXY;
                    SolutionMax = maxXY;
                    SolutionOrg = Outline[i - 1];
                    first = false;
                }
            }
            List<IntPoint> rect = new List<IntPoint>() { SolutionMin, new IntPoint(SolutionMax.X, SolutionMin.Y),
                SolutionMax ,new IntPoint(SolutionMin.X, SolutionMax.Y) };
            rect = RotateOutline(AngleOfsolution, rect, SolutionOrg); // stored rectangle, rotated back to original orientation and place
            return rect;
        }

        // ===========
        private List<Shapes.Component> FindComponentsFunct(Bitmap bitmap)
        {
            // Locating objects
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.FilterBlobs = true;
            blobCounter.MinHeight = 8;
            blobCounter.MinWidth = 8;
            blobCounter.ProcessImage(bitmap);
            List<Shapes.Component> Components = new List<Shapes.Component>();

            Blob[] blobs = blobCounter.GetObjectsInformation(); // Get blobs
            foreach (Blob blob in blobs)
            {
                if ((blob.Rectangle.Height > (bitmap.Height - 10)) && (blob.Rectangle.Width > (bitmap.Width - 10)))
                {
                    continue;  // The whole image could be a blob, discard that
                }

                List<IntPoint> edgePoints = GetBlobsOutline(blobCounter, blob);     // get edge points
                List<IntPoint> OutlineRaw = GetConvexHull(edgePoints);                 // convert to convex hull
                if (OutlineRaw.Count < 3)
                {
                    continue;
                }
                
                // We are dealing with small objects, one pixel is coarse. Therefore, all calculations are done
                // on a scaled outline, scaling back after finding the rectangle. (This is because AForge uses IntPoints.)
                List<IntPoint> Outline = OutlineRaw;

                // add start point to list, so we get line segments
                Outline.Add(Outline[0]);
                List<AForge.Point> Box = GetMinimumBoundingRectangle(Outline).Select(s => (AForge.Point)s).ToList();    // get bounding rectangle

                //workaround for correct and consistent angle
                List<AForge.Point> anglePoints = OutlineRaw.Select(s => (AForge.Point)s).ToList();

                double angle = 0;
                List<double[]> angle_length = new List<double[]>(anglePoints.Count);
                for (int i = 0, j = 1; i < (anglePoints.Count - 0); i++, j = j < (anglePoints.Count - 1) ? j + 1 : 0)
                {
                    double tmpAngle;
                    if (anglePoints[i].X == anglePoints[j].X)
                    {
                        tmpAngle = 0;
                    }
                    else if (anglePoints[i].X < anglePoints[j].X)
                    {
                        tmpAngle = Math.Atan2(anglePoints[j].Y - anglePoints[i].Y, anglePoints[j].X - anglePoints[i].X) / Math.PI * 180;
                    }
                    else
                    {
                        tmpAngle = Math.Atan2(anglePoints[i].Y - anglePoints[j].Y, anglePoints[i].X - anglePoints[j].X) / Math.PI * 180;
                    }

                    while (tmpAngle >= 45)
                        tmpAngle -= 90;
                    while (tmpAngle <= -45)
                        tmpAngle += 90;

                    angle_length.Add(new double[] { tmpAngle, anglePoints[i].DistanceTo(anglePoints[j]) });
                }
                //average angles of 4 longest sides
                angle_length = angle_length.OrderByDescending(s => s[1]).ToList();
                for (int i = 0; i < 4; i++)
                {
                    angle += angle_length[i][0];
                }
                angle = angle / 4.0;

                //Box= ScaleOutline(0.001, Box);  // scale back
                if (Box.Count != 4)
                {
                    MainForm.DisplayText("Rectangle with " + Box.Count.ToString() + "corners (BoxToComponent)", KnownColor.Red, true);
                }
                else
                {
                    Components.Add(new Shapes.Component(Box, blob.CenterOfGravity, angle));
                }
            }
            return Components;
        }

        // ===========
        private List<Shapes.Component> FindComponentsNewFunct(Bitmap bitmap)
        {
            // Locating objects
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.FilterBlobs = true;
            blobCounter.MinHeight = 8;
            blobCounter.MinWidth = 8;
            blobCounter.ProcessImage(bitmap);
            List<Shapes.Component> Components = new List<Shapes.Component>();

            Blob[] blobs = blobCounter.GetObjectsInformation(); // Get blobs
            foreach (Blob blob in blobs)
            {
                if ((blob.Rectangle.Height > (bitmap.Height - 10)) && (blob.Rectangle.Width > (bitmap.Width - 10)))
                {
                    continue;  // The whole image could be a blob, discard that
                }

                List<IntPoint> edgePoints = GetBlobsOutline(blobCounter, blob);     // get edge points
                List<IntPoint> OutlineRaw = GetConvexHull(edgePoints);                 // convert to convex hull
                if (OutlineRaw.Count < 3)
                {
                    continue;
                }

                // We are dealing with small objects, one pixel is coarse. Therefore, all calculations are done
                // on a scaled outline, scaling back after finding the rectangle. (This is because AForge uses IntPoints.)
                List<IntPoint> Outline = OutlineRaw;

                // add start point to list, so we get line segments
                Outline.Add(Outline[0]);
                List<AForge.Point> Box = GetMinimumBoundingRectangle(Outline).Select(s => (AForge.Point)s).ToList();    // get bounding rectangle

                //workaround for correct and consistent angle
                List<AForge.Point> anglePoints = OutlineRaw.Select(s => (AForge.Point)s).ToList();

                double angle = 0;
                List<double[]> angle_length = new List<double[]>(anglePoints.Count);
                for (int i = 0, j = 1; i < (anglePoints.Count - 0); i++, j = j < (anglePoints.Count - 1) ? j + 1 : 0)
                {
                    double tmpAngle;
                    if (anglePoints[i].X == anglePoints[j].X)
                    {
                        tmpAngle = 0;
                    }
                    else if (anglePoints[i].X < anglePoints[j].X)
                    {
                        tmpAngle = Math.Atan2(anglePoints[j].Y - anglePoints[i].Y, anglePoints[j].X - anglePoints[i].X) / Math.PI * 180;
                    }
                    else
                    {
                        tmpAngle = Math.Atan2(anglePoints[i].Y - anglePoints[j].Y, anglePoints[i].X - anglePoints[j].X) / Math.PI * 180;
                    }

                    while (tmpAngle >= 45)
                        tmpAngle -= 90;
                    while (tmpAngle <= -45)
                        tmpAngle += 90;

                    angle_length.Add(new double[] { tmpAngle, anglePoints[i].DistanceTo(anglePoints[j]) });
                }
                //average angles of 4 longest sides
                angle_length = angle_length.OrderByDescending(s => s[1]).ToList();
                for (int i = 0; i < 4; i++)
                {
                    angle += angle_length[i][0];
                }
                angle = angle / 4.0;

                //Box= ScaleOutline(0.001, Box);  // scale back
                if (Box.Count != 4)
                {
                    MainForm.DisplayText("Rectangle with " + Box.Count.ToString() + "corners (BoxToComponent)", KnownColor.Red, true);
                }
                else
                {
                    Components.Add(new Shapes.Component(Box, blob.CenterOfGravity, angle));
                }
            }
            return Components;
        }

        // ===========
        private Bitmap DrawComponentsFunct(Bitmap bitmap)
        {
            List<Shapes.Component> Components = FindComponentsFunct(bitmap);
            Graphics g = Graphics.FromImage(bitmap);
            Pen OrangePen = new Pen(Color.DarkOrange, 3);
            double zoom = GetMeasurementZoom();
            double XmmPpix = Settings.XmmPerPixel / zoom;
            double YmmPpix = Settings.YmmPerPixel / zoom;
            for (int i = Components.Count-1; i >= 0; i--)
            {
                Shapes.Component component = Components[i];
                if (((component.BoundingBox.LongsideLenght * XmmPpix) <= MeasurementParameters.Xmin) ||
                      ((component.BoundingBox.LongsideLenght * XmmPpix) >= MeasurementParameters.Xmax) ||
                      ((component.BoundingBox.ShortSideLenght * YmmPpix) <= MeasurementParameters.Ymin) ||
                      ((component.BoundingBox.ShortSideLenght * YmmPpix) >= MeasurementParameters.Ymax))

                {
                    Components.Remove(component);
                }
            }

            if(Components.Count == 1)
            {
                Shapes.Component component = Components[0];
                MainForm.DisplayBigText( string.Format("{0,5:0.000}", (component.BoundingBox.Center.X - FrameCenterX) * XmmPpix) + "  " + string.Format("{0,5:0.000}", (component.BoundingBox.Center.Y - FrameCenterY) * YmmPpix) +
                    "  " + string.Format("{0,5:0.00}", component.BoundingBox.Angle));
            }
            else
            {
                MainForm.DisplayBigText("No Unique Position");
            }

            foreach (Shapes.Component component in Components)
            {
                g.DrawPolygon(OrangePen, ToPointsArray(component.BoundingBox.Corners));
                int crossSize = 300;
                AForge.Point center = component.BoundingBox.Center;
                g.DrawLine(OrangePen, center.X - crossSize, center.Y, center.X + crossSize, center.Y);
                g.DrawLine(OrangePen, center.X, center.Y - crossSize, center.X, center.Y + crossSize);
            }
            
            g.Dispose();
            OrangePen.Dispose();
            return (bitmap);
        }

        // ==========================================================================================================
        // Components, older version:
        // ==========================================================================================================
        private List<Shapes.LitePlacerShapeComponent> FindComponentsFunctOld(Bitmap bitmap)
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

            List<Shapes.LitePlacerShapeComponent> Components = new List<Shapes.LitePlacerShapeComponent>();

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
                NormalStart.X = Longest.Start.Y - LongestCenter.Y + LongestCenter.X;
                NormalStart.Y = Longest.Start.X - LongestCenter.X + LongestCenter.Y;
                // cos-90=0, sin-90= -1 =>
                // p'x= (py-oy) + ox
                // p'y= -(px-ox)+oy= ox-px+oy
                NormalEnd.X = LongestCenter.Y - Longest.Start.Y + LongestCenter.X;
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
                    Components.Add(new Shapes.LitePlacerShapeComponent(ComponentCenter, Alignment, Outline, Longest, NormalStart, NormalEnd));
                }
            }
            return Components;
        }

        public List<Shapes.Component> GetMeasurementComponents()
        {
            // No filtering! (tech. debt, maybe)
            Bitmap image = GetMeasurementFrame();
            List<Shapes.Component> Components = FindComponentsFunct(image);
            image.Dispose();
            return Components;
        }


        public int GetClosestComponent(out double X, out double Y, out double A, double MaxDistance)
        // Sets X, Y position of the closest component to the frame center in pixels, 
        // A to rotation in degrees, 
        // return value is number of components found
        {
            Bitmap image = GetMeasurementFrame();
            List<Shapes.LitePlacerShapeComponent> RawComponents = FindComponentsFunctOld(image);
            image.Dispose();
            List<Shapes.LitePlacerShapeComponent> GoodComponents = new List<Shapes.LitePlacerShapeComponent>();

            X = 0.0;
            Y = 0.0;
            A = 0.0;
            if (RawComponents.Count == 0)
            {
                return (0);
            }
            // Remove those that are more than MaxDistance away from frame center
            foreach (Shapes.LitePlacerShapeComponent Component in RawComponents)
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
            A = GoodComponents[0].Angle;
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
                    A = GoodComponents[i].Angle;
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
                    if (radius > 2)  // Filter out some noise
                    {
                        double dRadius = radius;
                        AForge.DoublePoint dCenter = center;
                        CircleOptimizer circleOptimizer = new CircleOptimizer();
                        circleOptimizer.optimize(edgePoints, ref dCenter, ref dRadius);
                        Circles.Add(new Shapes.Circle((AForge.Point)dCenter, dRadius));
                    }
                }
            }
            return (Circles);
        }

        // =========================================================
        private void DrawCirclesFunct(Bitmap bitmap)
        {
            List<Shapes.Circle> Circles = FindCirclesFunct(bitmap);
            double zoom = GetMeasurementZoom();
            double XmmPpix = Settings.XmmPerPixel / zoom;
            double YmmPpix = Settings.YmmPerPixel / zoom;
            for (int i = Circles.Count - 1; i >= 0; i--)
            {
                Shapes.Circle circle = Circles[i];
                if (((circle.Radius * 2.0 * XmmPpix) <= MeasurementParameters.Xmin) ||
                      ((circle.Radius * 2.0 * XmmPpix) >= MeasurementParameters.Xmax) ||
                      ((circle.Radius * 2.0 * YmmPpix) <= MeasurementParameters.Ymin) ||
                      ((circle.Radius * 2.0 * YmmPpix) >= MeasurementParameters.Ymax))

                {
                    Circles.Remove(circle);
                }
            }

            if (Circles.Count == 1)
            {
                Shapes.Circle circle = Circles[0];
                MainForm.DisplayBigText(string.Format("{0,5:0.000}", (circle.Center.X - FrameCenterX) * XmmPpix) + "  " + string.Format("{0,5:0.000}", (circle.Center.Y - FrameCenterY) * YmmPpix));
            }
            else
            {
                MainForm.DisplayBigText("No Unique Position");
            }

            if (Circles.Count == 0)
            {
                return;
            }
            // Find smallest
            int Smallest = FindSmallestCircle(Circles);

            // find closest to center
            int Closest = FindClosestCircle(Circles);

            int PenSize = 3;
            // if show pixels is off and image is not zoomed, the drawn pixels are going to be scaled down.
            // To make the circles visible, we need to draw then bit thicker
            if (!(ImageBox.SizeMode == PictureBoxSizeMode.CenterImage) && !Settings.Zoom)
            {
                PenSize = 5;
            }

            Graphics g = Graphics.FromImage(bitmap);
            Pen OrangePen = new Pen(Color.DarkOrange, PenSize);
            Pen AquaPen = new Pen(Color.Aqua, PenSize);
            Pen LimePen = new Pen(Color.Lime, PenSize);
            Pen MagentaPen = new Pen(Color.Magenta, PenSize);

            if (Closest == Smallest)
            {
                g.DrawEllipse(MagentaPen,
                    (float)(Circles[Closest].Center.X - Circles[Closest].Radius), (float)(Circles[Closest].Center.Y - Circles[Closest].Radius),
                    (float)(Circles[Closest].Radius * 2), (float)(Circles[Closest].Radius * 2));
            }
            else
            {
                g.DrawEllipse(LimePen,
                    (float)(Circles[Closest].Center.X - Circles[Closest].Radius), (float)(Circles[Closest].Center.Y - Circles[Closest].Radius),
                    (float)(Circles[Closest].Radius * 2), (float)(Circles[Closest].Radius * 2));
                g.DrawEllipse(AquaPen,
                    (float)(Circles[Smallest].Center.X - Circles[Smallest].Radius), (float)(Circles[Smallest].Center.Y - Circles[Smallest].Radius),
                    (float)(Circles[Smallest].Radius * 2), (float)(Circles[Smallest].Radius * 2));
            }


            for (int i = 0, n = Circles.Count; i < n; i++)
            {
                if ((i != Closest) && (i != Smallest))
                {
                    g.DrawEllipse(OrangePen,
                       (float)(Circles[i].Center.X - Circles[i].Radius), (float)(Circles[i].Center.Y - Circles[i].Radius),
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
            double X = (Circles[0].Center.X - FrameCenterX);
            double Y = (Circles[0].Center.Y - FrameCenterY);
            double dist = X * X + Y * Y;  // we are interested only which one is closest, don't neet to take square roots to get distance right
            double dX, dY;
            for (int i = 0; i < Circles.Count; i++)
            {
                dX = Circles[i].Center.X - FrameCenterX;
                dY = Circles[i].Center.Y - FrameCenterY;
                if ((dX * dX + dY * dY) < dist)
                {
                    dist = dX * dX + dY * dY;
                    closest = i;
                }
            }
            return closest;
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
                QuadChecker.MinAcceptableDistortion = 0.5F;  // in pixels, default 0.5 
                QuadChecker.RelativeDistortionLimit = 0.05F;  // default 0.03 (3%)

                // use the Outline checker to extract the corner points
                if (QuadChecker.IsQuadrilateral(edgePoints, out cornerPoints))
                {
                    // only do things if the corners form a rectangle
                    SimpleShapeChecker RectangleChecker = new SimpleShapeChecker();
                    RectangleChecker.AngleError = 7;  // default 7
                    RectangleChecker.LengthError = 0.050F;  // default 0.1 (10%)
                    RectangleChecker.MinAcceptableDistortion = 1F;  // in pixels, default 0.5 
                    RectangleChecker.RelativeDistortionLimit = 0.05F;  // default 0.03 (3%)
                    if (RectangleChecker.CheckPolygonSubType(cornerPoints) == PolygonSubType.Rectangle)
                    //if (true)
                    {
                        List<IntPoint> corners = PointsCloud.FindQuadrilateralCorners(edgePoints);
                        // In some cases, the bitmap edges count as a rectangle. That should be ignored:
                        if ( !( (corners[2].X==(bitmap.Size.Width-1)) && 
                                (corners[2].Y == (bitmap.Size.Height - 1))
                              )
                            )
                        {
                            Rectangles.Add( new Shapes.Rectangle(corners.Select(s => (AForge.Point)s).ToList()) );
                        }
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
            int PenSize = 3;
            // if show pixels is off and image is not zoomed, the drawn pixels are going to be scaled down.
            // To make the circles visible, we need to draw then bit thicker
            if (!(ImageBox.SizeMode == PictureBoxSizeMode.CenterImage) && !Settings.Zoom)
            {
                PenSize = 5;
            }
            Graphics g = Graphics.FromImage(image);
            Pen OrangePen = new Pen(Color.DarkOrange, PenSize);
            Pen LimePen = new Pen(Color.Lime, PenSize);

            for (int i = 0, n = Rectangles.Count; i < n; i++)
            {
                if (i == closest)
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
            if (Rectangles.Count == 0)
            {
                return 0;
            }
            int closest = 0;
            float X = Rectangles[0].Center.X - (float)FrameCenterX;
            float Y = Rectangles[0].Center.Y - (float)FrameCenterY;
            float dist = X * X + Y * Y;  // we are interested only which one is closest, don't neet to take square roots to get distance right
            float dX, dY;
            for (int i = 0; i < Rectangles.Count; i++)
            {
                dX = Rectangles[i].Center.X - (float)FrameCenterX;
                dY = Rectangles[i].Center.Y - (float)FrameCenterY;
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
            List<Shapes.Rectangle> GoodRectangles = new List<Shapes.Rectangle>();
            image.Dispose();

            double X = 0.0;
            double Y = 0.0;
            MaxDistance = MaxDistance * GetMeasurementZoom();
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
            Pen YellowPen = new Pen(Color.Yellow, 1);
            Pen GreenPen = new Pen(Color.Green, 1);
            Pen BluePen = new Pen(Color.Blue, 1);
            Graphics g = Graphics.FromImage(img);
            int x1, x2, y1, y2;
            int step = 40;
            // vertical
            int i = 0;
            while (i < FrameSizeX)
            {
                // FrameCenterX, 0 to FrameCenterX, FrameSizeY
                RotateByFrameCenter(FrameCenterX + i, 0, out x1, out y1);
                RotateByFrameCenter(FrameCenterX + i, FrameSizeY, out x2, out y2);
                g.DrawLine(YellowPen, x1, y1, x2, y2);
                RotateByFrameCenter(FrameCenterX - i, 0, out x1, out y1);
                RotateByFrameCenter(FrameCenterX - i, FrameSizeY, out x2, out y2);
                g.DrawLine(YellowPen, x1, y1, x2, y2);
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
                g.DrawLine(YellowPen, x1, y1, x2, y2);
                RotateByFrameCenter(0, FrameCenterY - i, out x1, out y1);
                RotateByFrameCenter(FrameSizeX, FrameCenterY - i, out x2, out y2);
                g.DrawLine(YellowPen, x1, y1, x2, y2);
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

            YellowPen.Dispose();
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
        {/*
            // default values used when show pixels is off: 
            // Draw from frame edges inwards, using ticksize that gets zoomed down
            int TickSize = (FrameSizeX / 640) * 8;
            int XstartUp = FrameSizeY;  // values used when drawing along X axis
            int XstartBot = 0;
            int YstartLeft = 0;         // values used when drawing along Y axis
            int YstartRight = FrameSizeX;
            int Xinc = Convert.ToInt32(YstartRight / SideMarksX);    // sidemarks: 10cm on machine
            int Yinc = Convert.ToInt32(XstartUp / SideMarksY);

            if (ImageBox.SizeMode == PictureBoxSizeMode.CenterImage)
            {
                // Show pixels is on, draw to middle of the image
                TickSize = 8;
                XstartUp = (FrameSizeY / 2) + 240;
                XstartBot = (FrameSizeY / 2) - 240;
                YstartLeft = (FrameSizeX / 2) - 320;
                YstartRight = (FrameSizeX / 2) + 320;
                Xinc = Convert.ToInt32(640 / SideMarksX);
                Yinc = Convert.ToInt32(480 / SideMarksY);
            }

            Pen pen = new Pen(Color.Red, 2);
            Graphics g = Graphics.FromImage(img);
            int X = YstartLeft + Xinc;
            while (X < YstartRight)
            {
                g.DrawLine(pen, X, XstartUp, X, XstartUp - TickSize);
                g.DrawLine(pen, X, XstartBot, X, XstartBot + TickSize);
                X += Xinc;
            }

            int Y = XstartBot + Yinc;
            while (Y < XstartUp)
            {
                g.DrawLine(pen, YstartLeft, Y, YstartLeft + TickSize, Y);
                g.DrawLine(pen, YstartRight, Y, YstartRight - TickSize, Y);
                Y += Yinc;
            }
*/
            Graphics g = Graphics.FromImage(img);
            int PenSize = 2;
            int tick = 12;
            int PicSizeX = FrameSizeX;
            int PicSizeY = FrameSizeY;
            if (ImageBox.SizeMode == PictureBoxSizeMode.CenterImage)
            {
                // UI shows 640x480 from middle of image. Draw tics accordingly
                PicSizeX = 640;
                PicSizeY = 480;
                PenSize = 1;
                tick = 6;
            }

            Pen pen = new Pen(Color.Red, PenSize);
            int XStart = (FrameSizeX / 2) - (PicSizeX / 2);
            int XEnd = (FrameSizeX / 2) + (PicSizeX / 2);
            int YStart = (FrameSizeY / 2) - (PicSizeY / 2);
            int YEnd = (FrameSizeY / 2) + (PicSizeY / 2);

            int Xinc = Convert.ToInt32(PicSizeX / SideMarksX);
            int X = XStart;
            while (X < XEnd)
            {
                g.DrawLine(pen, X, YEnd, X, YEnd - tick);
                g.DrawLine(pen, X, YStart, X, YStart + tick);
                X += Xinc;
            }
            int Yinc = Convert.ToInt32(PicSizeY / SideMarksY);
            int Y = YEnd;
            while (Y > YStart)
            {
                g.DrawLine(pen, XEnd, Y, XEnd - tick, Y);
                g.DrawLine(pen, XStart, Y, XStart + tick, Y);
                Y -= Yinc;
            }
            pen.Dispose();
            g.Dispose();

            pen.Dispose();
            g.Dispose();
        }

        // =========================================================
        private void DrawBoxFunct(ref Bitmap img)
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
            Pen pen = new Pen(Color.Green, 3);
            Graphics g = Graphics.FromImage(img);
            double length = 60;
            float detailLength = (float)(0.65 * length);
            double angle = Math.PI / 180.0 * (ArrowAngle); // to radians, -180 to get ccw, +90 to start from up

            System.Drawing.Drawing2D.AdjustableArrowCap bigArrow = new System.Drawing.Drawing2D.AdjustableArrowCap(6, 6);
            
            g.DrawLine(pen, FrameCenterX, FrameCenterY, FrameCenterX + (int)(detailLength), FrameCenterY);
            g.DrawArc(pen, FrameCenterX - detailLength, FrameCenterY - detailLength, detailLength * 2.0f, detailLength * 2.0f, 0.0f, (float)ArrowAngle);

            pen.Dispose();
            pen = new Pen(Color.Blue, 3);
            g.DrawLine(pen, FrameCenterX, FrameCenterY, (int)(FrameCenterX + Math.Cos(angle + Math.PI) * length), (int)(FrameCenterY + Math.Sin(angle + Math.PI) * length));
            pen.CustomEndCap = bigArrow;
            g.DrawLine(pen, FrameCenterX, FrameCenterY, (int)(FrameCenterX + Math.Cos(angle) * length), (int)(FrameCenterY + Math.Sin(angle) * length));

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
                        image.SetPixel(x, y, Settings.SnapshotColor);
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


        // =========================================================
        // Measurements on video images
        // =========================================================
        #region Measurements

        // Caller = any function doing measurement from video frames. 
        // The list of functions processing the image used in measurements, set by caller:
        List<AForgeFunction> MeasurementFunctions = new List<AForgeFunction>();

        // Measurement parameters: min and max size, max distance from initial location, set by caller:
        public MeasurementParametersClass MeasurementParameters = new MeasurementParametersClass();

        // ==========================================================================================================
        // Measurements are done by taking one frame and processing that:
        // volatile to avoid multithread bugs
        private volatile bool CopyFrame = false;     // Tells we need to take a frame from the stream 
        private volatile bool secondFrame = false;          //throw away first TemporaryFrame to avoid movement artifacts
        private Bitmap TemporaryFrame;      // The frame is stored here.

        // The caller builds the MeasurementFunctions list:

        public void BuildMeasurementFunctionsList(List<AForgeFunctionDefinition> UiList)
        {
            MeasurementFunctions = BuildFunctionsList(UiList);
        }

        // And calls xx_measure() funtion. 
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
                    f.func(ref TemporaryFrame, f.parameter_int, f.parameter_double, f.R, f.G, f.B);
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
        // Measure
        // ==========================================================================================================

        private void DisplayShapes(List<Shapes.Shape> Shapes, int StartFrom, double XmmPpix, double YmmPpix)
        {
            if ((Shapes.Count - StartFrom) == 0)
            {
                MainForm.DisplayText("    No results.");
                return;
            }
            string OutString = "";
            string Xpxls;
            string Ypxls;
            string Xmms;
            string Ymms;
            string Xsize;
            string Ysize;
            string Angle;
            string SizeXmm;
            string SizeYmm;

            for (int i = StartFrom; i < Shapes.Count; i++)
            {
                Xpxls = String.Format("{0,6:0.0}", Shapes[i].Center.X - FrameCenterX);
                Xmms = String.Format("{0,6:0.000}", (Shapes[i].Center.X - FrameCenterX) * XmmPpix);
                Ypxls = String.Format("{0,6:0.0}", Shapes[i].Center.Y - FrameCenterY);
                Ymms = String.Format("{0,6:0.000}", (Shapes[i].Center.Y - FrameCenterY) * YmmPpix);
                Xsize = String.Format("{0,5:0.0}", Shapes[i].Xsize);
                Ysize = String.Format("{0,5:0.0}", Shapes[i].Ysize);
                Angle = String.Format("{0,5:0.0}", Shapes[i].Angle);
                SizeXmm = String.Format("{0,5:0.00}", Shapes[i].Xsize * XmmPpix);
                SizeYmm = String.Format("{0,5:0.00}", Shapes[i].Ysize * YmmPpix);
                OutString = "pos: " + Xpxls + ", " + Ypxls + "px; " + Xmms + ", " + Ymms + "mm; " +
                    "size: " + Xsize + ", " + Ysize + "px; " + SizeXmm + ", " + SizeYmm + "mm";
                if (true)
                    OutString += "; angle: " + Angle + "°";
                MainForm.DisplayText(OutString);
            }
        }


        // =========================================================
        private double DistanceToCenter(Shapes.Shape shape)
        {
            double X = shape.Center.X - FrameCenterX;
            double Y = shape.Center.Y - FrameCenterY;

            return Math.Sqrt(X * X + Y * Y);
        }

        public bool Measure(out double Xresult, out double Yresult, out double Aresult, out int err, bool DisplayResults = false )
        {
            Xresult = 0.0;
            Yresult = 0.0;
            Aresult = 0.0;
            err = 0;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            if ((!MeasurementParameters.SearchRounds) && (!MeasurementParameters.SearchRectangles)
                && (!MeasurementParameters.SearchComponents))
            {
                MainForm.DisplayText("Nothing to search for. Check some of the \"Features to search for\" boxes.", KnownColor.Red, true);
                return false;
            }

            Bitmap image = GetMeasurementFrame();

            // Find candidates:
            stopwatch.Stop();   // Don't time diagnostic messages
            if (DisplayResults)
            {
                MainForm.DisplayText("Result candidates:");
            }
            stopwatch.Start();

            List<Shapes.Shape> Candidates = new List<Shapes.Shape>();

            double zoom = GetMeasurementZoom();
            double XmmPpix = Settings.XmmPerPixel / zoom;
            double YmmPpix = Settings.YmmPerPixel / zoom;

            if (MeasurementParameters.SearchRounds)
            {
                List<Shapes.Circle> Circles = FindCirclesFunct(image);
                foreach (Shapes.Circle circle in Circles)
                {
                    Candidates.Add(new Shapes.Shape()
                    {
                        Center = circle.Center,
                        Angle = circle.Angle,
                        Xsize = circle.Radius * 2.0,
                        Ysize = circle.Radius * 2.0
                    });
                }
                stopwatch.Stop();
                if (DisplayResults)
                {
                    MainForm.DisplayText("Circles:");
                    DisplayShapes(Candidates, 0, XmmPpix, YmmPpix);
                }
                stopwatch.Start();

            }
            int count = Candidates.Count;

            if (MeasurementParameters.SearchRectangles)
            {
                List<Shapes.Rectangle> Regtangles = FindRectanglesFunct(image);
                foreach (Shapes.Rectangle regt in Regtangles)
                {
                    Candidates.Add(new Shapes.Shape()
                    {
                        Center = regt.Center,
                        Angle = regt.Angle,
                        Xsize = regt.LongsideLenght,
                        Ysize=regt.ShortSideLenght
                    });
                }
                stopwatch.Stop();
                if (DisplayResults)
                {
                    MainForm.DisplayText("Regtangles:");
                    DisplayShapes(Candidates, count, XmmPpix, YmmPpix);
                }
                stopwatch.Start();
            }

            count = Candidates.Count;
            if (MeasurementParameters.SearchComponents)
            {
                List<Shapes.Component> Components = FindComponentsFunct(image);

                foreach (Shapes.Component comp in Components)
                {
                    if (comp.BoundingBox.Corners.Count != 4)
                        continue;

                    Candidates.Add(new Shapes.Shape()
                    {
                        Center = comp.BoundingBox.Center,
                        Angle = comp.BoundingBox.Angle,
                        Xsize = comp.BoundingBox.LongsideLenght,
                        Ysize = comp.BoundingBox.ShortSideLenght
                    });
                }

                stopwatch.Stop();
                if (DisplayResults)
                {
                    MainForm.DisplayText("Components:");
                    DisplayShapes(Candidates, count, XmmPpix, YmmPpix);
                }
                stopwatch.Start();
            }

            // Filter for size
            List<Shapes.Shape> FilteredForSize = new List<Shapes.Shape>();
            foreach (Shapes.Shape shape in Candidates)
            {
                if (  ((shape.Xsize * XmmPpix) > MeasurementParameters.Xmin) &&
                      ((shape.Xsize * XmmPpix) < MeasurementParameters.Xmax) &&
                      ((shape.Ysize * YmmPpix) > MeasurementParameters.Ymin) &&
                      ((shape.Ysize * YmmPpix) < MeasurementParameters.Ymax))

                {
                    FilteredForSize.Add(shape);
                }
            }
            stopwatch.Stop();
            if (DisplayResults)
            {
                MainForm.DisplayText("Filtered for size, results:");
            }
            stopwatch.Start();

            if (FilteredForSize.Count == 0)
            {
                if (DisplayResults)
                {
                    MainForm.DisplayText("No items left.");
                }
                else
                {
                    MainForm.DisplayText("Camera Measure(), no items left after size filtering.");
                }
                return false;
            }
            stopwatch.Stop();
            if (DisplayResults)
            {
                DisplayShapes(FilteredForSize, 0, XmmPpix, YmmPpix);
            };
            stopwatch.Start();

            // Find closest to center
            int ResultIndex = 0;
            int ResultCount = 0;
            double ResultDistance = DistanceToCenter(FilteredForSize[0]);

            if (FilteredForSize.Count > 0)
            {
                // if more than one result candidate at this point, find closest to center
                for (int i = 1; i < FilteredForSize.Count; i++)
                {
                    double ThisDistance = DistanceToCenter(FilteredForSize[i]);
                    if (ThisDistance< ResultDistance)
                    {
                        ResultIndex = i;
                        ResultDistance = ThisDistance;
                    }
                }
                // and check uniqueness: 
                // ResultIndex tells which item is closest. There should not be others too close
                double Xclosest = FilteredForSize[ResultIndex].Center.X;
                double Yclosest = FilteredForSize[ResultIndex].Center.Y;
                if ((MeasurementParameters.XUniqueDistance<0.01)||
                    (MeasurementParameters.YUniqueDistance < 0.01))
                {
                    MainForm.DisplayText("Uniquedistance is 0 or less.", KnownColor.Red, true);
                    return false;
                }
                foreach (Shapes.Shape shape in FilteredForSize)
                {
                    double Xdist = Math.Abs((shape.Center.X - Xclosest) * XmmPpix);
                    double Ydist = Math.Abs((shape.Center.Y - Yclosest) * YmmPpix);
                    if ((Xdist < MeasurementParameters.XUniqueDistance) && (Ydist < MeasurementParameters.YUniqueDistance))
                    {
                        ResultCount++;
                    }
                }
                if (ResultCount==0)
                {
                    MainForm.DisplayText("***Camera Measure(), uniquetest=0", KnownColor.Red, true);
                    return false;
                }
            }
            Xresult = (FilteredForSize[ResultIndex].Center.X - FrameCenterX) * XmmPpix;
            Yresult = (FilteredForSize[ResultIndex].Center.Y - FrameCenterY) * YmmPpix;
            Aresult = FilteredForSize[ResultIndex].Angle;

            stopwatch.Stop();
            if (DisplayResults)
            {
                string result = "Result: X= " + Xresult.ToString("0.000", CultureInfo.InvariantCulture) +
                    ", Y= " + Yresult.ToString("0.000", CultureInfo.InvariantCulture);
                MainForm.DisplayText(result);
                if (ResultCount == 1)
                {
                    MainForm.DisplayText("Result is unique.");
                }
                else
                {
                    MainForm.DisplayText("Result is NOT unique! There are " + (ResultCount - 1).ToString() + " other possible results."
                        , KnownColor.DarkRed, true);
                }

            }
            if (DisplayResults)
            {
                MainForm.DisplayText("Camera Measure(): Elapsed time " + stopwatch.ElapsedMilliseconds.ToString() + "ms");
            }

            err = ResultCount;
            if (err==1)
            {
                return true;
            }
            else
            {
                MainForm.DisplayText("Camera Measure(): result not unique");
                return false;
            }
        }

        #endregion
    }
}

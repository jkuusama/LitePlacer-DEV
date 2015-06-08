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

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu;


namespace LitePlacer{
	public class Camera	{
        //these are locations to draw as an overlay on the image
        //the points are relative to the centeral point (0,0) on the screen
        public List<PointF> MarkA = new List<PointF>();
        public List<PointF> MarkB = new List<PointF>();
        public List<VideoTextMarkup> MarkupText = new List<VideoTextMarkup>();

		private VideoCaptureDevice VideoSource = null;
		public FormMain MainForm;
        public VideoDetection videoDetection;
        public VideoProcessing videoProcessing;

		public Camera(FormMain MainF){
			MainForm = MainF;
            videoDetection = new VideoDetection(this);
            videoProcessing = new VideoProcessing(this);
		}
    

        public bool IsUpCamera() {
            return this.Equals(MainForm.UpCamera);
        }
        public bool IsDownCamera() {
            return this.Equals(MainForm.DownCamera);
        }

        public void AddMarkupText(VideoTextMarkup markup) {
            lock (MarkupText) {
                MarkupText.Add(markup);
            }
        }

        public void ClearMarkupText() {
            lock (MarkupText) {
                MarkupText.Clear();
            }
        }

        public double XmmPerPixel {
            get { return (IsUpCamera()) ? Properties.Settings.Default.UpCam_XmmPerPixel : Properties.Settings.Default.DownCam_XmmPerPixel; }
        }
        public double YmmPerPixel {
            get { return (IsUpCamera()) ? Properties.Settings.Default.UpCam_YmmPerPixel : Properties.Settings.Default.DownCam_YmmPerPixel; }
        }
        
        public PartLocation XYmmPerPixel {
            get { return new PartLocation(XmmPerPixel,YmmPerPixel); }
        }

		public bool IsRunning()		{
			if (VideoSource != null)  return (VideoSource.IsRunning);
			return false;
		}

        // Asks nicely
		public void SignalToStop() 		{
			VideoSource.SignalToStop();
		}

        // Tries to force it (but still doesn't always work, just like with children)
		public void NakedStop() {
			VideoSource.Stop();
		}

		public void Close(){
			if (VideoSource == null ||  !VideoSource.IsRunning)  return;				
			VideoSource.SignalToStop();
			VideoSource.WaitForStop();  // problem?
			VideoSource.NewFrame -= new NewFrameEventHandler(Video_NewFrame);
			VideoSource = null;
			MainForm.DisplayText(Id + " stop: " + MonikerString);
			MonikerString = "unconnected";
		}

		public void DisplayPropertyPage()		{
			VideoSource.DisplayPropertyPage(IntPtr.Zero);
		}

		// Image= PictureBox in UI, the final shown image
		// Frame= picture from camera

		// All processing and returned values are in Frame content
		System.Windows.Forms.PictureBox _imageBox;
		public System.Windows.Forms.PictureBox ImageBox {
			get	{return (_imageBox); }
			set	{_imageBox = value;  }
		}

        //these shouldn't have setters
        public int ImageCenterX { get { return ImageBox.Width / 2; } }
        public int ImageCenterY { get { return ImageBox.Height / 2; } }
        public int ImageSizeX { get { return ImageBox.Width; } }
        public int ImageSizeY { get { return ImageBox.Height; } }
		public int FrameCenterX { get { return FrameSizeX  / 2;}}
		public int FrameCenterY { get { return FrameSizeY / 2; }}
        public int FrameSizeX { get { return (VideoSource != null) ? VideoSource.VideoCapabilities[0].FrameSize.Width : 0; } }
		public int FrameSizeY { get { return (VideoSource!=null) ? VideoSource.VideoCapabilities[0].FrameSize.Height : 0; } }
        /// <summary>
        /// This is the center of the screen in pixels with no zoom
        /// </summary>
        public PartLocation FrameCenter {
            get { return new PartLocation(FrameCenterX, FrameCenterY); }
        }

		private string MonikerString = "unconnected";
		private string Id = "unconnected";

		public bool Start(string cam, int DeviceNo)		{
            try {
			    FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
			    MonikerString = videoDevices[DeviceNo].MonikerString;
			    Id = cam;
			    MainForm.DisplayText(cam + " start: Id= "+ Id.ToString() +  "moniker= " + MonikerString);
			    VideoSource = new VideoCaptureDevice(MonikerString);

			    VideoSource.NewFrame += new NewFrameEventHandler(Video_NewFrame);
			    VideoSource.Start();
                return VideoSource.IsRunning;
            }  catch {
                return false;
            }
		}

        /**** STATIC METHODS *****/
        public static List<string> GetDeviceList()  {
            List<string> Devices = new List<string>();

            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo device in videoDevices)          {
                Devices.Add(device.Name);
            }
            return (Devices);
        }

        public List<string> GetMonikerStrings()  {
            List<string> Monikers = new List<string>();

            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo device in videoDevices)            {
                Monikers.Add(device.MonikerString);
            }
            return (Monikers);
        }


		// ==========================================================================================================
		// Measurements are done by taking one frame and processing that:
		public bool CopyFrame = false;		// Tells we need to take a frame from the stream 
		public Bitmap TemporaryFrame;      // The frame is stored here.

		// The caller builds the MeasurementFunctions list:

		public void BuildMeasurementFunctionsList(DataGridView Grid) {
            BuildDisplayFunctionsList(Grid); // better to display whatever the camera is seeing at all times to help debug problems
          //  measurementProcessing.UpdateFunctionList(Grid);
          //  measurementProcessing.ApplyVideoMarkup = false; //don't draw over this image
		}

        public void BuildDisplayFunctionsList(DataGridView Grid) {
            videoProcessing.UpdateFunctionList(Grid);
            videoProcessing.ApplyVideoMarkup = true; //draw over this image
        }


        public double GetMeasurementZoom() {
            return videoProcessing.GetZoom();
        }

        public double GetDisplayZoom() {
            return videoProcessing.GetZoom() * ZoomFactor;
        }


		// ==========================================================================================================
		// Members we need for our drawing functions
		// ==========================================================================================================

		// ==========================================================================================================
		// Zoom
		public bool Zoom { get; set; }          // If image is zoomed or not
		private double _ZoomFactor = 1.0;
		public double ZoomFactor {get {return (Zoom)?_ZoomFactor:1;} set {_ZoomFactor=value;}}
        public double SnapshotRotation = 0.0;  // rotation when snapshot was taken

		public int Threshold { get; set; }          // Threshold for all the "draw" functions
		public bool GrayScale { get; set; }         // If image is converted to grayscale 
		public bool Invert { get; set; }            // If image is inverted (makes most sense on grayscale, looking for black stuff on light background)
		public bool DrawCross { get; set; }         // If crosshair cursor is drawn
		public bool DrawSidemarks { get; set; }     // If marks on the side of the image are drawn
		public double SideMarksX { get; set; }		// How many marks on top and bottom (X) and sides (Y)
		public double SideMarksY { get; set; }		// (double, so you can do "SidemarksX= workarea_in_mm / 100;" to get mark every 10cm
		public bool DrawDashedCross { get; set; }   // If a dashed crosshaircursor is drawn (so that the center remains visible)
		public bool FindCircles { get; set; }       // Find and highlight circles in the image
		public bool FindRectangles { get; set; }    // Find and draw regtangles in the image
        public bool FindFiducial { get; set; }      // Find and marks location of template based fiducials in image
        public bool Draw1mmGrid { get; set; }       // overlay image with a 1mm grid pattern based on optical mapping
		public bool FindComponent { get; set; }     // Finds a component and identifies its center
		public bool TakeSnapshot { get; set; }      // Takes a b&w snapshot (of a component, most likely)     
		public bool Draw_Snapshot { get; set; }     // Draws the snapshot on the image 
        public bool PauseProcessing { get {return !videoProcessing.ApplyVideoMarkup;} set { videoProcessing.ApplyVideoMarkup = !value; } }   // Drawing the video slows everything down. This can pause it for measurements.
		public bool TestAlgorithm { get; set; }
		public bool DrawBox { get; set; }           // Draws a box on the image that is used for scale setting
        
        
        // BOX 
        private int boxSizeX;
        public int BoxSizeX                         // The box size
        {
            get {
                return (boxSizeX);
            }
            set {
                boxSizeX = value;
                BoxRotationDeg = boxRotation; // force recalculation of corner points

            }
        }
        private int boxSizeY;
        public int BoxSizeY {
            get {
                return (boxSizeY);
            }
            set {
                boxSizeY = value;
                BoxRotationDeg = boxRotation; // force recalculation of corner points

            }
        }

        private double boxRotation = 0;
        public System.Drawing.Point[] BoxPoints = new System.Drawing.Point[4];
        public double BoxRotationDeg        // The box is drawn rotated this much
        {
            get {
                return (boxRotation);
            }
            set {
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
                double Rot = -boxRotation / (180 / Math.PI);  // to radians, and counter-clockwise
                for (int i = 0; i < 4; i++) {
                    double pX = BoxPoints[i].X;
                    double pY = BoxPoints[i].Y;
                    BoxPoints[i].X = (int)Math.Round(Math.Cos(Rot) * pX - Math.Sin(Rot) * pY);
                    BoxPoints[i].Y = (int)Math.Round(Math.Sin(Rot) * pX + Math.Cos(Rot) * pY);
                }
            }
        }


		// ==========================================================================================================
		// Eventhandler if new frame is ready
		// ==========================================================================================================
		// Each frame goes trough Video_NewFrame

		private void Video_NewFrame(object sender, NewFrameEventArgs eventArgs)		{
			Bitmap frame = (Bitmap)eventArgs.Frame.Clone();

            // apply processing
            videoProcessing.ProcessFrame(ref frame, ref TemporaryFrame);

            try { ImageBox.Image = frame; } catch { }
       	}


        public Bitmap GetMeasurementFrame() {
            // make sure we get a new frame
            CopyFrame = true;
            while (CopyFrame) Thread.Sleep(10); // wait till new Temporary Frame is set
            return TemporaryFrame;
        }

	}
}

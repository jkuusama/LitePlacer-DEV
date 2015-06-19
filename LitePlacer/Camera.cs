namespace LitePlacer{
/*	public class Camera	{
        //these are locations to draw as an overlay on the image
        //the points are relative to the centeral point (0,0) on the screen
       
        
        public VideoCapture videoCapture;
	    public CameraView parent;
        public VideoDetection videoDetection;
        public VideoProcessing videoProcessing;
        public PictureBoxClickDelegate clickDelegate = null;
        public string ID = "";

		public Camera(CameraView form, string name){
			parent = form;
            videoDetection = new VideoDetection(this);
            videoProcessing = new VideoProcessing(this);
            videoCapture = new VideoCapture();
            ID = name;
		}


        public void SetVideoProcessing(List<AForgeFunction> list) {
            videoProcessing.SetFunctionsList(list);
        }

        public void ClearVideoProcessing() {
            videoProcessing.SetFunctionsList(new List<AForgeFunction>());
        }

       public bool IsUpCamera() {
           return ID.Equals("UpCamera");
          //  return this.Equals(MainForm.UpCamera);
        }
        public bool IsDownCamera() {
            return ID.Equals("DownCamera");
            //return this.Equals(MainForm.DownCamera);
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
    //    public int FrameCenterX { get { return videoCapture.FrameCenterX; } }
      //  public int FrameCenterY { get { return videoCapture.FrameCenterY; } }
      //  public int FrameSizeX { get { return videoCapture.FrameSizeX; } }
      //  public int FrameSizeY { get { return videoCapture.FrameSizeY; } }
    
      //  public PartLocation FrameCenter {
      //      get { return new PartLocation(FrameCenterX, FrameCenterY); }
      //  }


		public bool Start(int id) {
            return videoCapture.Start(id);
		}

 

		// ==========================================================================================================
		// Measurements are done by taking one frame and processing that:
		public bool CopyFrame = false;		// Tells we need to take a frame from the stream 
		public Bitmap TemporaryFrame;      // The frame is stored here.

		
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



        public Bitmap GetMeasurementFrame() {
            var frame = videoCapture.GetFrame();
            frame = videoProcessing.ProcessFrame(frame);
            return frame;
        }



	}
*/
}

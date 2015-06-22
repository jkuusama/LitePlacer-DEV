        using System;
        using System.Collections.Generic;
        using System.Drawing;
        using System.Threading;
        using AForge.Video;
        using AForge.Video.DirectShow;

namespace LitePlacer {
    public delegate void NewFrameCaptureDelegate(Bitmap bitmap);
    public enum CameraType {
        UpCamera, DownCamera
    };

    public class VideoCapture {
        VideoCaptureDevice VideoSource;
        public CameraType cameraType;
        
        int FrameNumber;
        string MonikerString = "";

        public Size FrameSize { get { return new Size(FrameSizeX,FrameSizeY); } }
        public int FrameSizeX { get { return (VideoSource != null) ? VideoSource.VideoCapabilities[0].FrameSize.Width : 0; } }
		public int FrameSizeY { get { return (VideoSource != null) ? VideoSource.VideoCapabilities[0].FrameSize.Height : 0; } }		
        public int FrameCenterX { get { return FrameSizeX  / 2;}}
		public int FrameCenterY { get { return FrameSizeY / 2; }}
        public PartLocation FrameCenter {get { return new PartLocation(FrameCenterX, FrameCenterY); } }

        public List<NewFrameCaptureDelegate> FrameCaptureDelegates = new List<NewFrameCaptureDelegate>();

        public VideoCapture(CameraType type) {
            cameraType = type;
        }

        public bool IsDown() { return (cameraType == CameraType.DownCamera);}
        public bool IsUp() { return (cameraType == CameraType.UpCamera); }

		public bool IsRunning() {
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

        public void NoWaitClose() {
            if (VideoSource == null || !VideoSource.IsRunning) return;
            VideoSource.SignalToStop();
            VideoSource.NewFrame -= NewFrame;
            VideoSource = null;
            MonikerString = "";
        }

		public void Close(){
			if (VideoSource == null ||  !VideoSource.IsRunning)  return;				
			VideoSource.SignalToStop();
			VideoSource.WaitForStop();  // problem?
			VideoSource.NewFrame -= NewFrame;
			VideoSource = null;
            MonikerString = "";
		}

		public void DisplayPropertyPage()		{
			VideoSource.DisplayPropertyPage(IntPtr.Zero);
		}

		public bool Start(int DeviceNo)		{
            try {
			    FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
			    MonikerString = videoDevices[DeviceNo].MonikerString;
			    VideoSource = new VideoCaptureDevice(MonikerString);
			    VideoSource.NewFrame += NewFrame;
			    VideoSource.Start();
                FrameNumber = 0;
                return VideoSource.IsRunning;
            }  catch {
                return false;
            }
		}

        /* a bunch of BS to help ensure we avoid race conditions */
        /* still untested but was seeing problems before */
        object NewFrameLock = new object();
        Bitmap lastFrame;
        public Bitmap GetFrame() {
            bool acquiredLock = false;
            Bitmap ret = null;
            while (ret == null) {
                try {
                    Monitor.TryEnter(NewFrameLock, ref acquiredLock);
                    if (acquiredLock) {
                        ret = (Bitmap)lastFrame.Clone();
                    } else {
                        Thread.Sleep(1); //wait for someone else to give up the lock?
                    }
                } finally {
                    if (acquiredLock) {
                        Monitor.Exit(NewFrameLock);
                    }
                }
            }
            return ret;
        }


		private void NewFrame(object sender, NewFrameEventArgs eventArgs) {
            bool acquiredLock = false;
            
            // (try to) set the last frame
            try {
                Monitor.TryEnter(NewFrameLock, ref acquiredLock);
                if (acquiredLock) {
                    if (lastFrame != null) lastFrame.Dispose();
                    lastFrame = (Bitmap)eventArgs.Frame.Clone();
                    FrameNumber++;
                }
            } finally {
                if (acquiredLock) Monitor.Exit(NewFrameLock);
            }

            //forward copies of the bitmap to any delegates
            foreach (var x in FrameCaptureDelegates) {
                x((Bitmap)eventArgs.Frame.Clone());
            }
       	}


        /* STATIC FUNCTIONS */
        public static List<string> GetVideoDeviceList() {
            List<string> Devices = new List<string>();

            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo device in videoDevices) {
                Devices.Add(device.Name);
            }
            return (Devices);
        }

	}
}

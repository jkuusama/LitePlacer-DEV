using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace LitePlacer
{
#pragma warning disable CA1031 // Do not catch general exception types (see MainForm.cs beginning)

    public class SerialComm
    {

        SerialPort Port = new SerialPort();

        // To process data on the DataReceived thread, get reference of Cnc, so we can pass data to it.
        private CNC Cnc;
        // To show what we send, we need a reference to mainform.
        private static FormMain MainForm;

        public SerialComm(CNC caller, FormMain MainF)
        {
            Cnc = caller;
            Port.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
            Port.PinChanged += new SerialPinChangedEventHandler(PinChanged);
            Port.ErrorReceived += new SerialErrorReceivedEventHandler(ErrorReceived);
            MainForm = MainF;
        }

        // Pin changed and error received events are subscribed to, in hoping to catch board
        // resets and cable disconnections. These never fire. Bummer. If you read this and know
        // how to catch those, please contribute!
        // How to: Find out the connected port device ID: https://stackoverflow.com/a/64541160/2419027
        // Detect USB device connect or remove: https://community.silabs.com/s/article/detecting-when-a-usb-device-is-connected-or-removed-in-c-net?language=en_US
        // Check if our device is still there. Maybe one day I'll get to this...

        void ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            MainForm.DisplayText("Serial port error (error received event");
            PortError();
        }

        void PinChanged(object sender, SerialPinChangedEventArgs e)
        {
            MainForm.DisplayText("Serial port error (pin changed event");
            PortError();
        }

        void PortError()
        {
            if (Cnc.ErrorState)
            {
                return; // no need to handle multiple error raising events
            }
            Close();
            Cnc.RaiseError();
            Cnc.Connected = false;
        }

        public bool IsOpen
        {
            get
            {
                return Port.IsOpen;
            }
        }

        private void Close_thread()
        {
            try
            {
                Port.Close();
            }
            catch
            {
                // there would be an exeption if the device is turned off and the port doesn't exist anymore
            }
        }

        public void Close()
        {
            try
            {
                if (!Port.IsOpen)
                {
                    return;
                }
                MainForm.DisplayText("Port close");
                Port.DiscardInBuffer();
                Port.DiscardOutBuffer();
               // Known issue: Sometimes serial port hangs in app closing. Google says that 
                // the workaround is to close in another thread

                Thread t = new Thread(() => Close_thread());
                t.Start();
                if (!t.Join(100))
                {
                    MainForm.DisplayText("*** Com didn't close");
                    t.Abort();
                }
                Thread.Sleep(50);  // Don't open/close too fast
            }
            catch
            {
            }
        }

        public bool Open(string Com)
        {
            Close();
            try
            {
                Port.PortName = Com;
                Port.BaudRate = 115200;
                Port.Parity = Parity.None;
                Port.StopBits = StopBits.One;
                Port.DataBits = 8;
                // Port.Handshake = Handshake.RequestToSend;
                Port.Handshake = Handshake.None;
                Port.DtrEnable = true;  // prevent hangs on some drivers
                Port.RtsEnable = true;
                Port.WriteTimeout = 500;
                RxString = string.Empty; 
                Thread.Sleep(100);  // Don't open/close too fast
                Port.Open();
                if (Port.IsOpen)
                {
                    Port.DiscardOutBuffer();
                    Port.DiscardInBuffer();
                }
                // Port.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
                return Port.IsOpen;
            }
            catch
            {
                return false;
            }
        }

        // ======================================================
        // Write:
        // If the PC has more than one serial port and one which is not connected to TinyG has hardware handshake
        // on, the write will hang. Doing write this way catches this situation

        public bool Write(string TxText)
        {
            try
            {
                if (!Port.IsOpen)
                {
                    MainForm.DisplayText("Serial port not open, attempt to re-open", KnownColor.DarkRed);
                }
                if (Port.IsOpen)
                {
                    Port.Write(TxText + MainForm.Setting.Serial_EndCharacters);
                }
                else
                {
                    MainForm.DisplayText("Serial port not open, write discarded: " + TxText, KnownColor.DarkRed);
                }
                return true;
            }
            catch (Exception e)
            {
                MainForm.DisplayText("Serial port write failed: " + e.Message, KnownColor.DarkRed);
                Close();
                return false;
            }
        }

        public void ClearBuffer()
        {
            RxString = string.Empty;
        }

        const int ReadBufferSize = 10000;
        private string RxString = string.Empty;

        // The DataReceived() routine is called when a charater is received from the serial port.
        // The data is assumed to be ASCII, terminated with \n or \n\r
        // When \n received, calls Cnc.LineReceived(), without termination character
        // the \r is discarded

        void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //Initialize a buffer to hold the received data 
            byte[] buffer = new byte[ReadBufferSize];
            string WorkingString;

            try
            {
                //There is no accurate method for checking how many bytes are read 
                //unless you check the return from the Read method 
                int bytesRead = Port.Read(buffer, 0, buffer.Length);

                //The received data is ASCII
                RxString += Encoding.ASCII.GetString(buffer, 0, bytesRead);
                //Process each line
                while (RxString.IndexOf("\n", StringComparison.Ordinal) > -1)
                {
                    //Even when RxString does contain terminator we cannot assume that it is the last character received 
                    WorkingString = RxString.Substring(0, RxString.IndexOf("\n", StringComparison.Ordinal));
                    //Remove the data and the terminator from tString 
                    RxString = RxString.Substring(RxString.IndexOf("\n", StringComparison.Ordinal) + 1);
                    WorkingString = WorkingString.Replace("\r", "");
                    Cnc.LineReceived(WorkingString);
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                MainForm.DisplayText("########## " + ex, KnownColor.DarkRed, true);
#pragma warning restore CA1031 // Do not catch general exception types
            }
        }

    }
}

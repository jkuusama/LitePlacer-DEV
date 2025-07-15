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
            MainForm = MainF;
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
                Port.DiscardInBuffer();
                Port.DiscardOutBuffer();
               // Known issue: Sometimes serial port hangs in app closing. Google says that 
                // the workaround is to close in another thread
                Thread t = new Thread(() => Close_thread());
                t.Start();
                MainForm.DisplayText("Com closing delay:");
                for (int i = 0; i < 100; i++)  // delay for system to clear buffers
                {
                    Thread.Sleep(2);
                    Application.DoEvents();
                }
                MainForm.DisplayText("Done.");
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
                    MainForm.DisplayText("==> " + TxText, KnownColor.Blue);
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


        const int ReadBufferSize = 10000;
        private string RxString = string.Empty;

        // The DataReceived() routine is called when a charater is received from the serial port.
        // The data is assumed to be ASCII, terminated with \n or \n\r
        // When \n received, calls Cnc.LineReceived(), without termination character

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

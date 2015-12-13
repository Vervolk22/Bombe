using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Windows;

namespace ComputingHelpers
{
    /// <summary>
    /// The parent class for all, server and client socket workers. 
    /// Contains generic behavior. Only for using in WPF.
    /// </summary>
    public abstract class SocketWorker
    {
        protected int CHECK_ALIVE_DELAY = 1000;

        protected byte[] buffer = new byte[1024];
        protected System.Timers.Timer aliveTimer;

        /// <summary>
        /// Check alive connection/connections. Will be called by a timer.
        /// Needs to be overriden.
        /// </summary>
        /// <param name="source">Calling source (timer).</param>
        /// <param name="e">ElapsedEventArgs of a timer.</param>
        protected abstract void checkAlive(object source, System.Timers.ElapsedEventArgs e);

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="window">Main window of the application.</param>
        public SocketWorker()
        {
            aliveTimer = new System.Timers.Timer(CHECK_ALIVE_DELAY);
            aliveTimer.Elapsed += new System.Timers.ElapsedEventHandler(checkAlive);
            aliveTimer.Enabled = true;
        }

        /// <summary>
        /// Converts string to array of bytes.
        /// </summary>
        /// <param name="str">String to convert.</param>
        /// <returns>Resulting array of bytes.</returns>
        protected static byte[] getBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        /// <summary>
        /// Converts a single byte to byte[1] array;
        /// </summary>
        /// <param name="b">Byte to convert.</param>
        /// <returns>Resulting array of bytes.</returns>
        protected static byte[] getBytes(byte b)
        {
            byte[] bytes = new byte[1];
            bytes[0] = b;
            return bytes;
        }

        /// <summary>
        /// Gets string from the array of bytes.
        /// </summary>
        /// <param name="bytes">Array of bytes to convert.</param>
        /// <param name="iRx">Number of bytes to get from the array.</param>
        /// <returns>Resulting string.</returns>
        protected static string getString(byte[] bytes, int iRx)
        {
            char[] chars = new char[iRx / sizeof(char)];
            if (iRx % 2 == 1) iRx -= 1;
            System.Buffer.BlockCopy(bytes, 0, chars, 0, iRx);
            return new string(chars);
        }

        /// <summary>
        /// Checks, if the given socket's connection is alive.
        /// </summary>
        /// <param name="socket">Socket to check.</param>
        /// <returns>Result of check.</returns>
        protected static bool isAlive(Socket socket)
        {
            return !(socket.Poll(0, SelectMode.SelectRead) && socket.Available == 0);
        }

        /// <summary>
        /// Gets local ip address of the computer.
        /// </summary>
        /// <returns>Actually local ip adress.</returns>
        public static string getLocalIP()
        {
            IPAddress[] addr = Dns.GetHostAddresses(Dns.GetHostName());
            if (addr.Length > 1)
                return addr[1].ToString();
            else
                return addr[0].ToString();
        }

        /// <summary>
        /// Send data via socket.
        /// </summary>
        /// <param name="socket">Socket to use for sending data.</param>
        /// <param name="s">Data to send.</param>
        public void sendData(Socket socket, string s)
        {
            try
            {
                sendInfoMessageToForm("Message sent: " + s + "\n");
                byte[] byData = getBytes(s);
                // First byte of every message is it's length.
                socket.Send(getBytes(byData.Length.ToString("000")));
                socket.Send(byData);
            }
            catch (Exception se)
            {
                //sendInfoMessageToForm(se.Message);
            }
        }

        /// <summary>
        /// Receive data via socket.
        /// </summary>
        /// <param name="socket">Socket to listen for data.</param>
        /// <returns>Received message.</returns>
        public string receiveData(Socket socket)
        {
            try
            {
                sendInfoMessageToForm("Message received:\n");
                // First byte of every message is it's length.
                int iRx = socket.Receive(buffer, 6, SocketFlags.None);
                string str = getString(buffer, iRx);
                iRx = socket.Receive(buffer, Int32.Parse(str), SocketFlags.None);
                str = getString(buffer, iRx);
                sendInfoMessageToForm("---" + str + '\n');
                return str;
            }
            catch (Exception se)
            {
                //sendInfoMessageToForm(se.Message);
                return null;
            }
        }

        /// <summary>
        /// Split received command into parts, using predefined delimiter.
        /// </summary>
        /// <param name="s">Received string.</param>
        /// <returns>Resulting delimeted commands.</returns>
        protected string[] getCommand(string s)
        {
            if (s == null)
            {
                return new string[] { null };
            }
            return s.Split(':');
        }

        /// <summary>
        /// Send message to main window, about changes at socket level.
        /// </summary>
        /// <param name="s">Message to send.</param>
        public abstract void sendInfoMessageToForm(string s);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ComputingHelpers;

namespace BombeClient
{
    internal static class Bridge
    {
        public static new ComputingExecutor computingSide { get; private set; }
        public static new ClientSocketWorker socketWorker { get; private set; }
        public static new MainWindow window { get; private set; }

        public static string getPortText()
        {
            return window.port.Text;
        }

        public static string getIpText()
        {
            return window.iplabel.Text;
        }

        internal static void changeConnectionStatus(bool isOnline)
        {
            if (isOnline)
            {
                window.status.Content = "connected";
                window.status.Foreground = System.Windows.Media.Brushes.Green;
                window.cmdReceiveConnections.Content = "Disconnect";
            }
            else
            {
                window.status.Content = "not connected";
                window.status.Foreground = System.Windows.Media.Brushes.Red;
                window.cmdReceiveConnections.Content = "Connect";
            }
        }

        /// <summary>
        /// Send message to main window, about changes at socket level.
        /// </summary>
        /// <param name="s">Message to send.</param>
        public static void sendInfoMessageToForm(string s)
        {
            try
            {
                window.Dispatcher.Invoke((Action)(() =>
                {
                    window.mainlist.AppendText(s);
                    window.mainlist.Focus();
                    window.mainlist.CaretIndex = window.mainlist.Text.Length;
                    window.mainlist.ScrollToEnd();
                }));
            }
            catch (Exception e)
            {

            }
        }

        public static void setWindow(MainWindow windowIn)
        {
            window = windowIn;
        }

        public static void setComputingSide(ComputingExecutor side)
        {
            computingSide = side;
        }

        public static void setSocketWorker(ClientSocketWorker worker)
        {
            socketWorker = worker;
        }
    }
}

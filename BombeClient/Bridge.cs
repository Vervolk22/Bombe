using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ComputingHelpers;

namespace BombeClient
{
    /// <summary>
    /// Pattern, to bind togetger MainWindow, ComputingScheduler and 
    /// ServerSocketWorker.
    /// </summary>
    internal static class Bridge
    {
        public static ComputingExecutor computingSide { get; private set; }
        public static ClientSocketWorker socketWorker { get; private set; }
        public static MainWindow window { get; private set; }

        /// <summary>
        /// Get port from MainWindow and return it as string.
        /// </summary>
        /// <returns>Port from MainWindow.</returns>
        public static string getPortText()
        {
            return window.port.Text;
        }

        /// <summary>
        /// Get ip address from MainWindow and return it as string.
        /// </summary>
        /// <returns>Ip address from MainWindow.</returns>
        public static string getIpText()
        {
            return window.iplabel.Text;
        }

        /// <summary>
        /// Change current state of program.
        /// </summary>
        /// <param name="isOnline">Should program be in online status.</param>
        internal static void changeConnectionStatus(bool isOnline)
        {
            window.Dispatcher.Invoke((Action)(() =>
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
                }));
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

        /// <summary>
        /// Set window of current project.
        /// </summary>
        /// <param name="windowIn">MainWindow to set.</param>
        public static void setWindow(MainWindow windowIn)
        {
            window = windowIn;
        }

        /// <summary>
        /// Set ComputingSide of current project.
        /// </summary>
        /// <param name="side">Computing side to set.</param>
        public static void setComputingSide(ComputingExecutor side)
        {
            computingSide = side;
        }

        /// <summary>
        /// Set SocketWorker of current project.
        /// </summary>
        /// <param name="worker">SocketWorker to set.</param>
        public static void setSocketWorker(ClientSocketWorker worker)
        {
            socketWorker = worker;
        }
    }
}

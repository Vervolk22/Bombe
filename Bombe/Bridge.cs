using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ComputingHelpers;

namespace Bombe
{
    /// <summary>
    /// Pattern, to bind togetger MainWindow, ComputingExecutor and 
    /// ClientSocketWorker.
    /// </summary>
    internal static class Bridge
    {
        public static ComputingScheduler computingSide { get; private set; }
        public static ServerSocketWorker socketWorker { get; private set; }
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
        /// Change current state of program.
        /// </summary>
        /// <param name="isOnline">Should program be in online status.</param>
        public static void changeConnectionStatus(bool isOnline)
        {
            window.Dispatcher.Invoke((Action)(() =>
                {
                    if (isOnline)
                    {
                        window.status.Content = "online";
                        window.status.Foreground = System.Windows.Media.Brushes.Green;
                        window.cmdReceiveConnections.Content = "Close all connections";
                    }
                    else
                    {
                        window.status.Content = "offline";
                        window.status.Foreground = System.Windows.Media.Brushes.Red;
                        window.cmdReceiveConnections.Content = "Start receive connections";
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
        /// Transfer newClientConnected event from ServerSocketWorker to
        /// ComputingScheduler.
        /// </summary>
        /// <param name="socket"></param>
        internal static void newClientConnected(Socket socket)
        {
            computingSide.newClient(socket);
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
        public static void setComputingSide(ComputingScheduler side)
        {
            computingSide = side;
        }

        /// <summary>
        /// Set SocketWorker of current project.
        /// </summary>
        /// <param name="worker">SocketWorker to set.</param>
        public static void setSocketWorker(ServerSocketWorker worker)
        {
            socketWorker = worker;
        }
    }
}

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
    internal static class Bridge
    {
        public static ComputingScheduler computingSide { get; private set; }
        public static ServerSocketWorker socketWorker { get; private set; }
        public static MainWindow window { get; private set; }

        public static string getPortText()
        {
            return window.port.Text;
        }

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

        internal static void newClientConnected(Socket socket)
        {
            computingSide.newClient(socket);
        }

        public static void setWindow(MainWindow windowIn)
        {
            window = windowIn;
        }

        public static void setComputingSide(ComputingScheduler side)
        {
            computingSide = side;
        }

        public static void setSocketWorker(ServerSocketWorker worker)
        {
            socketWorker = worker;
        }
    }
}

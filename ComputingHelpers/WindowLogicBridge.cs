using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ComputingHelpers
{
    /// <summary>
    /// Pattern, to bind togetger Window and logic.
    /// </summary>
    public static class WindowLogicBridge
    {
        
        public static ComputingSide computingSide { get; private set; }
        public static SocketWorker socketWorker { get; private set; }
        public static Window window { get; private set; }

        /// <summary>
        /// Set window of current project.
        /// </summary>
        /// <param name="windowIn">MainWindow to set.</param>
        public static void setWindow(Window windowIn)
        {
            window = windowIn;
        }

        /// <summary>
        /// Set ComputingSide of current project.
        /// </summary>
        /// <param name="side">Computing side to set.</param>
        public static void setComputingSide(ComputingSide side)
        {
            computingSide = side;
        }

        /// <summary>
        /// Set SocketWorker of current project.
        /// </summary>
        /// <param name="worker">SocketWorker to set.</param>
        public static void setSocketWorker(SocketWorker worker)
        {
            socketWorker = worker;
        }
    }
}

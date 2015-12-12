using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ComputingHelpers
{
    public static class WindowLogicBridge
    {
        public static ComputingSide computingSide { get; private set; }
        public static SocketWorker socketWorker { get; private set; }
        public static Window window { get; private set; }

        public static void setWindow(Window windowIn)
        {
            window = windowIn;
        }

        public static void setComputingSide(ComputingSide side)
        {
            computingSide = side;
        }

        public static void setSocketWorker(SocketWorker worker)
        {
            socketWorker = worker;
        }
    }
}

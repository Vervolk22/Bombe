using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Bombe
{
    class ComputingScheduler
    {
        private MainWindow window;
        private SocketWorker worker;
        private bool isServerRunning = false;

        public ComputingScheduler(MainWindow window)
        {
            this.window = window;
            worker = new SocketWorker(window);
        }

        public void changeServerStatus()
        {
            if (isServerRunning)
            {
                worker.closeAllConnections();
            }
            else
            {
                worker.waitForConnections();
            }
            isServerRunning = !isServerRunning;
        }

        public string getLocalIP()
        {
            return worker.getLocalIP();
        }
    }
}

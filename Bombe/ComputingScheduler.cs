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

        public ComputingScheduler(MainWindow window)
        {
            this.window = window;
            worker = new SocketWorker();
        }

        internal void run()
        {
            //window.mainlist.AppendText("PTER)");
            SocketWorker worker = new SocketWorker();
            //worker.start();
        }

        public string getLocalIP()
        {
            return worker.getLocalIP();
        }
    }
}

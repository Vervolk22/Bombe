using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombeClient
{
    class ComputingExecutor
    {
        private MainWindow window;
        private ClientSocketWorker worker;
        private bool isConnected = false;

        public ComputingExecutor(MainWindow window)
        {
            this.window = window;
            worker = new ClientSocketWorker(window);
        }

        public void changeClientStatus()
        {
            if (isConnected)
            {
                worker.closeConnection();
            }
            else
            {
                worker.establishConnection();
            }
            isConnected = !isConnected;
        }
    }
}

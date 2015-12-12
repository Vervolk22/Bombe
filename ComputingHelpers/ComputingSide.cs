using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ComputingHelpers
{
    public abstract class ComputingSide
    {
        public ComputingSide(/*WindowLogicBridge bridge*/)
        {
            //this.bridge = bridge;
            //this.bridge.setComputingSide(this);
        }

        public string getLocalIP()
        {
            return SocketWorker.getLocalIP();
        }

        protected string[] getCommand(string s)
        {
            if (s == null)
            {
                return new string[] { null };
            }
            return s.Split(':');
        }
    }
}

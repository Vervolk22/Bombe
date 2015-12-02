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
        protected Window window;
        protected SocketWorker worker;

        protected abstract void sendMessageToForm(string s);

        public ComputingSide(Window window)
        {
            this.window = window;
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

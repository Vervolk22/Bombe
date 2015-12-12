using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ComputingHelpers
{
    /// <summary>
    /// Contains common methods for Computing scheduler and executor.
    /// </summary>
    public abstract class ComputingSide
    {
        /// <summary>
        /// Get local ip addres from SocketWorker.
        /// </summary>
        /// <returns>String - local ip address.</returns>
        public string getLocalIP()
        {
            return SocketWorker.getLocalIP();
        }

        /// <summary>
        /// Split received command into parts, using predefined delimiter.
        /// </summary>
        /// <param name="s">Received string.</param>
        /// <returns>Resulting delimeted commands.</returns>
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

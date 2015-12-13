using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bombe
{
    /// <summary>
    /// Represents a single client.
    /// </summary>
    public class Client
    {
        public int id { get; protected set; }
        public int computed { get; protected set; }
        public string ip { get; protected set; }
        public DateTime connectionTime { get; protected set; }
        public int totalCores { get; protected set; }
        public int connectedCores { get; protected set; }

        public Client(int id, string ip, DateTime time, int totalCores)
        {
            this.id = id;
            this.computed = 0;
            this.ip = ip;
            this.connectionTime = time;
            this.totalCores = totalCores;
            this.connectedCores = 1;
        }

        public void increaseComputed()
        {
            computed++;
        }

        public void increaseConnectedCores()
        {
            connectedCores++;
        }
    }
}
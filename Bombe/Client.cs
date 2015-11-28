using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bombe
{
    public class Client
    {
        public int id { get; private set; }
        public int computed { get; private set; }
        public string ip { get; private set; }

        public Client(int id, string ip)
        {
            this.id = id;
            this.computed = 0;
            this.ip = ip;
        }

        public void increaseComputed()
        {
            computed++;
        }
    }
}

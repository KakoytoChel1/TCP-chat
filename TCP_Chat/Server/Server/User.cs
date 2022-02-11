using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class User
    {
        private TcpClient client;
        private string name;

        public TcpClient Client
        {
            get { return client; }
            set { client = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}

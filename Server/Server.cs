using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Server : BaseScript
    {
        public Server()
        {

        }

        [EventHandler("pspray:server_ping")]
        private void test()
        {
            Debug.WriteLine("I'm inside the server?");
            TriggerClientEvent("pspray:client_pong");
        }
    }
}

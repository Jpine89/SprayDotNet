using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Client : BaseScript
    {
        public Client()
        {

        }

        [EventHandler("client_pong")]
        private void clientTest()
        {
            Debug.WriteLine("Server has Ponged me");
        }

        [Command("ping")]
        private void ping()
        {
            Debug.WriteLine("Calling Server with a Ping");
            TriggerServerEvent("server_ping");
        }
    }
}

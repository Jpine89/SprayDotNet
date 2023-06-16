using CitizenFX.Core;
using System;
using System.Collections.Generic;

namespace Client
{
    public class Client : BaseScript
    {
        public Client()
        {

        }

        [EventHandler("pspray:client_pong")]
        private void clientTest()
        {
            Debug.WriteLine("Client/Server has Ponged me");
        }

        [Command("ping")]
        private void ping(int src, List<object> args, string raw)
        {
            Debug.WriteLine(raw);
            Debug.WriteLine("Calling Server with a Ping");
            TriggerServerEvent("pspray:server_ping");
        }

        [Command("clientping")]
        private void ClientPing()
        {
            TriggerEvent("pspray:client_pong");
        }


        [Command("oldspawn")]
        private void testspawn()
        {
            Debug.WriteLine("Inside Old Spawn");
            Exports["spawnmanager"].spawnPlayer(new
            {
                x = 0,
                y = 0,
                z = 0,
                model = "s_m_y_cop_01"
            });
        }
    }
}

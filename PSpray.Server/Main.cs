using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using Dapper;
using PSpray.Server.Utils;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PSpray.Server
{
    class Main : BaseScript
    {
        Dictionary<string, string> playerList = new Dictionary<string, string>(); 
        public Main()
        {
            EventHandlers["init_player"] += new Action<Player>(InitPlayer);
            EventHandlers["pspray:add_spray"] += new Action<Player, string, string>(AddSpray);

            Debug.WriteLine("Server Init");
            
            //initConnectAsync();
        }

        //private async Task TestDapper()
        //{
        //    DynamicParameters test = new DynamicParameters();
        //}

        private async Task initConnectAsync()
        {
            using (var connection = Database.GetConnection())
            {
                int rowsAffected = await connection.ExecuteAsync(Queries.createPSprayTable);
                Debug.WriteLine($"Server Init rowsAffected: {rowsAffected}");
            }
        }

        private async void AddSpray([FromSource] Player source, string test, string test2)
        {
            Debug.WriteLine("AddSpray worked?");
        }

        private async void InitPlayer([FromSource] Player source)
        {
            //GetPlayerIdentifier("test", 1);
            Debug.WriteLine("Calling Init Player!");
            Debug.WriteLine("Player Ped: " + source.Character.NetworkId);
            //playerList.Add(source.Name, source.)
            //var parameters = new { Identifier = source.Identifiers["license"], Money = 5000, XP = 0 };
            //using (var connection = Database.GetConnection())
            //{
            //    int rowsAffected = await connection.ExecuteAsync(Queries.initPlayer, parameters);
            //    Debug.WriteLine($"Player Init rowsAffected: {rowsAffected}");
            //}
        }
    }
}

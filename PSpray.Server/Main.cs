using CitizenFX.Core;
using Dapper;
using PSpray.Server.Utils;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using PSpray.Server.Entities;
using Newtonsoft.Json;

namespace PSpray.Server
{
    class Main : BaseScript
    {
        Dictionary<string, string> playerList = new Dictionary<string, string>(); 
        public Main()
        {
            //I'm making a simple change to showcase a Pull Request

            EventHandlers["pspray:add_spray"] += new Action<Player, string>(AddSpray);
            EventHandlers["pspray:remove_sprays"] += new Action<Player, string>(RemoveSpray);
            EventHandlers["pspray:get_sprays"] += new Action(GetSprays);
            PSprayDbInitialize();
            Debug.WriteLine("PServer Init");
        }

        private async Task PSprayDbInitialize()
        {
            using (var connection = Database.GetConnection())
            {
                int rowsAffected = await connection.ExecuteAsync(Queries.createPSprayTable);
                Debug.WriteLine($"Server Init rowsAffected: {rowsAffected}");
            }
        }

        private async void AddSpray([FromSource] Player source, string obj)
        {
            Debug.WriteLine(obj);
            SprayTag spray = JsonConvert.DeserializeObject<SprayTag>(obj);
            var parameters = new { 
                Identifier = source.Identifiers["license"], 
                Locx = spray.Location.X, 
                Locy = spray.Location.Y, 
                Locz = spray.Location.Z,
                Rotx = spray.Rotation.X, 
                Roty = spray.Rotation.Y, 
                Rotz = spray.Rotation.Z,
                Scale = spray.Scale.X - 2f,
                spray.Color,
                spray.Text,
                spray.Font
            };

            using (var connection = Database.GetConnection())
            {
                int rowsAffected = await connection.ExecuteAsync(Queries.insertPSprayToTable, parameters);
                Debug.WriteLine($"Player Init rowsAffected: {rowsAffected}");
            }

            GetSprays();
        }

        private async void RemoveSpray([FromSource] Player source, string obj)
        {
            //Debug.WriteLine(obj);
            SprayTag spray = JsonConvert.DeserializeObject<SprayTag>(obj);
            
            var parameters = new
            {
                Id = spray.Id
            };
            using (var connection = Database.GetConnection())
            {
                int rowsAffected = await connection.ExecuteAsync(Queries.removePSprayFromTable, parameters);
                Debug.WriteLine($"Player Init rowsAffected: {rowsAffected}");
            }

            GetSprays();
        }

        private async void GetSprays()
        {
            IList<SprayTag> sprays = new List<SprayTag>();
            float scaleTemp;
            using (var connection = Database.GetConnection())
            {
                var reader = connection.ExecuteReader(Queries.getPSprayFromTable);
                while (reader.Read())
                {
                    scaleTemp = 2f + (int)reader[8];
                    sprays.Add(new SprayTag()
                    {
                        Id = (int)reader[0],
                        Location = new Vector3((float)reader[2], (float)reader[3], (float)reader[4]),
                        Rotation = new Vector3((float)reader[5], (float)reader[6], (float)reader[7]),
                        Scale = new Vector3(scaleTemp, scaleTemp, 0),
                        Color = (string)reader[9],
                        Font = (string)reader[11],
                        Text = (string)reader[10]
                    });

                }
                //Debug.WriteLine($"Player Init rowsAffected: {rowsAffected}");
            }
            string jsonSpray = JsonConvert.SerializeObject(sprays);
            BaseScript.TriggerClientEvent("pspray:List_Spray", jsonSpray);
            //Debug.WriteLine($"{jsonSpray}");
        }

    }
}

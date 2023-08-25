using CitizenFX.Core;
using Dapper;
using PSpray.Server.Utils;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using PSpray.Server.Entities;
using Newtonsoft.Json;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace PSpray.Server
{
    class Main : BaseScript
    {
        Dictionary<string, string> playerList = new Dictionary<string, string>();
        List<StreetNode> NodesList;
        List<FivemObj> ObjList;
        public Main()
        {
            _ = PSprayDbInitialize();
            _ = PTurfDbInitialize();

            NodesList = new();
            ObjList = new();
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
            //PSpray Events
            EventHandlers["pspray:add_spray"] += new Action<Player, string>(AddSpray);
            EventHandlers["pspray:remove_sprays"] += new Action<Player, string>(RemoveSpray);
            EventHandlers["pspray:get_sprays"] += new Action(GetSprays);

            //PTurf Events
            EventHandlers["pspray:street_data"] += new Action<string>(WriteDataToFile);
            EventHandlers["pspray:finish_data"] += new Action(FinishData);
            EventHandlers["pspray:check_data"] += new Action(CheckCount);

            EventHandlers["pspray:add_turf"] += new Action<Player, string>(AddTurf);

            EventHandlers["pspray:get_turf"] += new Action(GetTurfs);
            EventHandlers["pspray:get_dump"] += new Action(GetListOfObject);

            Debug.WriteLine("PServer Init");


            //GetTurfs();
        }

        private void OnClientResourceStart(string resourceName)
        {
            //if (GetCurrentResourceName() != resourceName) return;
            Debug.WriteLine($"The resource {resourceName} has been started on the client.");
            //CreateListOfObject();
        }

        private void GetListOfObject()
        {
            string filePath = "DumpsterData.json"; // Replace with the actual file path
            string jsonContent = File.ReadAllText(filePath);         
            BaseScript.TriggerClientEvent("pspray:List_Dump", jsonContent);
        }


        private void CheckCount()
        {
            Debug.WriteLine($"Count is: {NodesList.Count}");
        }

        private async void FinishData()
        {
            string jsonData = JsonConvert.SerializeObject(NodesList);
            File.WriteAllText("output5.json", jsonData);
            Debug.WriteLine("Data pushed to Output5.json");
        }

        private async void WriteDataToFile(string jsonData)
        {
            Debug.WriteLine("Data Received");
            //Debug.WriteLine(jsonData);
            List<StreetNode> Nodes = JsonConvert.DeserializeObject<List<StreetNode>>(jsonData);
            NodesList = NodesList.Concat(Nodes).ToList();
            Debug.WriteLine($"Current NodeList Count: {NodesList.Count}");
        }

        #region PTRUF
        private async Task PTurfDbInitialize()
        {
            using (var connection = Database.GetConnection())
            {
                int rowsAffected = await connection.ExecuteAsync(Queries.createPTurfTable);
                Debug.WriteLine($"Server Init Pturf rowsAffected: {rowsAffected}");
            }
        }
        private async void AddTurf([FromSource] Player source, string obj)
        {
            Debug.WriteLine("Create Turf was Called");
            Debug.WriteLine(obj);
            TurfNode turf = JsonConvert.DeserializeObject<TurfNode>(obj);
            var parameters = new
            {
                Identifier = source.Identifiers["license"],
                Name = turf.Name,
                Nodes = JsonConvert.SerializeObject(turf.NodeList)
            };

            using (var connection = Database.GetConnection())
            {
                int rowsAffected = await connection.ExecuteAsync(Queries.insertPTurfToTable, parameters);
                Debug.WriteLine($"Player Init rowsAffected: {rowsAffected}");
            }

            GetTurfs();
        }

        private void RemoveTurf([FromSource] Player source, string obj)
        {

        }

        private async void GetTurfs()
        {
            IList<TurfNode> turfs = new List<TurfNode>();
            using (var connection = Database.GetConnection())
            {
                var reader = connection.ExecuteReader(Queries.getPTurfFromTable);
                while (reader.Read())
                {
                    //Debug.WriteLine($"reader[3] : {reader[3]}");

                    turfs.Add(new TurfNode()
                    {
                        Name = reader.GetString(2),
                        //NodeList = null
                        NodeList = JsonConvert.DeserializeObject<List<Vector3>>(reader.GetString(3))
                    });

                }
                //Debug.WriteLine($"Player Init rowsAffected: {rowsAffected}");
            }
            string jsonTurf = JsonConvert.SerializeObject(turfs);
            BaseScript.TriggerClientEvent("pspray:List_Turf", jsonTurf);
            //Debug.WriteLine($"{jsonSpray}");
        }
        #endregion

        #region PSPRAY

        private async Task PSprayDbInitialize()
        {
            using (var connection = Database.GetConnection())
            {
                int rowsAffected = await connection.ExecuteAsync(Queries.createPSprayTable);
                Debug.WriteLine($"Server Init Pspray rowsAffected: {rowsAffected}");
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
        #endregion
    }
}

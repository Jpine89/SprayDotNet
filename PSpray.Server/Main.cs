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
        public Main()
        {

            //BaseScript.GetStreetNameAtCoord();
            //EventHandlers["pspray:add_spray"] += new Action<Player, string>(AddSpray);
            //EventHandlers["pspray:remove_sprays"] += new Action<Player, string>(RemoveSpray);
            //EventHandlers["pspray:get_sprays"] += new Action(GetSprays);
            //PSprayDbInitialize();
            //Debug.WriteLine("PServer Init");
            NodesList = new();
            EventHandlers["pspray:street_data"] += new Action<string>(WriteDataToFile);
            EventHandlers["pspray:finish_data"] += new Action(FinishData);
            EventHandlers["pspray:check_data"] += new Action(CheckCount);
            //PTurfCreateMap();


            EventHandlers["pspray:create_turf"] += new Action<string>(CreateTurf);
        }

        private async Task PTurfCreateMap()
        {
            string filePath = "output.json"; // Replace with the actual file path
            string jsonContent = File.ReadAllText(filePath);

            List<StreetNode> NodesList = JsonConvert.DeserializeObject<List<StreetNode>>(jsonContent);
            Debug.WriteLine("Data Generated to Local Memory");

            Bitmap image = GenerateImage(NodesList);

        }

        private void CreateTurf(string jsonData)
        {
            Debug.WriteLine("Create Turf was Called");
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
            //NodesList = new();
            //image.Save(imagePath, ImageFormat.Png);
        }

        private async void WriteDataToFile(string jsonData)
        {
            Debug.WriteLine("Data Received");
            //Debug.WriteLine(jsonData);
            List<StreetNode> Nodes = JsonConvert.DeserializeObject<List<StreetNode>>(jsonData);
            NodesList = NodesList.Concat(Nodes).ToList();
            Debug.WriteLine($"Current NodeList Count: {NodesList.Count}");
        }

        private Bitmap GenerateImage(List<StreetNode> rootNode)
        {
            Debug.WriteLine("Processing Data pt1");
            //int width = 8800;  // Set the image width
            //int height = 12400; // Set the image height

            int width = 400;  // Set the image width
            int height = 800; // Set the image height

            Bitmap image = new(width, height);
            Debug.WriteLine("Processing Data pt2");
            using (Graphics graphics = Graphics.FromImage(image))
            {
                // Clear the image with a white background
                graphics.Clear(Color.White);
                foreach (StreetNode node in rootNode)
                // Call a recursive method to draw nodes and subnodes
                    DrawNode(graphics, node, 0, 0, width, height);
            }


            return image;
        }

        private void DrawNode(Graphics graphics, StreetNode node, int x, int y, int width, int height)
        {
            // Calculate the position based on the node's coordinates and the available space
            int nodeX = x + node.CoordX;    // Adjust coordinates to fit within the image
            int nodeY = y + node.CoordY;   // Adjust coordinates to fit within the image

            // Parse the color from the HTML color code
            Color color = ColorTranslator.FromHtml(node.Color);

            // Draw a colored rectangle representing the node
            using (Brush brush = new SolidBrush(color))
            {
                graphics.FillRectangle(brush, nodeX, nodeY, 10, 10); // Adjust the size as needed
            }

            // Recursively draw subnodes
            //foreach (var subnode in node.Subnodes)
            //{
            //    DrawNode(graphics, subnode, nodeX, nodeY, width, height);
            //}
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

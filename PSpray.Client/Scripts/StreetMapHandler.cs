using Newtonsoft.Json;
using PSpray.Client.Entities;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace PSpray.Client.Scripts
{
    internal class StreetMapHandler
    {
        private static readonly object _padlock = new();
        private static StreetMapHandler _instance;
        private Dictionary<string, string> colorList = new Dictionary<string, string>();
        HashSet<string> usedColors = new HashSet<string>();
        List<string> usedColorList = new List<string>();
        
        Random random = new Random();


        private List<StreetNode> nodes = new List<StreetNode>();
        private List<StreetNode> _tempNodesList = new List<StreetNode>();
        private StreetNode _tempStreetNode;

        private int _x, _y;
        private int _min = -4400;
        private int _maxX = 4400;
        private int _maxY = 8000;
        private bool _freeze = false;

        private uint StoreStreetUint = 0;
        private uint streetNameUINT = 0;
        private uint crossRoadUINT = 0;
        private string _StreetName;
        private string _CornerName;
        private string crossRoad;
        private string _StreetColor = "";
        private HashSet<string> usedCoords = new HashSet<string>();


        private int overlayHandle;
        private bool firstTick;

        const string CLIENT_CONFIG_LOCATION = $"GangTurf.json";

        private StreetMapHandler()
        {
            Init();
            Debug.WriteLine("^2PStreets Online.");
        }

        internal static StreetMapHandler Instance
        {
            get
            {
                lock (_padlock)
                {
                    return _instance ??= new StreetMapHandler();
                }
            }
        }

        private async void Init()
        {
            SetupEventHandler();
            SetupRegisterCommands();

            string serverConfigFile = LoadResourceFile(GetCurrentResourceName(), CLIENT_CONFIG_LOCATION);
            //Debug.WriteLine($"gangConfig : {serverConfigFile}");
            nodes = JsonConvert.DeserializeObject<List<StreetNode>>(serverConfigFile);
            //Main.Instance.AttachTick(loadGangOnMap);

        }

        private async Task loadGangOnMap()
        {
            overlayHandle = AddMinimapOverlay("gang_areas.gfx");

            while (!HasMinimapOverlayLoaded(overlayHandle))
            {
                Debug.WriteLine("Waiting..Waiting...");
                await BaseScript.Delay(50);
            }

            Debug.WriteLine($"{overlayHandle}");
            AddGangColor("AMBIENT_GANG_BALLAS", 255, 255, 0);
            int count = 0;
            foreach (var node in nodes)
            {
                CustomAddGangArea(node.CoordX, node.CoordY, node.CoordX + 1, node.CoordY + 1, $"Test{count}");
                Debug.WriteLine($"Cords: [{node.CoordX},{node.CoordY}] :: [{node.CoordX + 1},{node.CoordY + 1}]");
                SetGangAreaOwner($"Test{count}", "AMBIENT_GANG_BALLAS");
                count++;
            }

            //CustomAddGangArea(0, 0, 100, 100, $"Test{count}");
            //SetGangAreaOwner($"Test{count}", "AMBIENT_GANG_BALLAS");

            //"AMBIENT_GANG_MEXICAN", Color.FromArgb(255, 255, 255, 0)
            Debug.WriteLine($"done; count {count}");

            KillTic();
        }

        private async Task KillTic()
        {
            Main.Instance.DetachTick(loadGangOnMap);
        }

        private void SetupEventHandler()
        {

        }


        private void SetupRegisterCommands()
        {
            RegisterCommand("streets1", new Action(mycords1), false);
            RegisterCommand("streets2", new Action(mycords2), false);
            RegisterCommand("streets3", new Action(mycords3), false);
            RegisterCommand("streets4", new Action(mycords4), false);
            RegisterCommand("streets5", new Action(mycords5), false);
            RegisterCommand("streets6", new Action(mycords6), false);

            RegisterCommand("streets7", new Action(mycords7), false);
            RegisterCommand("streets8", new Action(mycords8), false);
            RegisterCommand("streets9", new Action(mycords9), false);
            //RegisterCommand("streets10", new Action(mycords10), false);
            RegisterCommand("finish", new Action(finish), false);
            RegisterCommand("count", new Action(count), false);
            RegisterCommand("pos", new Action(pos), false);
            RegisterCommand("fake", new Action(fake), false);
            RegisterCommand("start", new Action(Start), false);
            RegisterCommand("small", new Action(StartSmall), false);
            RegisterCommand("send", new Action(Send), false);
            RegisterCommand("stop", new Action(Stop), false);
            RegisterCommand("load", new Action(Load), false);

        }

        private async void finish()
        {
            BaseScript.TriggerServerEvent("pspray:finish_data");
        }


        private async void count()
        {
            BaseScript.TriggerServerEvent("pspray:check_data");
        }

        private async void fake()
        {
            uint streetNameUINT = 0;
            uint crossRoadUINT = 0;
            Vector3 pos = GetEntityCoords(PlayerPedId(), true);
            GetStreetNameAtCoord(2500, 1530, 0, ref streetNameUINT, ref crossRoadUINT);
            Debug.WriteLine($"Player POS: {pos.X}, {pos.Y}, Street: {GetStreetNameFromHashKey(streetNameUINT)}");
            Debug.WriteLine($"Secondary: {GetStreetNameFromHashKey(crossRoadUINT)}");
        }

        private async void pos()
        {
            uint streetNameUINT = 0;
            uint crossRoadUINT = 0;
            Vector3 pos = GetEntityCoords(PlayerPedId(), true);
            GetStreetNameAtCoord(pos.X, pos.Y, 0, ref streetNameUINT, ref crossRoadUINT);
            Debug.WriteLine($"Player POS: {pos.X}, {pos.Y}, Street: {GetStreetNameFromHashKey(streetNameUINT)}");
            Debug.WriteLine($"Secondary: {GetStreetNameFromHashKey(crossRoadUINT)}");
            Debug.WriteLine($"Node Count: {nodes.Count}");
            Debug.WriteLine($"Are We Inside?: {IsInside(pos)}");
        }

        public bool IsInside(Vector3 pos)
        {
            bool inside = false;
            int numVertices = nodes.Count;

            for (int i = 0, j = numVertices - 1; i < numVertices; j = i++)
            {
                if (((nodes[i].CoordY > pos.Y) != (nodes[j].CoordY > pos.Y)) &&
                    (pos.X < (nodes[j].CoordX - nodes[i].CoordX) * (pos.Y - nodes[i].CoordY) / (nodes[j].CoordY - nodes[i].CoordY) + nodes[i].CoordX))
                {
                    inside = !inside;
                }
            }

            return inside;
        }

        private async void Send()
        {
            Main.Instance.AttachTick(SendData);
        }

        private async void Load()
        {
            Main.Instance.DetachTick(SmallLoop);
            Main.Instance.AttachTick(loadGangOnMap);
        }

        private async void Stop()
        {
            Main.Instance.DetachTick(SmallLoop);
        }

        private async void StartSmall()
        {
            Vector3 pos = GetEntityCoords(PlayerPedId(), true);
            _x = (int)Math.Round(pos.X);
            _y = (int)Math.Round(pos.Y);
            GetStreetNameAtCoord(pos.X, pos.Y, 0, ref StoreStreetUint, ref crossRoadUINT);
            _StreetName = GetStreetNameFromHashKey(StoreStreetUint);
            _CornerName = GetStreetNameFromHashKey(crossRoadUINT);
            _StreetColor = RandomColor(random);
            _tempStreetNode = new() {
                Name = _StreetName,
                Color = _StreetColor,
                CoordX = _x,
                CoordY = _y,
                Subnode = _CornerName
            };
            _tempNodesList.Add(_tempStreetNode);
            _tempStreetNode = null;
            Main.Instance.AttachTick(SmallLoop);
        }

        private async Task SmallLoop()
        {
            Debug.WriteLine($"I'm Inside Small loop, Count is: {_tempNodesList.Count} vs {nodes.Count}");
            if (_tempNodesList.Count == 0)
                Main.Instance.DetachTick(SmallLoop);

            //Logic, instead of doing large parts of the map, we just create nodes and colors for specific streets at a time.
            if (!GetNameOfZone(_tempNodesList[0].CoordX, _tempNodesList[0].CoordY, 0).Equals("OCEANA"))
            {
                await ArrayToLoop(_tempNodesList[0]);
                nodes.Add(_tempNodesList[0]);
            }

            
            _tempNodesList.Remove(_tempNodesList[0]);
        }

        private async Task ArrayToLoop(StreetNode node)
        {
            //Debug.WriteLine($"[{nodeX},{nodeY}]");
            for (int x = node.CoordX - 1; x <= node.CoordX + 1; x++)
            {
                for (int y = node.CoordY - 1; y <= node.CoordY + 1; y++)
                {
                    if (!usedCoords.Contains($"{x},{y}"))
                    {
                        GetStreetNameAtCoord(x, y, 0, ref streetNameUINT, ref crossRoadUINT);
                        //Debug.WriteLine($"{streetNameUINT} v {StoreStreetUint}");
                        if (streetNameUINT == StoreStreetUint)
                        {
                            _StreetName = GetStreetNameFromHashKey(streetNameUINT);
                            _CornerName = GetStreetNameFromHashKey(crossRoadUINT);
                            _tempStreetNode = new()
                            {
                                Name = _StreetName,
                                CoordX = x,
                                CoordY = y,
                                Subnode = _CornerName,
                                Color = _StreetColor
                            };

                            if (_tempNodesList.Contains(_tempStreetNode))
                            {
                                Debug.WriteLine($"I'm totally inside... {_tempStreetNode}");
                                continue;
                            }
                            else
                            {
                                usedCoords.Add($"{x},{y}");
                                _tempNodesList.Add(_tempStreetNode);
                            }
                            
                        }
                    }
                }
            }

        }

        private async void Start()
        {
            _x = _min;
            Main.Instance.AttachTick(CreateNodeList);
            //Main.Instance.AttachTick(CreateNodeList2);
        }

        private async Task CreateNodeList()
        {
            if (_x >= _maxX)
            {
                Main.Instance.DetachTick(CreateNodeList);
            }

            if(nodes.Count > 1000)
                await SendData();
            await LoopWork();
            _x = _x + 10;
            

        }

        private async Task LoopWork()
        {
            for (int y = _min; y < 8000; y += 10)
            {

                if (!GetNameOfZone(_x, y, 0).Equals("OCEANA"))
                {
                    GetStreetNameAtCoord(_x, y, 0, ref streetNameUINT, ref crossRoadUINT);
                    if (streetNameUINT != 0)
                    {
                        StreetNode tempNode = new StreetNode();
                        string color = "";
                        _StreetName = GetStreetNameFromHashKey(streetNameUINT);
                        //if (crossRoadUINT != 0)
                        //    crossRoad = GetStreetNameFromHashKey(crossRoadUINT);
                        if (colorList.ContainsKey(_StreetName))
                        {
                            color = colorList[_StreetName];
                        }
                        else
                        {
                            color = RandomColor(random);
                            colorList.Add(_StreetName, color);
                            usedColors.Add(color);
                        }

                        tempNode.Name = _StreetName;
                        tempNode.Color = color;
                        tempNode.CoordX = _x;
                        tempNode.CoordY = y;
                        Debug.WriteLine($"Street: {_StreetName}, Coords: [{_x},{y}], Color: {color}");
                        nodes.Add(tempNode);
                        tempNode = null;
                        color = "";
                    }
                }
                else
                {
                    Debug.WriteLine($"2: [X: {_x}, Y: {y}]");
                }

            }
        }

        private async Task CreateNodeMap(int minY, int maxY)
        {
            
            
            uint streetNameUINT = 0;
            uint crossRoadUINT = 0;
            //Dictionary<string, StreetNode> streetInNodeList = new Dictionary<string, StreetNode>();
            string jsonData = "";
            string color = "";
            //int count = 1;

            //for (int y = minY; y < maxY; y += 10)
            for (int y = 0; y < 3000; y += 1)
            {
                
                for (int x = 0; x < 4400; x += 1)
                {
                    StreetNode tempNode = new StreetNode();
                    //Debug.WriteLine($"{x},{y}");
                    if (GetNameOfZone(x, y, 0).Equals("OCEANA"))
                    {
                        //if(count == 1)
                        //{
                        //    usedColors.Add("#0046ff");
                        //    count++;
                        //}
                        //tempNode.Name = "OCEANA";
                        //tempNode.Color = "0046ff";
                        //tempNode.CoordX = x;
                        //tempNode.CoordY = y;

                        //nodes.Add(tempNode);
                        continue;
                    }
                        
                    string streetName;
                    string crossRoad;
                    GetStreetNameAtCoord(x, y, 0, ref streetNameUINT, ref crossRoadUINT);
                    if (streetNameUINT != 0)
                    {
                        streetName = GetStreetNameFromHashKey(streetNameUINT);
                        //if (crossRoadUINT != 0)
                        //    crossRoad = GetStreetNameFromHashKey(crossRoadUINT);
                        if (colorList.ContainsKey(streetName))
                        {
                            color = colorList[streetName];
                        }
                        else
                        {
                            color = RandomColor(random);
                            colorList.Add(streetName, color);
                            usedColors.Add(color);
                        }

                        tempNode.Name = streetName;
                        tempNode.Color = color;
                        tempNode.CoordX = x;
                        tempNode.CoordY = y;

                        nodes.Add(tempNode);
                        tempNode = null;
                        color = "";
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            Main.Instance.AttachTick(SendData);
        }

        private async Task SendData()
        {
            //for(int c = 0; c < 3; c++)
            //{
            Debug.WriteLine($"I'm Sending, Count: {nodes.Count}");
                int stop = nodes.Count - 3000 > 0 ? nodes.Count - 3000 : 0;
                if (nodes.Count >= 0)
                {
                    List<StreetNode> _TempNodes = new List<StreetNode>();
                    for (int i = nodes.Count - 1; i >= stop; i--)
                    {
                        if (i < 0)
                            continue;
                        Debug.WriteLine($"{i}");
                        _TempNodes.Add(nodes[i]);
                        nodes.RemoveAt(i);

                    }

                    if (_TempNodes.Count > 0)
                    {
                        Debug.WriteLine($"About to Send Packets, Number of Items: {_TempNodes.Count}");
                        string jsonData = JsonConvert.SerializeObject(_TempNodes, Formatting.Indented);
                        //byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonData);
                        //Debug.WriteLine($"jsonData is : {jsonBytes.Length} bytes");
                        BaseScript.TriggerServerEvent("pspray:street_data", jsonData);
                        Debug.WriteLine($"Packets Sent.");
                    }
                }
                else
                {
                    Debug.WriteLine("We are Empty?");
                    nodes.Clear();
                    //_y++;
                    //Main.Instance.DetachTick(SendData);
                }
            //}

            await BaseScript.Delay(5000);
        }

        private string RandomColor(Random random)
        {
            string colorCode = "";
            int red = random.Next(256);    // Random value between 0 and 255
            int green = random.Next(256);  // Random value between 0 and 255
            int blue = random.Next(256);   // Random value between 0 and 255

            colorCode = $"#{red:X2}{green:X2}{blue:X2}";            
            return colorCode;
        }

        private void SetGangAreaOwner(string name, string owner)
        {
            CallMinimapScaleformFunction(overlayHandle, "SET_GANG_AREA_OWNER");
            PushScaleformMovieFunctionParameterString(name);
            PushScaleformMovieFunctionParameterString(owner);
            PopScaleformMovieFunctionVoid();
        }


        private void CustomAddGangArea(float x1, float y1, float x2, float y2, string name)
        {
            //float x1, x2, x3, x4, y1, y2, y3, y4 = 5;
            CallMinimapScaleformFunction(overlayHandle, "ADD_GANG_AREA");
            PushScaleformMovieFunctionParameterFloat(x1);
            PushScaleformMovieFunctionParameterFloat(y1);
            PushScaleformMovieFunctionParameterFloat(x2);
            PushScaleformMovieFunctionParameterFloat(y2);
            PushScaleformMovieFunctionParameterString(name);
            PopScaleformMovieFunctionVoid();
        }

        private void AddGangColor(string identifier, byte r, byte g, byte b)
        {
            CallMinimapScaleformFunction(overlayHandle, "ADD_GANG_COLOR");
            PushScaleformMovieFunctionParameterString(identifier);
            PushScaleformMovieFunctionParameterInt(r);
            PushScaleformMovieFunctionParameterInt(g);
            PushScaleformMovieFunctionParameterInt(b);
            PopScaleformMovieFunctionVoid();
        }


        private async void mycords1()
        {
            await CreateNodeMap(-4400, -3000);
        }
        private async void mycords2()
        {
            await CreateNodeMap(-3000, -1500);
        }
        private async void mycords3()
        {
            await CreateNodeMap(-1500, 0000);
        }
        private async void mycords4()
        {
            await CreateNodeMap(0000, 1500);
        }
        private async void mycords5()
        {
            await CreateNodeMap(1500, 3000);
        }
        private async void mycords6()
        {
            await CreateNodeMap(3000, 4500);
        }
        private async void mycords7()
        {
            await CreateNodeMap(4500, 6000);
        }
        private async void mycords8()
        {
            await CreateNodeMap(6000, 7500);
        }
        private async void mycords9()
        {
            await CreateNodeMap(7500, 8100);
        }
        //private async void mycords10()
        //{
        //    await CreateNodeMap(-4400, -2000);
        //}
    }
}

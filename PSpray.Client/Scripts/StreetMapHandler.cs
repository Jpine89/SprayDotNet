using Newtonsoft.Json;
using PSpray.Client.Entities;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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
        //waypoint id
        private const int blipWPid = 8;
        //point of interest id
        private const int blipPOIid = 162;

        private List<Vector3> blipList = new List<Vector3>();
        private List<int> blipListId = new List<int>();
        private Vector3 lastSavedBlip;



        private int overlayHandle;
        private bool firstTick;
        private dynamic list;

        const string CLIENT_CONFIG_LOCATION = $"GroveStData.json";
        HashSet<int> bins = new HashSet<int>();
        List<FivemObj> ObjList;
        List<TurfNode> TurfList;
        string TurfName = "";

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
            ObjList = new();
            TurfList = new();
            SetupEventHandler();
            SetupRegisterCommands();


            //bins.Add(-206690185);
            //bins.Add(682791951);
            //bins.Add(218085040);
            //bins.Add(1511880420);
            //bins.Add(666561306);
            //bins.Add(-58485588);


            BaseScript.TriggerServerEvent("pspray:get_dump");
            BaseScript.TriggerServerEvent("pspray:get_turf");
        }



        private async Task KillTic()
        {
            
            Main.Instance.DetachTick(loadGangOnMap);
            Main.Instance.DetachTick(loadTurfOnMap);
            Main.Instance.DetachTick(IsPlayNearObject);
        }

        private void SetupEventHandler()
        {
            Main.Instance.EventHandlerDictionary.Add("pspray:List_Dump", new Action<string>(ListDump));
            Main.Instance.EventHandlerDictionary.Add("pspray:List_Turf", new Action<string>(GetTurfs));
        }


        private void SetupRegisterCommands()
        {
            RegisterCommand("finish", new Action(finish), false);
            RegisterCommand("count", new Action(count), false);
            RegisterCommand("stop", new Action(Stop), false);
            RegisterCommand("load", new Action(Load), false);


            RegisterCommand("NewTurf", new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (args.ToList().Count() > 0)
                {
                    string name = "";
                    foreach(var item in args)
                    {
                        name = $"{name} {item}";
                    }
                    //Debug.WriteLine($"{name}");
                    NewTurf(name.Trim());
                }
            }), false);


            RegisterCommand("Drawblip", new Action(DrawBlip), false);
            RegisterCommand("AddTurf", new Action(AddTurf), false);

            //GetGamePool
            RegisterCommand("FindObj", new Action(FindObj), false);
        }
        private void FindObj()
        {
            Debug.WriteLine("Starting");
            Main.Instance.AttachTick(IsPlayNearObject);
        }

        private async void ListDump(string data)
        {
            ObjList = JsonConvert.DeserializeObject<List<FivemObj>>(data);
            //foreach(var obj in ObjList)
            //{
            //    AddBlipForCoord(obj.Position.X, obj.Position.Y, obj.Position.Z);
            //}
        }


        private async void NewTurf(string turfName)
        {
            TurfName = turfName;
            Main.Instance.AttachTick(TurfTask);
        }

        private async Task TurfTask()
        {

            var blip = GetFirstBlipInfoId(blipWPid);
            var newPoint = GetBlipCoords(blip);
            if(newPoint != new Vector3() { X = 0, Y = 0, Z = 0 } && newPoint != lastSavedBlip)
            {
                blipList.Add(newPoint);
                lastSavedBlip = newPoint;
                int i = AddBlipForCoord(newPoint.X, newPoint.Y, newPoint.Z);
                blipListId.Add(i);
            }

        }

        private void DrawBlip()
        {
            Main.Instance.AttachTick(loadTurfOnMap);
        }


        private void AddTurf()
        {
            TurfNode newTurf = new();
            newTurf.Name = TurfName;
            newTurf.NodeList = blipList;
            string jsonData = JsonConvert.SerializeObject(newTurf, Formatting.Indented);
            BaseScript.TriggerServerEvent("pspray:add_turf", jsonData);
            TurfName = ""; blipList.Clear();
            Main.Instance.DetachTick(TurfTask);
        }

        private void GetTurfs(string data)
        {
            TurfList = JsonConvert.DeserializeObject<List<TurfNode>>(data);
            foreach(var node in TurfList)
            {
                foreach(var pos in node.NodeList)
                {
                    AddBlipForCoord(pos.X, pos.Y, pos.Z);
                }
            }
        }


        private async Task IsPlayNearObject()
        {
            if (IsInside(Game.PlayerPed.Position))
            {
                foreach (var obj in ObjList)
                {
                    if (IsInside(obj.Position))
                    {
                        //Entity entity = GetClosestObjectOfType(pedCoord.X, pedCoord.Y, pedCoord.Z, 5.0f, (uint)b, false, false, false);
                        //AddBlipForCoord(_entity.);
                        int i = AddBlipForCoord(obj.Position.X, obj.Position.Y, obj.Position.Z);
                        SetBlipColour(i, 43);
                    }
                }
            }

            await BaseScript.Delay(2000);
        }

        public bool IsInside(Vector3 pos)
        {
            bool inside = false;
            foreach (var node in TurfList)
            {
                int numVertices = node.NodeList.Count;
                for (int i = 0, j = numVertices - 1; i < numVertices; j = i++)
                {
                    if (((node.NodeList[i].Y > pos.Y) != (node.NodeList[j].Y > pos.Y)) &&
                        (pos.X < (node.NodeList[j].X - node.NodeList[i].X) * (pos.Y - node.NodeList[i].Y) / (node.NodeList[j].Y - node.NodeList[i].Y) + node.NodeList[i].X))
                    {
                        inside = !inside;
                    }
                }
            }
            return inside;
        }

        #region Proof Of Concept Ideas
        private async void finish()
        {
            BaseScript.TriggerServerEvent("pspray:finish_data");
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
        private async void fake()
        {
            uint streetNameUINT = 0;
            uint crossRoadUINT = 0;
            Vector3 pos = GetEntityCoords(PlayerPedId(), true);
            GetStreetNameAtCoord(2500, 1530, 0, ref streetNameUINT, ref crossRoadUINT);
            Debug.WriteLine($"Player POS: {pos.X}, {pos.Y}, Street: {GetStreetNameFromHashKey(streetNameUINT)}");
            Debug.WriteLine($"Secondary: {GetStreetNameFromHashKey(crossRoadUINT)}");
        }
        private async void count()
        {
            BaseScript.TriggerServerEvent("pspray:check_data");
        }

        private async void Send()
        {
            Main.Instance.AttachTick(SendData);
        }

        private async void Load()
        {

            string serverConfigFile = LoadResourceFile(GetCurrentResourceName(), CLIENT_CONFIG_LOCATION);
            //Debug.WriteLine($"gangConfig : {serverConfigFile}");
            nodes = JsonConvert.DeserializeObject<List<StreetNode>>(serverConfigFile);
            //Main.Instance.AttachTick(loadGangOnMap);


            foreach(var node in nodes)
            {
                int i = AddBlipForCoord(node.CoordX, node.CoordY, 0f);
                SetBlipColour(i, 27);
            }

            //Main.Instance.DetachTick(SmallLoop);
            //Main.Instance.AttachTick(loadGangOnMap);
        }

        private async void Stop()
        {
            Main.Instance.DetachTick(SmallLoop);
            Main.Instance.DetachTick(SendData);
            Main.Instance.DetachTick(TurfTask);
            Main.Instance.DetachTick(loadTurfOnMap);
            
            if (blipListId.Count > 0)
            {
                int blip;
                for (int i = 0; i < blipListId.Count; i++)
                {
                    blip = blipListId[i];
                    RemoveBlip(ref blip);
                }
            }

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
                int stop = nodes.Count - 2000 > 0 ? nodes.Count - 2000 : 0;
                if (nodes.Count >= 0)
                {
                    List<StreetNode> _TempNodes = new List<StreetNode>();
                    for (int i = nodes.Count - 1; i >= stop; i--)
                    {
                        if (i < 0)
                            continue;
                        //Debug.WriteLine($"{i}");
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

            await BaseScript.Delay(3500);
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

        private async Task loadTurfOnMap()
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
            if (blipList.Count > 2)
            {
                for (int i = 0; i < blipList.Count; i++)
                {
                    if (i + 1 == blipList.Count)
                    {
                        CustomAddGangArea(blipList[i].X, blipList[i].Y, blipList[0].X, blipList[0].Y, $"Turf{count}");
                    }
                    else
                    {
                        CustomAddGangArea(blipList[i].X, blipList[i].Y, blipList[i + 1].X, blipList[i + 1].Y, $"Turf{count}");
                    }

                    //Debug.WriteLine($"Cords: [{node.CoordX},{node.CoordY}] :: [{node.CoordX + 1},{node.CoordY + 1}]");
                    SetGangAreaOwner($"Turf{count}", "AMBIENT_GANG_BALLAS");
                    count++;
                }


            }
            KillTic();
        }
        #endregion
    }
}

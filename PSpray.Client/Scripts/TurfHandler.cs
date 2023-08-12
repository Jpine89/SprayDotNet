using PSpray.Client.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSpray.Client.Scripts
{
    internal class TurfHandler
    {
        private static readonly object _padlock = new();
        private static TurfHandler _instance;

        private TurfHandler()
        {
            Init();
            Debug.WriteLine("^2PTurf Handler has been initialised.");
        }

        internal static TurfHandler Instance
        {
            get
            {
                lock (_padlock)
                {
                    return _instance ??= new TurfHandler();
                }
            }
        }

        private async void Init()
        {
            SetupEventHandler();
            SetupRegisterCommands();
        }

        private void SetupEventHandler()
        {
            //Main.Instance.EventHandlerDictionary.Add("pspray:Init_Spray", new Action(InitSpray));
        }


        private void SetupRegisterCommands()
        {
            RegisterCommand("PTurf", new Action(PTurfTest), false);
            RegisterCommand("PDump", new Action(PDumpTest), false);
        }

        public void PTurfTest()
        {
            uint streetName = 0;
            uint crossingRoad = 0;
            Debug.WriteLine("Test");
            //Vector3 playerCoords = GetEntityCoords(-1, true);

            //Vector3 playerPos = player.Character.Position;
            Vector3 playerCoords = Game.PlayerPed.Position;

            Debug.WriteLine($"Your Coords Are: {playerCoords}");
            GetStreetNameAtCoord(playerCoords.X, playerCoords.Y,playerCoords.Z, ref streetName, ref crossingRoad);
            Debug.WriteLine($"(StreetName),(crossingRoad): {streetName},{crossingRoad} ");
        }

        private void PDumpTest()
        {
            //'prop_skip_05a',
            //'prop_dumpster_3a':-206690185
            //'prop_skip_08a',
            //'prop_dumpster_4b',:682791951
            //'prop_bin_14a',
            //'prop_skip_03',
            //'prop_dumpster_01a',:218085040
            //'prop_dumpster_4a',:1511880420
            //'prop_skip_10a',
            //'prop_dumpster_02b',:-58485588
            //'prop_bin_14b',
            //'prop_skip_06a',
            //'prop_dumpster_02a',:666561306
            //'prop_bin_07a',
            //'prop_skip_02a',
            List<int> bins = new List<int>();
            bins.Add(-206690185);
            bins.Add(682791951);
            bins.Add(218085040);
            bins.Add(1511880420);
            bins.Add(666561306);
            bins.Add(-58485588);
            int obj = 0;

            var pedCoord = GetEntityCoords(PlayerPedId(), true);
            Debug.WriteLine($"Ped Coords: {pedCoord}");
            int hash = 0;
            //Use to find the closet dumpster
            foreach (var b in bins) {
                //uint test = (uint)i;
                //Debug.WriteLine($"{b} vs {test}");
                obj = GetClosestObjectOfType(pedCoord.X, pedCoord.Y, pedCoord.Z, 5.0f, (uint)b, false, false, false);
                Debug.WriteLine($"This obj: {obj}");
                hash = b;
                if (obj != 0)
                    break;
            }
            //Notes - Back side of Dumpster is considered the face for EntityHeading
            Debug.WriteLine($"Entity Heading: {GetEntityHeading(obj)}"); 
            Debug.WriteLine($"Player Heading: {GetEntityHeading(PlayerPedId())}");
            CreateTurfSpray(obj, hash);

        }

        private async void CreateTurfSpray(int obj, int objHash)
        {

            //Vector3 Objmin = new Vector3();
            //Vector3 Objmax = new Vector3();

            //GetModelDimensions((uint)objHash, ref Objmin, ref Objmax);


            //Debug.WriteLine($"the Obj Dim: {Objmin} || {Objmax}");
            //Vector3 cords = GetEntityCoords(obj, false);
            //Vector3 rot = new Vector3() { X = 0, Y = 0, Z=0};
            //rot = GetEntityRotation(obj, 2);
            //Debug.WriteLine($"Cords: {cords} and Rot: {rot}");

            //Debug.WriteLine("------------------TEST 1 MOTHERFUCKER----------------------");

            //var degrees = GetEntityHeading(obj) + 180;
            //float normalizedDegrees = (degrees) % 360f;

            //Debug.WriteLine($"{normalizedDegrees}");
            //var radians = await getRadians(normalizedDegrees);
            //float x = (float)Math.Cos(radians);
            //float y = (float)Math.Sin(radians);
            //Debug.WriteLine($"For an angle of {radians} radians:");
            //Debug.WriteLine($"x = {x}");
            //Debug.WriteLine($"y = {y}");
            //Debug.WriteLine("------------------TEST 2 MOTHERFUCKER----------------------");
            //cords.Y = cords.Y - (y * (Objmin.Y));
            //cords.X = cords.X - (x * (Objmin.X));
            //cords.Z = cords.Z + 0.8f;
            ////rot.Z = (-1 * rot.Z);

            //turfSpray.Location = cords;
            //Debug.WriteLine($"SprayCords : {turfSpray.Location} and SprayRot : {turfSpray.Rotation}");
            //turfSpray.Rotation = rot;


            SprayTag turfSpray = new SprayTag();
            var pedCoord = GetEntityCoords(PlayerPedId(), true);
            //Prop dumpster = World.GetClosest(pedCoord, World.GetAllProps());
            Entity dumpster = Entity.FromHandle(obj);
            Vector3 localOffset = new() { X = -0.5f, Y = 0.0f, Z = 0.5f };


            Matrix worldMatrix = new(
                                    dumpster.ForwardVector.X, dumpster.ForwardVector.Y, dumpster.ForwardVector.Z, 0,
                                    dumpster.RightVector.X, dumpster.RightVector.Y, dumpster.RightVector.Z, 0,
                                    dumpster.UpVector.X, dumpster.UpVector.Y, dumpster.UpVector.Z, 0,
                                    dumpster.Position.X, dumpster.Position.Y, dumpster.Position.Z, 1
                                    );

            Vector3 position = Vector3.TransformCoordinate(localOffset, worldMatrix);
            Debug.WriteLine($"Prop:: {dumpster.Handle}");
            Debug.WriteLine($"transform Position: {position}");
            Vector3 rotation = dumpster.Rotation;
            rotation.Z = -1 * rotation.Z;
            rotation.X = 0;
            rotation.Y = 0;
            Debug.WriteLine($"Rotation:: {rotation}");
            turfSpray.Location = position;
            turfSpray.Rotation = rotation;
            SprayTagHandler.Instance.CreateSpray("-----------------------------Gang Turf-----------------------------", true, turfSpray);
        }

        private async Task<float> getRadians(float degree)
        {


            return degree * (float)Math.PI / 180; // Replace with the desired radians value
        }
    }
}

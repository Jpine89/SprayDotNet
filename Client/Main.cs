using System;
using System.Collections.Generic;
using Client.Util;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using Client.Functions;

namespace Client
{
    class Main : BaseScript
    {
        private bool _disposed = false;
        public static Entity HitEntity { get; set; }
        public Main()
        {
            
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);

            RegisterCommand("info", new Action<int, List<object>, string>((source, args, raw) =>
            {
                string compiled = "";
                foreach (string item in args)
                    compiled = compiled + " " + item;
                InfoCommand(compiled);
                // TODO: make a vehicle! fun!
                //TriggerEvent("chat:addMessage", new
                //{
                //    color = new[] { 255, 0, 0 },
                //    args = new[] { "[CarSpawner]", $"I wish I could spawn this {(args.Count > 0 ? $"{args[0]} or" : "")} adder but my owner was too lazy. :(" }
                //});
            }), false);

            RegisterCommand("dui", new Action(CreateDui), false);
            RegisterCommand("remove", new Action(RemoveSpray), false);
            RegisterCommand("test", new Action<string>(TestCommand), false);
            RegisterCommand("exit", new Action(EndTest), false);
            RegisterCommand("Decal", new Action(DecalCommand), false);
        }

        private static void RayCastGamePlayCamera(ref Vector3 endPoint, ref Vector3 rotation)
        {
            
            float distance = 10f;
            var cameraRotation = GetGameplayCamRot(0); //if ped not working try 0
            var cameraCoord = GetGameplayCamCoord();
            var dir = RotationToDirection(cameraRotation);

            Vector3 Des = new Vector3()
            {
                X = cameraCoord.X + dir.X * distance,
                Y = cameraCoord.Y + dir.Y * distance,
                Z = cameraCoord.Z + dir.Z * distance
            };

            var Ray = StartShapeTestRay(cameraCoord.X, cameraCoord.Y, cameraCoord.Z, Des.X, Des.Y, Des.Z, -1, PlayerPedId(), 0);

            int entityHandleArg = 0;
            bool hitSomethingArg = false;
            Vector3 hitPositionArg = new Vector3(); //EndCoords
            Vector3 surfaceNormalArg2 = new Vector3();

            uint materialArg = 0;
            int testee = GetShapeTestResultEx(Ray, ref hitSomethingArg, ref hitPositionArg, ref surfaceNormalArg2, ref materialArg, ref entityHandleArg);

            if (testee == 2)
            {
                endPoint = hitPositionArg;
                rotation = surfaceNormalArg2;
                Debug.WriteLine("hitsomething: " + hitSomethingArg.ToString());
                Debug.WriteLine("Hitsomething coords: " + hitPositionArg.ToString());
                Debug.WriteLine("surface: " + surfaceNormalArg2.ToString());
            }
        }

        private static Vector3 RotationToDirection(Vector3 rotation)
        {
            float pi = (float)Math.PI / 180f;
            float retZ = rotation.Z * pi;
            float retX = rotation.X * pi;
            float absX = Math.Abs((float)Math.Cos(retX));

            var Dir = new Vector3()
            {
                X = ((float)(Math.Sin(retZ)) * absX) * -1,
                Y = ((float)(Math.Cos(retZ)) * absX),
                Z = (float)Math.Sin(retX)
            };

            return Dir;
        }

        private void CreateDui()
        {

        }

        private async void DecalCommand()
        {
            Vector3 spradyData = new Vector3(16, 25, 73);
            //float uni = 0;
            Debug.WriteLine("Decal Called");
            //int decalType, float posX, float posY, float posZ,
            //float p4, float p5, float p6,
            //float p7, float p8, float p9,
            //float width, float height,
            //float rCoef, float gCoef, float bCoef,
            //float opacity, float timeout, bool p17, bool p18, bool p19

            RequestStreamedTextureDict("commonmenu", false);
            int i  = AddDecal(1110, 16, 25, 73,
                1f, 1f, 1f,//dir X/Y/Z
                0, 0, 0,
                2f, 4f, //width height
                1.0f, 1.0f, 1.0f, //RGB
                1.0f, -1f, //opac and time
                true, false, false); //Last bool is Car?
            Debug.WriteLine(i.ToString());
            PatchDecalDiffuseMap(1110, "commonmenu", "shop_box_cross");
            Debug.WriteLine(IsDecalAlive(i).ToString());

            while (true)
            {
                await Delay(0);
                PatchDecalDiffuseMap(1110, "commonmenu", "shop_box_cross");
                Debug.WriteLine(IsDecalAlive(i).ToString());

                Debug.WriteLine(GetEntityCoords(i, IsDecalAlive(i)).ToString());
            }

            //PatchDecalDiffuseMap(1110, "commonmenu", "shop_box_cross");


            //AddDecal(
            //    decalType,
            //    targetedCoord.x, targetedCoord.y, targetedCoord.z, --pos
            //    0.0, 0.0, -1.0, --unk
            //    func_522(0.0, 1.0, 0.0), --unk
            //    1.0, 1.0, --width, height
            //    0.196, 0.0, 0.0, --rgb
            //    1.0, -1.0, --opacity, timeout
            //    0, 0, 0-- unk
            //}
        }

        private void RemoveSpray()
        {
            //Has Info 
            //p_loose_rag_01_s  -> 921993182
            var ragProp = CreateObject(
                            921993182,
                            (float)0.0, (float)0.0, (float)0.0,
                            true, false, false
                            );

            var ped = PlayerPedId();
            var test = GetPedBoneIndex(ped, 28422);

            Debug.WriteLine(ped.ToString());
            Debug.WriteLine(test.ToString());

            AttachEntityToEntity(ragProp, ped, GetPedBoneIndex(ped, 28422), (float)0.0, (float)0.0, (float)0.0, (float)0.0, (float)0.0, (float)0.0, true, true, false, true, 1, true);
        }

        private void OnClientResourceStart(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;

            RegisterCommand("car", new Action<int, List<object>, string>((source, args, raw) =>
            {
                // TODO: make a vehicle! fun!
                TriggerEvent("chat:addMessage", new
                {
                    color = new[] { 255, 0, 0 },
                    args = new[] { "[CarSpawner]", $"I wish I could spawn this {(args.Count > 0 ? $"{args[0]} or" : "")} adder but my owner was too lazy. :(" }
                });
            }), false);
        }

        private async void InfoCommand(string userInput)
        {
            Spray spray = new Spray();
            spray.Text = userInput;
            spray.Color = "#FA1C09";
            string SprayUserData = $"<FONT color='{spray.Color}' FACE='Beat Street'> {spray.Text} ";


            Vector3 spradyData = new Vector3();
            Vector3 rotationData = new Vector3();
            RayCastGamePlayCamera(ref spradyData, ref rotationData);

            var scaleForm = RequestScaleformMovie("mp_big_message_freemode");
            while (!HasScaleformMovieLoaded(scaleForm))
            {
                await Delay(0);
            }

            PushScaleformMovieFunction(scaleForm, "SHOW_SHARD_WASTED_MP_MESSAGE");
            PushScaleformMovieFunctionParameterString(SprayUserData);
            PushScaleformMovieFunctionParameterString("Small Text");
            PushScaleformMovieFunctionParameterInt(5);
            PopScaleformMovieFunctionVoid();
            while (true)
            {
                await Delay(0);

                DrawScaleformMovie_3dSolid(
                                            scaleForm,
                                            spradyData.X, spradyData.Y, spradyData.Z, //16f, 25f, 73f,
                                            rotationData.X, rotationData.Y, rotationData.Z,
                                            //0,0,0,
                                            (float)1.0, (float)1.0, (float)1.0, //unk values
                                            (float)2.0, (float)2.0, (float)1.0, //Scale X/Y/Z
                                            2 //always 2?
                                            );
            }
        }

        private async void TestCommand(string testString)
        {
            _disposed = true;
            var test = GetGameplayCamCoords();
            var test2 = GetGameplayCamFov();
            var test3 = GetGameplayCamRot(0);
            //var alpha = 10 * math.floor(scale * 40);
            Debug.WriteLine(test.ToString());

            //var teest4 = CreateRuntimeTxd("test");

            RequestStreamedTextureDict("commonmenu", true);

            //var t = FindRaycastedSprayCoords();
            Vector3 spradyData = new Vector3(16, 25, 73);

            //PrivateFontCollection collection = new PrivateFontCollection();
            //collection.AddFontFile(@"C:\Projects\MyProj\free3of9.ttf");
            //FontFamily fontFamily = new FontFamily("Free 3 of 9", collection);
            //Font font = new Font(fontFamily, height);


            //RegisterFontFile();

            while (_disposed)
            {
                await Delay(0);
                SetTextColour(255, 0, 0, 0);

                //BeginTextCommandWidth("THREESTRINGS");
                //EndTextCommandGetWidth(1);

                //BeginTextCommandDisplayText("THREESTRINGS");
                //AddTextComponentString("TEST");
                //EndTextCommandDisplayText((float)0.50, (float)0.50);
                //EndTextCommandDisplayHelp();
                DrawSprite("commonmenu", "shop_box_cross", (float)0.10, (float)0.10, (float)0.50, (float)0.50, 0, 255, 255, 255, 150);
                //SetTextEntry("STRING");
                //AddTextComponentString("This is a test");
                //DrawText((float)0.50, (float)0.50);
            }

            //DrawSprite("dpscenes", "Circle", test.X, test.Y, 100, 100, 100, 255, 0, 0, 40);
        }

        private void EndTest()
        {
            _disposed = false;
        }
    }
}

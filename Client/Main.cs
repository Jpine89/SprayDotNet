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
            //RegisterCommand("info", new Action<string>(InfoCommand), false);

            RegisterCommand("Decal", new Action(DecalCommand), false);

            RegisterCommand("RayTrace", new Action(RayCastGamePlayCamera), false);
        }

        private async void RayCastGamePlayCamera()
        {
            
            float distance = 10f;
            //var cameraRotation = GetGameplayCamRot(GetPlayerPed(-1)); //if ped not working try 0
            var cameraRotation = GetGameplayCamRot(0); //if ped not working try 0
            Debug.WriteLine("Rot: " + cameraRotation.ToString());
            var cameraCoord = GetGameplayCamCoord();
            Debug.WriteLine("Coord: " + cameraCoord.ToString());

            float retz = cameraRotation.Z * 0.0174532924F;
            float retx = cameraRotation.X * 0.0174532924F;
            float absx = (float)Math.Abs(Math.Cos(retx));
            Vector3 camStuff = new Vector3((float)Math.Sin(retz) * absx * -1, (float)Math.Cos(retz) * absx,
                                               (float)Math.Sin(retx));
            Vector3 camstuffProjected = camStuff * distance;

            Debug.WriteLine("CamStuff: " + camstuffProjected);
            //int count = 0;
            //while (count < 10)
            //{
                await Delay(0);
                var Ray = StartShapeTestRay(cameraCoord.X, cameraCoord.Y, cameraCoord.Z, camstuffProjected.X, camstuffProjected.Y, camstuffProjected.Z, -1, PlayerPedId(), 0);
                //StartShapeTestLosProbe
                //int target = 100;
                int entityHandleArg = 0;
                bool hitSomethingArg = false;
                Vector3 hitPositionArg = new Vector3();
                Vector3 surfaceNormalArg = new Vector3();

                uint materialArg = 0;

                int result = GetShapeTestResult(Ray, ref hitSomethingArg, ref hitPositionArg, ref surfaceNormalArg, ref entityHandleArg);


                if (result == 2)
                {
                    Debug.WriteLine(hitSomethingArg.ToString());
                    Debug.WriteLine(hitPositionArg.ToString());
                    Debug.WriteLine(surfaceNormalArg.ToString());

                    var test = GetEntityType(entityHandleArg);
                    //HitEntity = Entity.FromHandle(entityHandleArg);
                    Debug.WriteLine(test.ToString());
                    Debug.WriteLine(((MaterialHash)materialArg).ToString());
                }

            //DrawMarker();
        }

        private Vector3 RotationToDirection(Vector3 rotation)
        {
            return rotation;
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
            //Font_Function testSpray = new Font_Function();
            Spray spray = new Spray();
            spray.Text = userInput;
            spray.Color = "#FA1C09";
            //spray.Font = "$" + testSpray.FontsList[13];

            //string SprayUserData = $"<FONT color='{spray.Color}' FACE='{spray.Font}'> {spray.Text} ";
            string SprayUserData = $"<FONT color='{spray.Color}' FACE='Beat Street'> {spray.Text} ";

            Vector3 spradyData = new Vector3(141.3313f, 1172.67f, 228.6097f);
            Vector3 currentComputedRotation = new Vector3(0, 0, 0);

            var ped = PlayerPedId();
            var cords = GetEntityCoords(ped, true);
            var test = GetGameplayCamCoords();
            Debug.WriteLine(test.ToString());

            var cameraRotation = GetGameplayCamRot(2);
            var cameraCoord = GetGameplayCamCoord();

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
                                            0f, 0f, 0f,
                                            (float)1.0,
                                            (float)1.0,
                                            (float)1.0,
                                            (float)2.0, (float)2.0,
                                            (float)1.0,
                                            2
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

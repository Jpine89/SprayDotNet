using CitizenFX.Core;
using CitizenFX.Core.UI;
using CitizenFX.Core.NaturalMotion;
using static CitizenFX.Core.Native.API;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Client.Util;

namespace Client
{
    class Main : BaseScript
    {
        private bool _disposed = false;
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


            RegisterCommand("remove", new Action(RemoveSpray), false);

            RegisterCommand("test", new Action<string>(TestCommand), false);
            RegisterCommand("exit", new Action(EndTest), false);
            //RegisterCommand("info", new Action<string>(InfoCommand), false);
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
            spray.Font = "$Font2";

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

            string SprayUserData = $"<FONT color='{spray.Color}' FACE='{spray.Font}'> {spray.Text} ";

            //Debug.WriteLine("Setting up sprady");
            Vector3 spradyData = new Vector3(16, 25, 73);
            Vector3 currentComputedRotation = new Vector3(0, 0, 0);
            Debug.WriteLine(userInput);
            Debug.WriteLine(SprayUserData);

            //Debug.WriteLine("Setting up this PushScale thing..");
            PushScaleformMovieFunction(scaleForm, "SHOW_SHARD_WASTED_MP_MESSAGE");
            //Debug.WriteLine("Setting up this PushScale thing..");
            PushScaleformMovieFunctionParameterString(SprayUserData);
            PushScaleformMovieFunctionParameterString("Small Text");
            PushScaleformMovieFunctionParameterInt(5);

            PopScaleformMovieFunctionVoid();


            while (true)
            {
                await Delay(0);


                //DrawScaleformMovieFullscreen(scaleForm, 255, 255, 255, 255, 0);

                //Debug.WriteLine("About to Draw..");
                DrawScaleformMovie_3dNonAdditive(
                                                1,
                                                spradyData.X, spradyData.Y, spradyData.Z,
                                                currentComputedRotation.X, currentComputedRotation.Y, currentComputedRotation.Z,
                                                (float)1.0,
                                                (float)1.0,
                                                (float)1.0,
                                                (float)2.0, (float)2.0,
                                                (float)1.0,
                                                2
                                            );
            }
            //var direction = RotationToDirection(cameraRotation);

            //var t = StartShapeTestRay();

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
            RegisterFontFile();

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

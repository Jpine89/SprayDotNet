using System;
using System.Collections.Generic;
using Client.Util;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using Client.Functions;
using System.Threading.Tasks;

namespace Client
{
    class Main : BaseScript
    {

        #region Variables
        #endregion


        public Main()
        {
            
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);

            //RegisterCommand("PSpray", new Action<int, List<object>, string>((source, args, raw) =>
            //{
            //    string compiled = "";
            //    foreach (string item in args)
            //        compiled = compiled + " " + item;

            //    sprayText = compiled;
            //    isSpray = true;
            //    //InfoCommand(compiled);
            //    // TODO: make a vehicle! fun!
            //    //TriggerEvent("chat:addMessage", new
            //    //{
            //    //    color = new[] { 255, 0, 0 },
            //    //    args = new[] { "[CarSpawner]", $"I wish I could spawn this {(args.Count > 0 ? $"{args[0]} or" : "")} adder but my owner was too lazy. :(" }
            //    //});
            //}), false);

        }



        #region Obselete Functions

        /*
         * This should be kept for looking into later
         * It's possible that this command may be lighter
         * then the ScaleForm, plus you can use this
         * in relation of vehicles. 
        */
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
            int i = AddDecal(1110, 16, 25, 73,
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


        /*
         * Set for Deletion later.
         * Was a reference for underestanding concept of OnClientResourceStart Functions.
        */
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

        /*
            * Obselete code from DPScene. Keeping for reference later.
            * Potential use for a "Cheap" Spray concept that wouldn't require
            * any heavy math or a scaleform?
        */
        private async void TestCommand(string testString)
            {
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

                while (true)
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
        #endregion
    }
}

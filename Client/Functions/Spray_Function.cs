using CitizenFX.Core;
using Client.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;

namespace Client.Functions
{
    class Spray_Function : BaseScript
    {
        private List<Spray> _sprays = new();
        private List<Spray> _spraysInRange = new();
        private Spray _tempSpray = null;

        private Dictionary<int, Scaleform> _scaleforms = new();
        private const string SCALEFORM_NAME = "PLAYER_NAME_";

        private const int PRIVATE_SPRAY_SCALEFORM_KEY = 15;
        private const int SCALEFORM_MAX_SCREEN = 12;
        private const float SCALEFORM_MAX_DISTANCE = 25f;

        private const float FORWARD_OFFSET = 0.015f;

        private Vector3 _sprayFinalRotation { get; set; }
        private int _camera;

        private int tracker = 1;

        public Spray_Function()
        {
            Init();


            //RegisterKeyMapping("+SaveSpray", "Used to Save Spray", "keyboard", "RETURN");
            //RegisterCommand("+SaveSpray", new Action(TempSave), false);
        }

        async void Init()
        {
            await LoadScaleForms();
            Tick += DrawSpraysInRangeAsync;
        }

        /// <summary>
        /// Creates all the Scaleforms which will be used for the spray text.
        /// </summary>
        /// <returns></returns>
        private async Task LoadScaleForms()
        {
            // 1 - 15 are what matches the PLAYER_NAME Scaleforms in the game.
            // TODO: Create own scaleform files for the spray text.
            for (int i = 1; i <= 15; i++)
            {
                Scaleform scaleform = new Scaleform($"{SCALEFORM_NAME}{i:00}");
                while (!scaleform.IsLoaded)
                {
                    await Delay(5);
                }
            }
            // All scaleforms are loaded
        }

        /// <summary>
        /// Gets the sprays in range of the player, and orders them by distance.
        /// </summary>
        /// <returns></returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task SpraysInRangeAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            _spraysInRange.Clear();

            Vector3 playerPos = Game.PlayerPed.Position;

            _spraysInRange = _sprays
                .Where(x => Vector3.Distance(playerPos, x.Location) < SCALEFORM_MAX_DISTANCE)
                .OrderBy(x => Vector3.Distance(playerPos, x.Location))
                .Take(SCALEFORM_MAX_SCREEN)
                .ToList();
        }

        /// <summary>
        /// Draws the sprays from _spraysInRange.
        /// </summary>
        /// <returns></returns>
        private async Task DrawSpraysInRangeAsync()
        {
            // Get the sprays in range
            await SpraysInRangeAsync();

            // foreach spray in range, draw it
            for (int i = 0; i < _spraysInRange.Count; i++)
            {
                Spray spray = _spraysInRange[i];
                spray.Scaleform = _scaleforms[i];
                spray.Draw();
            }
        }

        [Command("PSpray")]
        public void CommandSpray(int source, List<object> arguments, string raw)
        {
            string sprayText = "Spray Template " + tracker;

            if (arguments.Count > 0)
            {
                sprayText = arguments[0].ToString();
            }

            // Create the temporary spray
            if (_tempSpray == null)
            {
                _tempSpray = new Spray();
            }

            // Set the spray initial information (font, color, text)
            _tempSpray.Text = sprayText;

            // Set the spray scaleform to the private spray scaleform
            _tempSpray.Scaleform = _scaleforms[PRIVATE_SPRAY_SCALEFORM_KEY];

            // Check if the camera exists, if it does then destroy it
            if (DoesCamExist(_camera))
                DestroyCam(_camera, false);

            // Create the camera
            _camera = CreateCam("DEFAULT_SCRIPTED_CAMERA", false);

            // Start the tick event
            Tick += SetSprayPositionAsync;
        }

        private async Task SetSprayPositionAsync()
        {
            Vector3 coords = new();
            Vector3 rotation = new();

            RayCastGamePlayCamera(ref coords, ref rotation);

            await RunCameraMethod(coords, rotation);
            coords += (rotation * FORWARD_OFFSET);

            // update the spray position and rotation
            _tempSpray.Location = coords;
            _tempSpray.Rotation = _sprayFinalRotation;

            // draw the spray
            _tempSpray.Draw();

            // Check if the player has pressed the accept button (Enter)
            if (Game.IsControlJustPressed(0, Control.FrontendAccept))
            {
                // Add the spray to the list
                _sprays.Add(_tempSpray);

                // Reset the temp spray
                _tempSpray = null;

                // Check if the camera exists, if it does then destroy it
                if (DoesCamExist(_camera))
                    DestroyCam(_camera, false);

                // Remove the tick event
                Tick -= SetSprayPositionAsync;
            }
        }

        //public void TempSave()
        //{
        //    _sprays.Add(newSpray);
        //    newSpray = new Spray();
        //    isSpray = false;
        //    tracker++;
        //}


        //[EventHandler("pspray:start_spray")]
        //public void InitializeSpray()
        //{
        //    isSpray = true;
        //    sprayText = "Spray Template " + tracker;
        //    TriggerEvent("pspray:open_menu");
        //}

        //[EventHandler("pspray:SaveSpray")]
        //public void SaveSpray(string text, bool saveSpray)
        //{
        //    Debug.WriteLine("We are in SaveSpray");
        //    if (isSpray && saveSpray)
        //    {
        //        Spray_Text_Update(text);
        //        _sprays.Add(newSpray);
        //        newSpray = new Spray();
        //    }
        //    isSpray = false;
        //    tracker++;
        //}

        /*
         * ToDo
         * Add not only Functionality, but new class to handle this
         */
        //public void RemoveSpray()
        //{
        //    //Has Info 
        //    //p_loose_rag_01_s  -> 921993182
        //    var ragProp = CreateObject(
        //                    921993182,
        //                    (float)0.0, (float)0.0, (float)0.0,
        //                    true, false, false
        //                    );

        //    var ped = PlayerPedId();
        //    var test = GetPedBoneIndex(ped, 28422);

        //    Debug.WriteLine(ped.ToString());
        //    Debug.WriteLine(test.ToString());

        //    AttachEntityToEntity(ragProp, ped, GetPedBoneIndex(ped, 28422), (float)0.0, (float)0.0, (float)0.0, (float)0.0, (float)0.0, (float)0.0, true, true, false, true, 1, true);
        //}

        /*
         * Logic will want to be built out more on this when
         * Database Logic is being built. Plus NUI
         */

        //[EventHandler("pspray:spray_text_update")]
        //private void Spray_Text_Update(string text)
        //{
        //    sprayText = text;
        //}

        public void RayCastGamePlayCamera(ref Vector3 endPoint, ref Vector3 rotation)
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

            var Ray = StartShapeTestRay(cameraCoord.X, cameraCoord.Y, cameraCoord.Z, Des.X, Des.Y, Des.Z, -1, PlayerPedId(), 1);

            int entityHandleArg = 0;
            bool hitSomethingArg = false;
            Vector3 hitPositionArg = new Vector3(); //EndCoords
            Vector3 surfaceNormalArg2 = new Vector3();

            uint materialArg = 0;
            int result = GetShapeTestResultEx(Ray, ref hitSomethingArg, ref hitPositionArg, ref surfaceNormalArg2, ref materialArg, ref entityHandleArg);

            if (result == 2)
            {
                endPoint = hitPositionArg;
                rotation = surfaceNormalArg2;
            }
        }

        public static Vector3 RotationToDirection(Vector3 rotation)
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

        public async Task RunCameraMethod(Vector3 WantedLocation, Vector3 WantedRotation)
        {
            Vector3 currentSprayRotation = new Vector3();
            int reCheck = 30;

            if (WantedLocation != null && WantedRotation != null)
            {
                if (reCheck >= 0)
                    reCheck--;

                if (currentSprayRotation != WantedRotation || reCheck < 0)
                {
                    reCheck = 30;

                    var wantedSprayRotationFixed = new Vector3()
                    {
                        X = WantedRotation.X,
                        Y = WantedRotation.Y,
                        Z = WantedRotation.Z + 0.03f
                    };

                    var camLookPosition = WantedLocation - wantedSprayRotationFixed * 10;
                    SetCamCoord(_camera, WantedLocation.X, WantedLocation.Y, WantedLocation.Z);
                    PointCamAtCoord(_camera, camLookPosition.X, camLookPosition.Y, camLookPosition.Z);
                    SetCamActive(_camera, true);
                    await Delay(2);
                    var cameraRotation = GetCamRot(_camera, 2);
                    SetCamActive(_camera, false);

                    _sprayFinalRotation = cameraRotation;
                }
            }
        }

    }
}

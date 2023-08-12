using Newtonsoft.Json;
using PSpray.Client.Entities;
using System.Collections.Generic;
using System.Linq;

namespace PSpray.Client.Scripts
{
    internal class SprayTagHandler
    {
        private static readonly object _padlock = new();
        private static SprayTagHandler _instance;

        private List<SprayTag> _sprays = new();
        private List<SprayTag> _spraysInRange = new();
        private SprayTag _tempSpray = null;

        private Dictionary<int, Scaleform> _scaleforms = new();
        private const string SCALEFORM_NAME = "PLAYER_NAME_"; // PLAYER_NAME_01 - PLAYER_NAME_15 are the scaleforms used for the spray text.

        private const int PRIVATE_SPRAY_SCALEFORM_KEY = 11; // 15th scaleform PLAYER_NAME_15 is the private spray scaleform. -- 6/26 (14 doesnt work, made switch to 13, 7/4 13 doesnt work, switched to 11..)
        private const int SCALEFORM_MAX_SCREEN = 10; // 13th scaleform PLAYER_NAME_13 is the max screen scaleform (7/4 changed to load 10 max).
        private const float SCALEFORM_MAX_DISTANCE = 25f; // 25m is the max distance for the max screen scaleform.

        private const float FORWARD_OFFSET = 0.015f;
        private Vector3 _previousDistance;

        private Vector3 _sprayFinalRotation { get; set; }
        private int _camera;


        private bool MenuIsActive = false;
        //private TmcWrapper TmcWrapper;
        private SprayTagHandler()
        {
            Init();
            Debug.WriteLine("^2PSpray Tag Handler has been initialised.");
        }

        internal static SprayTagHandler Instance
        {
            get
            {
                lock (_padlock)
                {
                    return _instance ??= new SprayTagHandler();
                }
            }
        }

        private async void Init()
        {
            //TmcWrapper = new TmcWrapper();
            //Debug.WriteLine("Staring Init for PSpray");
            FrameWork();
            await LoadScaleFormsAsync();
            Main.Instance.AttachTick(DrawSpraysInRangeAsync);
            SetupEventHandler();
            SetupRegisterCommands();

            BaseScript.TriggerServerEvent("pspray:get_sprays");
        }

        private void SetupEventHandler()
        {
            Main.Instance.EventHandlerDictionary.Add("pspray:Init_Spray", new Action(InitSpray));
            Main.Instance.EventHandlerDictionary.Add("pspray:List_Spray", new Action<string>(ListSpray));
            Main.Instance.EventHandlerDictionary.Add("pspray:Save_Spray", new Action(SaveSpray));
            Main.Instance.EventHandlerDictionary.Add("pspray:Text_Spray", new Action<string>(SprayText));
            Main.Instance.EventHandlerDictionary.Add("pspray:Scale_Spray", new Action<float>(SprayScale));
            Main.Instance.EventHandlerDictionary.Add("pspray:Font_Spray", new Action<int>(SprayFont));
            Main.Instance.EventHandlerDictionary.Add("pspray:End_Spray", new Action(EndSprayCam));
            Main.Instance.EventHandlerDictionary.Add("pspray:Color_Spray", new Action<string>(SprayColor));
        }


        private void SetupRegisterCommands()
        {
            RegisterCommand("pspray", new Action(InitSpray), false);
            RegisterCommand("RemovePspray", new Action(RemoveSpray), false);
        }

        /// <summary>
        /// This Function job is to allow for other types of Frameworks to hook into
        /// the scripts. 
        /// To do this, you need a custom Wrapper similiar to TmcWrapper. 
        /// 
        /// TODO: Rewrite to take in custom frameworks.
        /// </summary>
        private void FrameWork()
        {
            //TmcWrapper.TMC = Main.Instance._ExportDictionary["core"].getCoreObject();
        }

        #region Tasks
        /// <summary>
        /// Creates all the Scaleforms which will be used for the spray text.
        /// </summary>
        /// <returns></returns>
        private async Task LoadScaleFormsAsync()
        {
            // 1 - 15 are what matches the PLAYER_NAME Scaleforms in the game.
            // TODO: Create own scaleform files for the spray text.
            try
            {
                int breakCounter = 0;
                for (int i = 0; i < 15; i++)
                {
                    Debug.WriteLine($"{i} -- Being loaded");
                    Scaleform scaleform = new Scaleform($"{SCALEFORM_NAME}{i + 1:00}");
                    while (!scaleform.IsLoaded)
                    {
                        if (breakCounter == 100) throw new Exception($"{i} -- Failed to load");
                        await BaseScript.Delay(5);
                        breakCounter++;
                    }

                    Debug.WriteLine($"{i} -- ScaleForm loaded -- " + $"{SCALEFORM_NAME}{i + 1:00}");
                    _scaleforms.Add(i, scaleform);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            finally
            {
                Debug.WriteLine($"^2All {_scaleforms.Count} Scaleforms have been loaded.");
                // All scaleforms are loaded
            }


        }

        /// <summary>
        /// Gets the sprays in range of the player, and orders them by distance.
        /// </summary>
        /// <returns></returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task SpraysInRangeAsync(bool ignorePlayerPos = false)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            Vector3 playerPos = Game.PlayerPed.Position;

            // TODO: Need to allow an update if a new SprayTag is added in range.
            if (Vector3.Distance(playerPos, _previousDistance) < 10f && _spraysInRange.Count > 0 && !ignorePlayerPos)
            {
                return;
            }

            _previousDistance = playerPos;

            _spraysInRange.Clear();

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
                //Debug.WriteLine($"{i}");
                SprayTag spray = _spraysInRange[i];
                spray.Scaleform = _scaleforms[i];
                spray.Draw();
            }
        }
        #endregion


        /// <summary>
        /// Called by EventHandle to trigger creating a new Spray
        /// </summary>
        /// <returns></returns>
        private void InitSpray()
        {
            try
            {
                //Debug.WriteLine("Command Called");
                CreateSpray("Spray Location");
                //TmcWrapper.SimpleNotify("Left Click to Set Locations, Right Click to Cancel", 10000);
            }
            catch
            {

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="arguments"></param>
        /// <param name="raw"></param>
        public void OnSprayCommand(int source, List<object> arguments, string raw)
        {
            GetPlayerPed(source);
            try
            {
                string sprayText = $"Spray Template {_sprays.Count}";
                if (arguments.Count > 0)
                {
                    sprayText = arguments[0].ToString();
                }

                CreateSpray(sprayText);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"^1Error: {ex.Message}");
                Debug.WriteLine($"^Stack:\n{ex}");
            }
        }

        public void CreateSpray(string sprayTag, bool isTurfSpray = false, SprayTag turfSpray = null)
        {
            //Debug.WriteLine("Inside Spray");
            // Create the temporary spray
            if (_tempSpray == null || _tempSpray?.Scaleform is null)
            {
                _tempSpray = new SprayTag();
            }

            //Debug.WriteLine("Temp Spray initialized");

            // Set the spray initial information (font, color, text)
            _tempSpray.Text = sprayTag;
            _tempSpray.Font = FontHandler.Instance.GetRandomFont();
            _tempSpray.Color = $"#{Main.Random.Next(0x1000000):X6}"; // Random color


            //Debug.WriteLine("Temp Spray has basic setup");
            // Set the spray scaleform to the private spray scaleform
            _tempSpray.Scaleform = _scaleforms[PRIVATE_SPRAY_SCALEFORM_KEY];

            // Check if the camera exists, if it does then destroy it
            if (DoesCamExist(_camera))
                DestroyCam(_camera, false);

            // Create the camera
            _camera = CreateCam("DEFAULT_SCRIPTED_CAMERA", false);
            //Debug.WriteLine("Camerea Created");

            if (isTurfSpray)
            {
                //Debug.WriteLine("Inside If");
                _tempSpray.Location = turfSpray.Location;
                _tempSpray.Rotation = turfSpray.Rotation;
                // draw the spray
                //Debug.WriteLine("About to draw");
                //_tempSpray.Draw();

                //string changeValue = JsonConvert.SerializeObject(_tempSpray);
                //BaseScript.TriggerServerEvent("pspray:add_spray", changeValue);

                // Reset the temp spray
                //_tempSpray = null;
                SaveSpray();
                //Main.Instance.AttachTick(TurfTempSprayAsync);
            }
            else
            {
                // Start the tick event
                Main.Instance.AttachTick(SetSprayPositionAsync);
            }


        }

        private async Task TurfTempSprayAsync()
        {
            // draw the spray
            //_tempSpray.Draw();
            _spraysInRange.Add(_tempSpray);
            if (Game.IsControlJustPressed(0, Control.Aim))
            {
                
                _spraysInRange.Clear();
                _tempSpray = null;
            }
            Main.Instance.DetachTick(TurfTempSprayAsync);
        }


#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task SetSprayPositionAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            Vector3 coords = new();
            Vector3 rotation = new();

            Debug.WriteLine("Inside Spray Position Start: " + MenuIsActive);

            RayCastGamePlayCamera(ref coords, ref rotation);


#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            // No need to await this, but it needs to be async to update the _sprayFinalRotation
            RunCameraMethodAsync(coords, rotation);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            coords += (rotation * FORWARD_OFFSET);

            // update the spray position and rotation
            _tempSpray.Location = coords;
            _tempSpray.Rotation = _sprayFinalRotation;

            // draw the spray
            _tempSpray.Draw();

            // Check if the player has pressed the accept button (Enter)
            DisableControlAction(0, (int)Control.Attack, true);
            if (Game.IsDisabledControlJustReleased(0, Control.Attack) && !MenuIsActive)
            {
                //TmcWrapper.CreateSideMenu(_tempSpray.Text, FontHandler.Instance.returnList());
                //Create new NUI
                MenuIsActive = true;
                if (true) BaseScript.TriggerEvent("pspray:Default_Menu_Trigger");

                //Users that want their own Custom Interface, can create an EventHandler in their own script. 
            }

            if (Game.IsControlJustPressed(0, Control.Aim))
            {
                EndSprayCam();
            }
        }


        private void ListSpray(string listSpray){ 
            /*Debug.WriteLine(listSpray);*/ 
            _sprays = JsonConvert.DeserializeObject<List<SprayTag>>(listSpray); 
            SpraysInRangeAsync(true); 
        }
        private void SprayText(string newText) => _tempSpray.Text = newText;
        private void SprayFont(int newFont) => _tempSpray.Font = FontHandler.Instance.GetFont(newFont);
        private void SprayScale(float newScale) => _tempSpray.ScaleSet(newScale);
        private void SprayColor(string newColor) => _tempSpray.Color = newColor;


        private void SaveSpray()
        {
            string changeValue = JsonConvert.SerializeObject(_tempSpray);
            BaseScript.TriggerServerEvent("pspray:add_spray", changeValue);
            // Reset the temp spray
            _tempSpray = null;

            // Check if the camera exists, if it does then destroy it
            if (DoesCamExist(_camera))
                DestroyCam(_camera, false);

            // Remove the tick event
            MenuIsActive = false;
            Main.Instance.DetachTick(SetSprayPositionAsync);
        }

        private void RemoveSpray()
        {
            //Remove it from List first
            Vector3 playerPos = Game.PlayerPed.Position;
            SpraysInRangeAsync(true);
            if (Vector3.Distance(playerPos, _spraysInRange[0].Location) < 3f)
            {
                string jsonSprayToRemove = JsonConvert.SerializeObject(_spraysInRange[0]);
                BaseScript.TriggerServerEvent("pspray:remove_sprays", jsonSprayToRemove);
            }
            else
            {
                Debug.WriteLine("Not In Range to Delete");
            }
        }

        private void EndSprayCam()
        {
            MenuIsActive = false;
            Main.Instance.DetachTick(SetSprayPositionAsync);
        }

        #region RayCast Functions
        private void RayCastGamePlayCamera(ref Vector3 endPoint, ref Vector3 rotation)
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

        public Vector3 RotationToDirection(Vector3 rotation)
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

        private async Task RunCameraMethodAsync(Vector3 WantedLocation, Vector3 WantedRotation)
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
                    await BaseScript.Delay(2);
                    var cameraRotation = GetCamRot(_camera, 2);
                    SetCamActive(_camera, false);

                    _sprayFinalRotation = cameraRotation;
                }
            }
        }
        #endregion

    }
}

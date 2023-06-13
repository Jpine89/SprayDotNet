using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Util;

namespace Client.Functions
{
    class Spray_Function : BaseScript
    {
        public static Entity HitEntity { get; set; }
        public Vector3 FinalRotation { get; set; }
        private List<Spray> SPRAYS = new List<Spray>();
        public string sprayText = "";
        public bool isSpray = false;
        private const float FORWARD_OFFSET = 0.015f;
        private Spray newSpray = new Spray();
        private const int SCAFLEFORM_MIN = 1;
        private const int SCAFLEFORM_MAX = 12;
        private Dictionary<int, int> ScaleFormList = new Dictionary<int, int>();
        int rotCam;

        public Spray_Function()
        {
            RegisterKeyMapping("+SaveSpray", "Used to Save Spray", "keyboard", "RETURN");
            RegisterCommand("+SaveSpray", new Action(SaveSpray), false);
        }

        public void DrawSpray(int scaleFormHandle, Spray spray)
        {
            string SprayUserData = $"<FONT color='{spray.Color}' FACE='Beat Street'> {spray.Text} ";

            //PushScaleformMovieFunction(scaleFormHandle, "SHOW_SHARD_WASTED_MP_MESSAGE");
            PushScaleformMovieFunction(scaleFormHandle, "SET_PLAYER_NAME");
            PushScaleformMovieFunctionParameterString(SprayUserData);
            //PushScaleformMovieFunctionParameterString("Small Text");
            PushScaleformMovieFunctionParameterInt(5);
            PopScaleformMovieFunctionVoid();

            //await Delay(0);

            DrawScaleformMovie_3dSolid
                (
                    scaleFormHandle,
                    spray.LocationCoords.X, spray.LocationCoords.Y, spray.LocationCoords.Z,
                    spray.RotationCoords.X, spray.RotationCoords.Y, spray.RotationCoords.Z,
                    (float)1.0, (float)1.0, (float)1.0, //unk values
                    (float)2.0, (float)2.0, (float)1.0, //Scale X/Y/Z
                    2 //always 2?
                );
        }

        /*
         * ToDo
         * Add not only Functionality, but new class to handle this
         */
        public void RemoveSpray()
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

        /*
         * Logic will want to be built out more on this when
         * Database Logic is being built. Plus NUI
         */


        [Command("pspray:SaveSpray")]
        public void SaveSpray()
        {
            if (isSpray)
            {
                isSpray = false;
                SPRAYS.Add(newSpray);
                newSpray = new Spray();
            }

        }


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
            if (DoesCamExist(rotCam))
                DestroyCam(rotCam, false);

            rotCam = CreateCam("DEFAULT_SCRIPTED_CAMERA", false);
            Vector3 currentSprayRotation = new Vector3();
            int reCheck = 30;
            while (true)
            {
                await Delay(0);

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
                        SetCamCoord(rotCam, WantedLocation.X, WantedLocation.Y, WantedLocation.Z);
                        PointCamAtCoord(rotCam, camLookPosition.X, camLookPosition.Y, camLookPosition.Z);
                        SetCamActive(rotCam, true);
                        await Delay(2);
                        var rot = GetCamRot(rotCam, 2);
                        SetCamActive(rotCam, false);


                        currentSprayRotation = WantedRotation;
                        FinalRotation = rot;
                        break;
                    }
                }
            }
        }

    }
}

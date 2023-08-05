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
        }

        private void PTurfTest()
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
    }
}

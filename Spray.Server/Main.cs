using CitizenFX.Core;
using Spray.Server.Utils;
using static CitizenFX.Core.Native.API;

namespace Spray.Server
{
    public class Main: BaseScript
    {
        public Main()
        {
            Debug.WriteLine("Starting Server Init");
            Init();
        }

        private void Init()
        {
            using (var connection = Database.GetConnection())
            {
                //connection.Open();
                Debug.WriteLine("Connection Open: " + connection.Ping);
            }
        }
    }
}
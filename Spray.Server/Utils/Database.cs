using CitizenFX.Core;
using CitizenFX.Core.Native;
using MySql.Data.MySqlClient;

namespace Spray.Server.Utils
{
    class Database
    {
        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(GetConnectionString());
        }

        private static string GetConnectionString()
        {
            string stringReturn = API.GetConvar("mysql_connection_string", null);
            if (stringReturn != null) Debug.WriteLine("Convars in Config are not set!");
            return stringReturn;
        }
    }
}

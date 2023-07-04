using CitizenFX.Core;
using CitizenFX.Core.Native;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Text;

namespace PSpray.Server.Utils
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
            if (stringReturn == null) Debug.WriteLine("Convars in Config are not set!");

            string actualString = $"Server=localhost; Port=3306; User ID=root; Password=pineapple; Database=QBCoreFramework_A237FE;";
            return actualString;
        }
    }
}

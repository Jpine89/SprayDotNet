using CitizenFX.Core;
using System;

namespace PSpray.Server
{
    class Main : BaseScript
    {
        public Main()
        {
            EventHandlers["init_player"] += new Action<Player>(InitPlayer);

            Debug.WriteLine("Server Init");

            initConnect();
        }

        private void initConnect()
        {
            using (var connection = Database.GetConnection())
            {
                Debug.WriteLine("Connection?");
            }
        }

        private async void InitPlayer([FromSource] Player source)
        {
            Debug.WriteLine("Calling Init Player!");
            //var parameters = new { Identifier = source.Identifiers["license"], Money = 5000, XP = 0 };
            //using (var connection = Database.GetConnection())
            //{
            //    int rowsAffected = await connection.ExecuteAsync(Queries.initPlayer, parameters);
            //    Debug.WriteLine($"Player Init rowsAffected: {rowsAffected}");
            //}
        }
    }
}

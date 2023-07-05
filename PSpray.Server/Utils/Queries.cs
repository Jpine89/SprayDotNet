using System;
using System.Collections.Generic;
using System.Text;

namespace PSpray.Server.Utils
{
    class Queries
    {
        public static string createPSprayTable = @"
            CREATE TABLE IF NOT EXISTS `pspray` (
                `id` int NOT NULL AUTO_INCREMENT,
                `identifier` varchar(128) NOT NULL,
                `locx` float,
                `locy` float,
                `locz` float,
                `rotx` float,
                `roty` float,
                `rotz` float,
                `scale` int,
                `color` varchar(128),
                `text` varchar(256),
                `font` varchar(128),
                PRIMARY KEY (`id`)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;";



        public static string getPSprayFromTable = @"
            SELECT * FROM `pspray` WHERE deleted = 0;
        ";

        public static string insertPSprayToTable = @"
            INSERT IGNORE INTO `pspray` (identifier, locx, locy, locz, rotx, roty, rotz, scale, color, text, font)
            VALUES (@Identifier, @Locx, @Locy, @Locz, @Rotx, @Roty, @Rotz, @Scale, @Color, @Text, @Font)
        ";

        public static string removePSprayFromTable = @"UPDATE `pspray`SET deleted = 1 WHERE id = @Id";

    }
}

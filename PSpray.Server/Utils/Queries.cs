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
                PRIMARY KEY (`id`),
                UNIQUE KEY `identifier` (`identifier`)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;";

    }
}

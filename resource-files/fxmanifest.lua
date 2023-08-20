game "gta5"
fx_version "cerulean"

author "@Jpine89 <https://github.com/Jpine89>"
description "Spray Graffitis on walls and other surfaces."
version "1.0.0"

files {
    "client/Newtonsoft.Json.dll",
    "client/ScaleformUI.dll",
	"gang_areas.gfx",
	"GangTurf.json"
}
client_scripts {
    "client/*.net.dll",
}

server_scripts {
    "server/*.net.dll",
}

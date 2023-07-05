using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace PSpray.Server.Entities
{
    class SprayTag
    {
        public int Id { get; set; }
        public Vector3 Location { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public string Text { get; set; }
        public string Font { get; set; }
        public string Color { get; set; }


    }
}

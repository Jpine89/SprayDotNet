﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CitizenFX.Core;

namespace Client.Util
{
    class Spray {
        private string _text;
        private string _color;
        private int _id;
        public Vector3 LocationCoords { get; set; }
        public Vector3 RotationCoords { get; set; }
        public int SprayId { 
            get => _id; 
            set { _id = value; HasChanged = true; } }
        public string Text { get => _text; set
            {
                _text = value;
            }
        }
        public string Font { get; set; }
        public string Scale { get; set; }
        public string Color { get; set; }

        public bool HasChanged { get; set; }
    }
}

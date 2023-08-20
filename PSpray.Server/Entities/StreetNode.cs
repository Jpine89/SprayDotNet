using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSpray.Server.Entities
{
    class StreetNode
    {
        public string Name { get; set; }
        public List<StreetNode> Subnodes { get; } = new List<StreetNode>();
        public int CoordX { get; set; }
        public int CoordY { get; set; }
        //public Coordinate Coordinates { get; set; }
        public string Color { get; set; }


        public StreetNode(string name)
        {
            Name = name;
        }

        public void AddSubnode(StreetNode subnode)
        {
            Subnodes.Add(subnode);
        }
    }

    class Coordinate
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}

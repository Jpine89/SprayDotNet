using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spray.Client.Entities
{
    internal class TmcWrapper
    {
        public void CreateMenu(dynamic TMC)
        {
            //TMC.Functions.OpenMenu(setting, elements, new Action<dynamic, bool>(close), new Action(something), new Action<dynamic>(testFunc));
        }
    }

    public class Elements
    {
        public string type { get; set; }
        public string name { get; set; }
        public string label { get; set; }
        public string value { get; set; }
        public bool multiline { get; set; }
        public bool form { get; set; }
        public string cat { get; set; }
        public string icon { get; set; }
        public bool disabled { get; set; }

    }

    public class TmcChangeFunc
    {
        public string NewValue { get; set; }
        public string ElementChanged { get; set; }
        public string OldValue { get; set; }
        public string MenuNamespace { get; set; }
        public string Text { get; set; }
        public bool SaveSpray { get; set; }
    }
}

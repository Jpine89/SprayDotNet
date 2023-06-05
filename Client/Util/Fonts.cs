using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Util
{
    class Fonts
    {
        public string FontName { get; set; }
        public int FontId { get; set; }
        public string FontCategory { get; set; }

        public Fonts(string fontName, int fontId, string fontCategory)
        {
            this.FontName = fontName;
            this.FontId = fontId;
            this.FontCategory = fontCategory;
        }
    }
}

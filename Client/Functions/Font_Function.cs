using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Util;
using System.Collections;

namespace Client.Functions
{
    class Font_Function : BaseScript
    {
        Dictionary<string, int> keyValuePairs = new Dictionary<string, int>();
        public List<Fonts> FontsList = new List<Fonts>() {
            new Fonts("Chalet Comprimé", 4, "Normal"),
            new Fonts("Chalet", 0, "Normal"),
            new Fonts("Sign Painter", 1, "Handwritten"),
            new Fonts("Pricedown", 7, "Misc")
        };

        public int FontID;

        public Font_Function()
        {
            //var test = System.IO.Path.GetFullPath("Font_Function.cs");
            //Debug.WriteLine("The Filepath? " + test);

            //string test = System.IO.Path.GetDirectoryName("~/BeatStreet.gfx");
            //Debug.WriteLine(test);
            SetupFonts();

            RegisterCommand("Fonts", new Action(TestFonts), false);

            //foreach (Fonts font in FontsList)
            //{
            //    Debug.WriteLine(font.FontName);
            //}
            Debug.WriteLine(FontsList.Count.ToString());
            //Debug.WriteLine(System.IO.Path.GetDirectoryName("C:/Users/FubarP/source/repos/SprayDotNet/Client/Stream/Fonts/Graffiti/Dity"));
        }

        private async void TestFonts()
        {
            RegisterFontFile("BeatStreet");
            FontID = RegisterFontId("Beat Street");
            Debug.WriteLine(FontID.ToString());
            while (true){
                await Delay(0);
                SetTextFont(FontID);
                BeginTextCommandDisplayText("STRING");
                AddTextComponentString("Hello, world!");
                EndTextCommandDisplayText(0.5f, 0.5f);
            };
        }

        private void SetupFonts()
        {
            foreach (var font in Addons)
            {
                //var test = "C:\\Users\\Fubarp\\source\\repos\\SprayDotNet\\Client\\Stream\\Fonts" + font.FontCategory + "\\" + font.FontName;
                //Debug.WriteLine(test);
                RegisterFontFile(font.FontCategory + "/" + font.FontName);
                var fontId = RegisterFontId(font.FontId);
                //Debug.WriteLine(fontId.ToString());
                FontsList.Add(new Fonts(font.FontId, fontId, font.FontCategory));
            }
        }

        List<AddonFonts> Addons = new List<AddonFonts>() {
            //Normal
            new AddonFonts("ArialNarrow", "Arial Narrow", "Normal"),
            new AddonFonts("Lato", "Lato", "Normal"),
            // Handwritten
            new AddonFonts("Inkfree", "Inkfree", "Handwritten"),
            new AddonFonts("Kid", "Kid", "Handwritten"),
            new AddonFonts("Strawberry", "Strawberry", "Handwritten"),
            new AddonFonts("PaperDaisy", "Paper Daisy", "Handwritten"),
            new AddonFonts("ALittleSunshine", "A Little Sunshine", "Handwritten"),
            new AddonFonts("WriteMeASong", "Write Me A Song", "Handwritten"),
            // Graffiti
            new AddonFonts("BeatStreet", "Beat Street", "Graffiti"),
            new AddonFonts("DirtyLizard", "Dirty Lizard", "Graffiti"),
            new AddonFonts("Maren", "Maren", "Graffiti"),
            // Misc
            new AddonFonts("HappyDay", "Happy Day", "Misc"),
            new AddonFonts("ImpactLabel", "Impact Label", "Misc"),
            new AddonFonts("Easter", "Easter", "Misc")

        };
    }

    public class AddonFonts
    {
        public string FontName { get; set; }
        public string FontId { get; set; }
        public string FontCategory { get; set; }

        public AddonFonts(string fontName, string fontId, string fontCategory)
        {
            FontName = fontName;
            FontId = fontId;
            FontCategory = fontCategory;
        }
    }
}

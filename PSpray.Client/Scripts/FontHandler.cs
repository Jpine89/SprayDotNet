using PSpray.Client.Entities;
using System.Collections.Generic;

namespace PSpray.Client.Scripts
{
    internal class FontHandler
    {
        private static readonly object _padlock = new();
        private static FontHandler _instance;

        private readonly List<AddonFont> _fonts = new()
        {
            //Normal
            new AddonFont("ArialNarrow", "Arial Narrow", "Normal"),
            new AddonFont("Lato", "Lato", "Normal"),
            // Handwritten
            new AddonFont("Inkfree", "Inkfree", "Handwritten"),
            new AddonFont("Kid", "Kid", "Handwritten"),
            new AddonFont("Strawberry", "Strawberry", "Handwritten"),
            new AddonFont("PaperDaisy", "Paper Daisy", "Handwritten"),
            new AddonFont("ALittleSunshine", "A Little Sunshine", "Handwritten"),
            new AddonFont("WriteMeASong", "Write Me A Song", "Handwritten"),
            // Graffiti
            new AddonFont("BeatStreet", "Beat Street", "Graffiti"),
            new AddonFont("DirtyLizard", "Dirty Lizard", "Graffiti"),
            new AddonFont("Maren", "Maren", "Graffiti"),
            // Misc
            new AddonFont("HappyDay", "Happy Day", "Misc"),
            new AddonFont("ImpactLabel", "Impact Label", "Misc"),
            new AddonFont("Easter", "Easter", "Misc")
        };

        private FontHandler()
        {
            Init();
        }

        internal static FontHandler Instance
        {
            get
            {
                lock (_padlock)
                {
                    return _instance ??= new FontHandler();
                }
            }
        }

        internal string GetFont(int i)
        {
            return _fonts[i].Name;
        }

        internal string GetRandomFont()
        {
            return _fonts[Main.Random.Next(_fonts.Count)].Name;
        }

        private async void Init()
        {
            for (int i = 0; i < _fonts.Count; i++)
            {
                await BaseScript.Delay(0);
                AddonFont font = _fonts[i];
                RegisterFontFile(font.FileName);
                font.Id = RegisterFontId(font.Name);
                //Debug.WriteLine($"Registered font: {font.Name} ({font.FileName}) with ID: {font.Id}");
            }
        }

        internal List<AddonFont> returnList() => _fonts;
    }
}

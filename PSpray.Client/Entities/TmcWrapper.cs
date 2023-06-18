using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSpray.Client.Entities
{
    internal class TmcWrapper
    {
        private dynamic _dynamic;
        private List<AddonFont> _font;
        public dynamic TMC { 
            get => _dynamic; 
            set { _dynamic = value; } }
        public void SimpleNotify(string message, int timer) => TMC.Functions.SimpleNotify(message, "info", timer);
        public void CreateMenu(string sprayText, List<AddonFont> fontList)
        {
            _font = fontList;
            Debug.WriteLine(_font.Count().ToString());

            Dictionary<string, object> setting = new() {
                {"namespace", "testing_namespace" },
                {"test", "openMenu" },
                {"title", "Edit text" },
                {"subtitle", "If you have a speciifc" },
                {"form", true }
            };


            TmcElements TextBox = new TmcElements()
            {
                type = "text",
                name = "text",
                label = "Text",
                value = sprayText,
                multiline = true
            };

            TmcElements Fonts = new TmcElements()
            {
                name = "font",
                type = "slider",
                label = "Current Font: " + _font[0].Name,
                disabled = false,
                step = 1,
                min = 0,
                max = _font.Count() -1
            };

            TmcElements Scale = new TmcElements()
            {
                type = "slider",
                name = "scale",
                label = "Font Size",
                disabled = false,
                step = .2,
                min = -1,
                max = 6

            };
            TmcElements[] elements = { TextBox, Fonts, Scale };
            TMC.Functions.OpenMenu(setting, elements, new Action<dynamic, bool>(CloseWindow), new Action(UnknownFunc), new Action<dynamic>(UpdateValuesFunc));
        }

        private void CloseWindow(dynamic finalchange, bool confirm)
        {
            if (confirm)
            {
                //string jsonChange = JsonConvert.SerializeObject(finalchange);
                //var FinalChanges = JsonConvert.DeserializeObject<TmcChangeFunc>(jsonChange);
                //BaseScript.TriggerEvent("pspray:Text_Spray", FinalChanges.NewValue);
                BaseScript.TriggerEvent("pspray:Save_Spray");
            }
            else { BaseScript.TriggerEvent("pspray:End_Spray"); }
        }
        private void UnknownFunc()
        {
            Debug.WriteLine("Unknown Func Was Called");
        }
        private void UpdateValuesFunc(dynamic change)
        {
            string changeValue = JsonConvert.SerializeObject(change);
            Debug.WriteLine(changeValue);
            var changed = JsonConvert.DeserializeObject<TmcChangeFunc>(changeValue);
            

            switch (changed.ElementChanged)
            {
                case "text":
                    BaseScript.TriggerEvent("pspray:Text_Spray", changed.NewValue);
                    break;
                case "scale":
                    BaseScript.TriggerEvent("pspray:Scale_Spray", changed.NewValue);
                    break;
                case "font":
                    TmcElements Fonts = new TmcElements()
                    {
                        name = "font",
                        label = "Current Font: " + _font[Int32.Parse(changed.NewValue)].Name,
                        type = "slider",
                        disabled = false,
                        step = 1,
                        min = 0,
                        max = _font.Count() - 1
                    };
                    TMC.Functions.UpdateSideMenuElement(Fonts);
                    BaseScript.TriggerEvent("pspray:Font_Spray", changed.NewValue);
                    break;
                case "color":
                    BaseScript.TriggerEvent("pspray:Color_Spray", changed.NewValue);
                    break;
            }

            //Switch Case on ElementChange to determine what values to change
            
        }

    }

    public class TmcElements
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
        public int min { get; set; }
        public int max { get; set; }
        public double step { get; set; }

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

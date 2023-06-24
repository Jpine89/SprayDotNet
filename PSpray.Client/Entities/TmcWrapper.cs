using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSpray.Client.Entities
{
    /// <summary>
    /// Warpper class for the TMC Framework,
    /// Can be used as a reference for other Frameworks on what Events to call.
    /// </summary>
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
        private void SetupSideMenuEvents()
        {
            TMC.Functions.RegisterUiHandler("psprayCloseSideMenu", new Action<dynamic>(SideMenuCloseWindow));
            TMC.Functions.RegisterUiHandler("psprayChangeSideMenu", new Action<dynamic>(SideMenuUpdateValuesFunc));
            //TMC.Functions.RegisterUiHandler("psprayCloseSideMenu", new Action<dynamic, bool>(CloseWindow));
        }

        public void CreateSideMenu(string sprayText, List<AddonFont> fontList)
        {
            SetupSideMenuEvents();
            _font = fontList;

            TmcCategories category = new TmcCategories() {
                name = "pspray",
                label = "PSpray Settings",
                icon = "fa-solid fa-spray-can"
            };

            TmcElements TextBox = new TmcElements()
            {
                type = "text",
                name = "text",
                label = "Text",
                value = sprayText,
                multiline = true,
                cat = "pspray"
            };

            TmcElements Fonts = new TmcElements()
            {
                name = "font",
                type = "slider",
                label = "Current Font: " + _font[0].Name,
                disabled = false,
                step = 1,
                min = 0,
                max = _font.Count() - 1,
                cat = "pspray"
            };

            TmcElements Scale = new TmcElements()
            {
                type = "slider",
                name = "scale",
                label = "Font Size",
                disabled = false,
                step = .2,
                min = -1,
                max = 6,
                cat = "pspray"

            };

            TmcElements Color = new TmcElements()
            {
                type = "colour-picker",
                name = "color",
                label = "Color Picker Field",
                cat = "pspray",
                value = "#0000ff"
            };

            //controls can be null?
            TmcCategories[] categories = { category };
            TmcElements[] elements = { TextBox, Fonts, Scale, Color };
            TMC.Functions.StartSideMenuUi("pspray", categories, elements);
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
                    TMC.Functions.UpdateMenuElement(Fonts);
                    BaseScript.TriggerEvent("pspray:Font_Spray", changed.NewValue);
                    break;
                case "color":
                    BaseScript.TriggerEvent("pspray:Color_Spray", changed.NewValue);
                    break;
            }

            //Switch Case on ElementChange to determine what values to change
            
        }

        private void SideMenuCloseWindow(dynamic data)
        {
            string json = JsonConvert.SerializeObject(data);
            var confirmClass = JsonConvert.DeserializeObject<TmcSideMenuClose>(json);
            if (confirmClass.confirm) { BaseScript.TriggerEvent("pspray:Save_Spray"); } else { BaseScript.TriggerEvent("pspray:End_Spray"); }
        }

        private void SideMenuUpdateValuesFunc(dynamic data)
        {
            string json = JsonConvert.SerializeObject(data);
            var values = JsonConvert.DeserializeObject<TmcSideChangeFunc>(json);
            switch (values.name)
            {
                case "text":
                    BaseScript.TriggerEvent("pspray:Text_Spray", values.val);
                    break;
                case "scale":
                    BaseScript.TriggerEvent("pspray:Scale_Spray", values.val);
                    break;
                case "font":
                    TmcElements Fonts = new TmcElements()
                    {
                        name = "font",
                        label = "Current Font: " + _font[Convert.ToInt32(values.val)].Name,
                        type = "slider",
                        disabled = false,
                        step = 1,
                        min = 0,
                        max = _font.Count() - 1
                    };
                    TMC.Functions.UpdateSideMenuElement(Fonts);
                    BaseScript.TriggerEvent("pspray:Font_Spray", values.val);
                    break;
                case "color":
                    string grabHex = JsonConvert.SerializeObject(values.val);
                    var hexClass = JsonConvert.DeserializeObject<TmcHex>(grabHex);
                    Debug.WriteLine(hexClass.hex);
                    BaseScript.TriggerEvent("pspray:Color_Spray", "#" + hexClass.hex);
                    break;
            }
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

    public class TmcCategories
    {
        public string name { get; set; }
        public string label { get; set; }
        public string icon { get; set; }
        public bool searchable { get; set; }
    }

    public class TmcControls
    {

    }

    public class TmcSideChangeFunc
    {
        public string name { get; set; }
        public object val { get; set; }
    }
    public class TmcHex
    {
        public string hex { get; set; }
    }
    public class TmcSideMenuClose
    {
        public bool confirm { get; set; }
    }
}

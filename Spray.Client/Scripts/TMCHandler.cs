using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spray.Client.Scripts
{
    internal class TMCHandler
    {
        private static readonly object _padlock = new();
        private static TMCHandler _instance;
        private dynamic _tmc;
        private TMCHandler()
        {
            Init();
        }
        internal static TMCHandler Instance
        {
            get
            {
                lock (_padlock)
                {
                    return _instance ??= new TMCHandler();
                }
            }
        }

        private async void Init()
        {
            //_tmc = Exports["core"].getCoreObject();

            _tmc = Main.Instance._ExportDictionary["core"].getCoreObject();
            RegisterCommand("testEvent", new Action<int, List<object>, string>(TestEvent), false);
            Debug.WriteLine($"Registered testEvent");
        }

        private void TestEvent(int source, List<object> arguments, string raw)
        {
            Debug.WriteLine("Test Event Called");
            Elements elm = new Elements()
            {
                type = "text",
                name = "text",
                label = "Text",
                value = "tester",
                multiline = true
            };
            Elements confirmbtn = new Elements()
            {
                type = "button",
                name = "button_test",
                label = "Button Test",
                icon = "fad fa-heartbeat",
                disabled = false
            };
            Elements[] elements = { elm, confirmbtn };
            Dictionary<string, object> setting = new() {
                {"namespace", "testing_namespace" },
                {"test", "openMenu" },
                {"title", "Edit text" },
                {"subtitle", "If you have a speciifc" },
                {"form", true }
            };
            _tmc.Functions.OpenMenu(setting, elements, new Action<dynamic, bool>(close), new Action(something), new Action<dynamic>(testFunc));
            //BaseScript.TriggerEvent("pspray:test");
        }
        private void close(dynamic change, bool confirm)
        {
            TmcChangeFunc sendToSave = new TmcChangeFunc();
            sendToSave.SaveSpray = false;
            if (confirm)
            {
                string test = JsonConvert.SerializeObject(change);
                sendToSave = JsonConvert.DeserializeObject<TmcChangeFunc>(test);
                Debug.WriteLine("test close");
                Debug.WriteLine(test);
                Debug.WriteLine($"{confirm}");
                sendToSave.SaveSpray = true;
            }
            //Spray_Function.SaveSpray(sendToSave);
            BaseScript.TriggerEvent("pspray:SaveSpray", sendToSave.Text, sendToSave.SaveSpray);
        }
        private void something()
        {
            Debug.WriteLine("test Something");
        }
        private void testFunc(dynamic change)
        {
            Debug.WriteLine("test func");
            string test = JsonConvert.SerializeObject(change);


            Debug.WriteLine(test);
            var changed = JsonConvert.DeserializeObject<TmcChangeFunc>(test);
            Debug.WriteLine(changed.NewValue);
            BaseScript.TriggerEvent("pspray:spray_text_update", changed.NewValue);
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

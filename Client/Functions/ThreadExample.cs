using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Client.Functions
{
    internal class ThreadExample : BaseScript
    {
        bool isOn = false;
        private dynamic TMC;

        public ThreadExample()
        {
            TMC = Exports["core"].getCoreObject();

            //RegisterNuiCallback("close", new Action<IDictionary<string, object>, CallbackDelegate>(async (body, result) =>
            //{
            //    Debug.WriteLine("Testing Close Callback");
            //    result(new { ok = true });
            //}));

            //RegisterScript(this);
            //UnregisterScript(this);
            //Interval 1000;
            //Tick += ThreadExample_Tick;

            //RegisterCommand("thread", new Action(TestThreads), false);
        }

        [Command("DoIt")]
        [EventHandler("pspray:open_menu")]
        private void TMCMenu()
        {
            Elements elm = new Elements()
            {
                type = "text",
                name = "text",
                label = "Text",
                value = "tester",
                multiline = true
            };
            Elements confirmbtn = new Elements() {
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
            TMC.Functions.OpenMenu(setting, elements, new Action<dynamic, bool>(close), new Action(something), new Action<dynamic>(testFunc));
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
            TriggerEvent("pspray:SaveSpray", sendToSave.Text, sendToSave.SaveSpray);
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
            TriggerEvent("pspray:spray_text_update", changed.NewValue);
        }

        private async Task ThreadExample_Tick()
        {
            await Delay(1000);
            Debug.WriteLine("This will pop every second");
        }

        private void TestThreads()
        {
            if (!isOn)
            {
                isOn = true;
                Tick += ThreadExample_Tick;
            }
            else
            {
                isOn = false;
                Tick -= ThreadExample_Tick;
            }
        }
    }

    public class Elements {
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

    public class TmcChangeFunc{
        public string NewValue { get; set; }
        public string ElementChanged { get; set; }
        public string OldValue { get; set; }
        public string MenuNamespace { get; set; }
        public string Text { get; set; }
        public bool SaveSpray { get; set; }
    }
}

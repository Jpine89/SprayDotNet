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
            //TMC = Exports["core"].getCoreObject();

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

        [Command("oldspawn")]
        private void testspawn()
        {
            Debug.WriteLine("Inside Old Spawn");
            Exports["spawnmanager"].spawnPlayer(new
            {
                x = 0,
                y = 0,
                z = 0,
                model = "s_m_y_cop_01"
            });
        }

        [Command("DoIt")]
        private void test()
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
            string test = JsonConvert.SerializeObject(change);
            Debug.WriteLine("test close");
            Debug.WriteLine(test);
            Debug.WriteLine($"{confirm}");
        }
        private void something()
        {
            Debug.WriteLine("test Something");
        }
        private void testFunc(dynamic change)
        {
            string test = JsonConvert.SerializeObject(change);
            Debug.WriteLine("test func");
            Debug.WriteLine(test);
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

    public class Elements{
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
}

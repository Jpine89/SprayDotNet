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
            BaseScript.TriggerEvent("pspray:test");
        }
    }
}

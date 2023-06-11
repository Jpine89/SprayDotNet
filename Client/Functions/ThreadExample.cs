using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Functions
{
    internal class ThreadExample : BaseScript
    {
        bool isOn = false;
        private dynamic TMC;

        public ThreadExample()
        {
            TMC = Exports["core"].getCoreObject();
            //RegisterScript(this);
            //UnregisterScript(this);
            //Interval 1000;
            //Tick += ThreadExample_Tick;

            //RegisterCommand("thread", new Action(TestThreads), false);
        }

        [Command("DoIt")]
        private void test()
        {
            Debug.WriteLine("Do it");
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
}

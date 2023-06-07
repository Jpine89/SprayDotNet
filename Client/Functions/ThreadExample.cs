using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Functions
{
    class ThreadExample : BaseScript
    {
        public ThreadExample()
        {
            //Interval 1000;
            //Tick += ThreadExample_Tick;
        }

        private async Task ThreadExample_Tick()
        {
            await Delay(1000);
            Debug.WriteLine("This will pop every second");
        }
    }
}

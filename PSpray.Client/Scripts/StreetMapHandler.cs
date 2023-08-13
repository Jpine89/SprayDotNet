using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSpray.Client.Scripts
{
    internal class StreetMapHandler
    {
        private static readonly object _padlock = new();
        private static StreetMapHandler _instance;
        private StreetMapHandler()
        {
            Init();
            Debug.WriteLine("^2PSpray Tag Handler has been initialised.");
        }

        internal static StreetMapHandler Instance
        {
            get
            {
                lock (_padlock)
                {
                    return _instance ??= new StreetMapHandler();
                }
            }
        }

        private async void Init()
        {
        }
    }
}

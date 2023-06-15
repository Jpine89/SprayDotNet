

namespace Spray.Client
{
    public class Main : BaseScript
    {
        internal static Main Instance { get; private set; }
        public static int GameTime { get; private set; }
        public static Random Random { get; } = new Random(GetGameTimer());

        public ExportDictionary ExportDictionary => Exports;

        public Main()
        {
            Instance = this;
            InitialiseScripts();

            Debug.WriteLine("^2Spray.Client has been initialised.");
        }

        private void InitialiseScripts()
        {
            _ = Scripts.FontHandler.Instance;
            _ = Scripts.SprayTagHandler.Instance;
        }

        /// <summary>
        /// Attaches a Tick
        /// </summary>
        /// <param name="task"></param>
        internal void AttachTick(Func<Task> task)
        {
            Tick += task;
        }

        /// <summary>
        /// Detaches a Tick
        /// </summary>
        /// <param name="task"></param>
        internal void DetachTick(Func<Task> task)
        {
            Tick -= task;
        }

        /// <summary>
        /// Adds an event handler to the event handlers dictionary.
        /// </summary>
        /// <remarks>This event will not go through FxEvents</remarks>
        /// <param name="eventName"></param>
        /// <param name="delegate"></param>
        internal void AddEventHandler(string eventName, Delegate @delegate)
        {
            Debug.WriteLine($"^7Registered Event Handler '{eventName}'");
            EventHandlers[eventName] += @delegate;
        }

        /// <summary>
        /// Gets the current game timer. This is the time in seconds since the game started.
        /// Using this as the only method to call GetGameTimer will lower the amount of calls to the native.
        /// </summary>
        [Tick]
        private async Task OnUpdateGameTimerAsync()
        {
            GameTime = API.GetGameTimer();
            await BaseScript.Delay(500);
        }
    }
}
using System.Runtime.CompilerServices;
using Urho3DNet;

namespace Urho3DNetTemplate
{
    /// <summary>
    /// Test application.
    /// </summary>
    public class TestApplication : Application
    {
        private object _gate = new object();
        private List<Action> _tasks = new List<Action>();
        private List<Action> _executionList = new List<Action>();

        public TestApplication(Context context) : base(context)
        {
            Instance = this;
        }

        public static TestApplication Instance { get; private set; }

        public ConfiguredTaskAwaitable<bool> ToMainThreadAsync()
        {
            var tcs = new TaskCompletionSource<bool>();
            InvokeOnMain(() => tcs.TrySetResult(true));
            return tcs.Task.ConfigureAwait(false);
        }

        private void InvokeOnMain(Action func)
        {
            lock (_gate)
            {
                _tasks.Add(func);
            }
        }

        public override void Setup()
        {
            EngineParameters[Urho3D.EpHeadless] = true;
            base.Setup();
        }

        public override void Start()
        {
            SubscribeToEvent(E.Update, OnUpdate);
            SubscribeToEvent(E.LogMessage, OnLogMessage);
            base.Start();
        }

        private void OnLogMessage(VariantMap args)
        {
            var logLevel = (LogLevel)args[E.LogMessage.Level].Int;
            if (logLevel >= LogLevel.LogError)
            {
                var message = args[E.LogMessage.Message].String;
                throw new Exception(message);
            }
        }

        private void OnUpdate(VariantMap obj)
        {
            lock (_gate)
            {
                (_tasks, _executionList) = (_executionList, _tasks);
            }

            foreach (var action in _executionList)
            {
                action?.Invoke();
            }
            _executionList.Clear();
        }

        public override void Stop()
        {
            UnsubscribeFromEvent(E.Update);
            base.Stop();
        }
    }
}
using Urho3DNet;
using Xunit.Abstractions;
using Xunit.Sdk;

[assembly: Xunit.TestFramework("Urho3DNetTemplate.InitializeEngine", "Urho3DNetTemplate.Tests")]

namespace Urho3DNetTemplate
{
    public class InitializeEngine : XunitTestFramework, IDisposable
    {
        private Context _context;
        private TestApplication _app;
        private Task<int> _task;
        private ManualResetEvent _initLock = new ManualResetEvent(false);
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public InitializeEngine(IMessageSink messageSink)
            : base(messageSink)
        {
            _task = Task.Factory.StartNew(RunApp, TaskCreationOptions.LongRunning);
            _initLock.WaitOne();
        }

        private int RunApp()
        {
            _context = new Context();
            _app = new TestApplication(_context);
            _initLock.Set();
            return _app.Run();
        }

        public new void Dispose()
        {
            _app.ErrorExit();
            try
            {
                int res = _task.Result;
            }
            catch (Exception ex)
            {
            }
            _context?.Dispose();
            // Place tear down code here
            base.Dispose();
        }
    }
}
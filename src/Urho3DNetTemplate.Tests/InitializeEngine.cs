using System.Diagnostics;
using Urho3DNet;
using Xunit.Abstractions;
using Xunit.Sdk;

[assembly: Xunit.TestFramework("Urho3DNetTemplate.InitializeEngine", "Urho3DNetTemplate.Tests")]

namespace Urho3DNetTemplate
{
    /// <summary>
    /// Helper class to run the Urho3D Application in a background.
    /// </summary>
    public class InitializeEngine : XunitTestFramework, IDisposable
    {
        private Context _context;
        private TestApplication _app;
        private Task<int> _task;
        private ManualResetEvent _initLock = new ManualResetEvent(false);
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Create and run application instance.
        /// </summary>
        /// <param name="messageSink"></param>
        public InitializeEngine(IMessageSink messageSink)
            : base(messageSink)
        {
            _task = Task.Factory.StartNew(RunApp, TaskCreationOptions.LongRunning);
            _initLock.WaitOne();
        }

        /// <summary>
        /// Run application main loop in a task.
        /// </summary>
        private int RunApp()
        {
            _context = new Context();
            _app = new TestApplication(_context);
            _initLock.Set();
            return _app.Run();
        }

        /// <summary>
        /// Dispose application.
        /// </summary>

        public new void Dispose()
        {
            _app.ErrorExit();
            try
            {
                int res = _task.Result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            _context?.Dispose();
            base.Dispose();
        }
    }
}
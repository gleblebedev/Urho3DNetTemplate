using Urho3DNet;

namespace Urho3DNetTemplate;

public class TestApplication: Application
{
    private object _gate = new object();
    private Queue<TaskCompletionSource> _tasks = new Queue<TaskCompletionSource>();
    private Queue<TaskCompletionSource> _executionList = new Queue<TaskCompletionSource>();
    private readonly int _mainThread;

    public TestApplication(Context context) : base(context)
    {
        Instance = this;
        _mainThread = Thread.CurrentThread.ManagedThreadId;
    }

    public static TestApplication Instance { get; private set; }

    public Task OnMainThreadAsync()
    {
        if (_mainThread == Thread.CurrentThread.ManagedThreadId)
            return Task.CompletedTask;

        var taskCompletionSource = new TaskCompletionSource(TaskCreationOptions.None);
        lock (_gate)
        {
            _tasks.Enqueue(taskCompletionSource);
        }

        taskCompletionSource.Task.ConfigureAwait(false);

        return taskCompletionSource.Task;
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

        while (_executionList.Count > 0)
        {
            var task = _executionList.Dequeue();
            task.SetResult();
        }
    }

    public override void Stop()
    {
        UnsubscribeFromEvent(E.Update);
        base.Stop();
    }
}
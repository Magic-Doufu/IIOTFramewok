public class TaskExecutor
{
    private readonly TaskManager taskManager;
    private readonly CancellationTokenSource cancellationTokenSource = new();
    private readonly ManualResetEventSlim pauseEvent = new(true);

    public TaskExecutor(TaskManager manager)
    {
        taskManager = manager;
    }

    public void Start()
    {
        Task.Run(async () =>
        {
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                pauseEvent.Wait();
                var tasks = taskManager.Dequeue();
                if (tasks != null)
                {
                    await Task.WhenAll(tasks);
                }
                await Task.Delay(100);
            }
        }, cancellationTokenSource.Token);
    }

    public void Pause() => pauseEvent.Reset();
    public void Resume() => pauseEvent.Set();
    public void Stop() => cancellationTokenSource.Cancel();
}

public class TaskManager
{
    private readonly Queue<List<Task>> taskQueue = new();
    private readonly TaskExecutor executor;

    public TaskManager()
    {
        executor = new TaskExecutor(this);
    }

    public void Enqueue(Task task) => taskQueue.Enqueue(new List<Task> { task });
    public void Enqueue(IEnumerable<Task> tasks) => taskQueue.Enqueue(tasks.ToList());
    public List<Task>? Dequeue() => taskQueue.Count > 0 ? taskQueue.Dequeue() : null;

    public void StartExecution() => executor.Start();
    public void PauseExecution() => executor.Pause();
    public void ResumeExecution() => executor.Resume();
    public void StopExecution() => executor.Stop();
}
using System.Collections.Concurrent;

namespace IIOTFramework.Core
{
    public class TaskExecutor
    {
        private readonly TaskManager taskManager;
        private TaskCompletionSource<bool> pauseSignal = new();
        private CancellationTokenSource cts = new CancellationTokenSource();
        public Task? runningTask { get; private set; }

        public bool IsBusy { get; private set; } = false;
        private void init()
        {
            cts = new();
            IsBusy = false;
            pauseSignal = new TaskCompletionSource<bool>();
            pauseSignal.SetResult(true);
        }
        public TaskExecutor(TaskManager manager) => taskManager = manager;

        async Task ExecuteAsync(CancellationToken token)
        {
            try {
                while (!token.IsCancellationRequested) {
                    var tasks = taskManager.Dequeue();
                    await (tasks.Any() ?
                        Task.WhenAll(tasks.Select(task => task(token))) : Task.Delay(100, token));
                }
            } catch (TaskCanceledException) {
                Console.WriteLine("[Task] Has been canceled.");
            } finally {
                Console.WriteLine($"[Task] {taskManager.QSize} tasks remaining. Cleaning up...");
                taskManager.Clear();
                Console.WriteLine("[Task] Gracefully cleaned up.");
            }
        }

        public void Start()
        {
            init();
            runningTask = Task.Run(async () => { await ExecuteAsync(cts.Token); });
        }
        public void Pause()
        {
            pauseSignal = new TaskCompletionSource<bool>();
        }
        public void Resume() => pauseSignal.SetResult(true);
        public async Task Stop()
        {
            if (runningTask != null) {
                Console.WriteLine("[Task] Cancelling ...");
                cts.Cancel();
                await runningTask;
            }
        }
    }

    public class TaskManager
    {
        private readonly TaskExecutor executor;
        public bool IsBusy { get => executor.IsBusy; }

        public TaskManager() => executor = new TaskExecutor(this);

        private readonly ConcurrentQueue<List<Func<CancellationToken, Task>>> taskQueue = new();
        public void Enqueue(Func<CancellationToken, Task> task)
            => taskQueue.Enqueue(new List<Func<CancellationToken, Task>> { task });
        public void Enqueue(params Func<CancellationToken, Task>[] tasks) => taskQueue.Enqueue(tasks.ToList());
        public List<Func<CancellationToken, Task>> Dequeue()
            => taskQueue.TryDequeue(out var tasks) ? tasks : new List<Func<CancellationToken, Task>>();
        public int QSize => taskQueue.Count;

        public void Start() => executor.Start();
        public void Pause() => executor.Pause();
        public void Resume() => executor.Resume();
        public async Task Stop() => await executor.Stop();
        public void Clear() => taskQueue.Clear();
    }
}
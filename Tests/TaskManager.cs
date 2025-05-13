using NUnit.Framework;
using System.Diagnostics;
using IIOTFramework.Core;

namespace TaskManagerTests;

public class Tests
{
    private readonly TaskManager taskManager = new TaskManager();
    private readonly Stopwatch stopwatch = Stopwatch.StartNew();
    [SetUp]
    public void Setup()
    {
        Console.WriteLine("TaskManager Test Start!");
    }

    [Test]
    public async Task TaskExecutor_Start_And_Stop()
    {
        static async Task ReadDevAsync(CancellationToken tkn)
        {
            Console.WriteLine("Task Executed");
            await Task.Delay(100, tkn);
            Console.WriteLine("Task Resume aaaaaaa");
        }
        taskManager.Enqueue(ReadDevAsync);
        taskManager.Enqueue(ReadDevAsync);
        taskManager.Enqueue(ReadDevAsync);
        stopwatch.Restart();
        Console.WriteLine($"[{stopwatch.ElapsedMilliseconds}] Run Start");
        taskManager.Start();
        while (stopwatch.ElapsedMilliseconds < 1000) {
            Console.WriteLine($"[{stopwatch.ElapsedMilliseconds}] Not Block!!!");
            Thread.Sleep(100);
        }
        await taskManager.Stop();
    }

    [Test]
    public async Task TaskExecutor_Null_Start()
    {
        stopwatch.Restart();
        Console.WriteLine($"[{stopwatch.ElapsedMilliseconds}] Run Start");
        taskManager.Start();
        while (stopwatch.ElapsedMilliseconds < 1000) {
            Console.WriteLine($"[{stopwatch.ElapsedMilliseconds}] Not Block!!!");
            Thread.Sleep(100);
        }
        await taskManager.Stop();
    }

    [TestCase(1000, 2000)]
    [TestCase(1000, 500)]
    public async Task TaskExecutor_Should_Process_Parallel_Multiple_Tasks(int delay, int breakdelay)
    {
        async Task ReadDevAsync(CancellationToken cts)
        {
            Console.WriteLine($"[{stopwatch.ElapsedMilliseconds}] Task Executed -> Parallel");
            await Task.Delay(delay, cts);
            Console.WriteLine($"[{stopwatch.ElapsedMilliseconds}] Task Resume");
        }
        taskManager.Enqueue(ReadDevAsync, ReadDevAsync, ReadDevAsync);
        taskManager.Enqueue(ReadDevAsync);
        taskManager.Enqueue(ReadDevAsync);
        stopwatch.Restart();
        Console.WriteLine($"[{stopwatch.ElapsedMilliseconds}] Run Start");
        taskManager.Start();
        Thread.Sleep(breakdelay);
        Console.WriteLine($"[{stopwatch.ElapsedMilliseconds}] Run Stop");
        await taskManager.Stop();
    }
}
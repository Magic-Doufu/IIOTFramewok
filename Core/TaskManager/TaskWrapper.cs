public interface IAsyncWrapper
{
    Task<T> RunAsync<T>(Func<T> f, CancellationToken token);
    Task<TRes> RunAsync<TArgs, TRes>(Func<TArgs[], TRes> f, TArgs[] args, CancellationToken token);
    Task<bool> RunAsync(Action f, CancellationToken token); // for void type function.
}
public abstract class AsyncWrapper : IAsyncWrapper
{
    protected virtual async Task<TRes> ExecuteAsync<TRes>(Func<TRes> f, CancellationToken token) 
        => await Task.Run(() => f(), token);

    protected virtual async Task<TRes> ExecuteAsync<TArgs, TRes>(Func<TArgs[], TRes> f, TArgs[] args, CancellationToken token) 
        => await Task.Run(() => f(args), token);

    protected virtual async Task<bool> ExecuteVoidAsync(Action action, CancellationToken token)
        => await Task.Run(() =>
        {
            try
            {
                action();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"error: {e.Message}");
                return false;
            }
        }, token);

    public virtual async Task<TRes> RunAsync<TRes>(Func<TRes> f, CancellationToken token) 
        => await ExecuteAsync(f, token);
    public virtual async Task<TRes> RunAsync<TArgs, TRes>(Func<TArgs[], TRes> f, TArgs[] args, CancellationToken token) 
        => await ExecuteAsync(f, args, token);
    public async Task<bool> RunAsync(Action f, CancellationToken token)
        => await ExecuteVoidAsync(f, token);
    public AsyncWrapper(){}
}
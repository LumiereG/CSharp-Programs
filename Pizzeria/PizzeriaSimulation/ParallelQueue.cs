namespace PizzeriaSimulation;

/// <summary>
/// Main data structure for our simulation.
/// </summary>
public sealed class ParallelQueue<T>
	: IParallelQueue<T>, IPausableQueue<T>, IDisposable
	where T : class
{
    List<T> Pizza { get; set; } = new List<T>();

    private SemaphoreSlim _itemSemaphore;
    private SemaphoreSlim _emptySemaphore;
    private readonly ManualResetEventSlim _pauseEnqueueEvent = new ManualResetEventSlim(true);
    private readonly ManualResetEventSlim _pauseDequeueEvent = new ManualResetEventSlim(true);
    public ParallelQueue(int maxSize)
	{
        _itemSemaphore = new SemaphoreSlim(0, maxSize);
        _emptySemaphore = new SemaphoreSlim(maxSize, maxSize);

    }

	public void Dispose()
	{
        _itemSemaphore.Dispose();
        _emptySemaphore.Dispose();
        _pauseEnqueueEvent.Dispose();
        _pauseDequeueEvent.Dispose();
    }

	public T Dequeue()
	{
        _pauseDequeueEvent.Wait();
        _itemSemaphore.Wait();
        T ret;
        lock (Pizza)
        {
            ret = Pizza[0];
            Pizza.RemoveAt(0);

        }
        _emptySemaphore.Release();
        return ret;
	}

	public void Enqueue(T item)
	{
        _pauseEnqueueEvent.Wait();

        _emptySemaphore.Wait();
        lock (Pizza)
        {
            Pizza.Add(item); 
        }
        _itemSemaphore.Release();
    }

	public async Task<T?> TryDequeueAsync(int timeoutMilliseconds, CancellationToken cancellationToken)
	{
        try
        {
            bool result = await _itemSemaphore.WaitAsync(timeoutMilliseconds, cancellationToken);
            if (!result) return default(T);
            _pauseDequeueEvent.Wait(cancellationToken);
        }
        catch (Exception ex)
        {
            return default;
        }
        T ret;
        lock (Pizza)
        {
            ret = Pizza[0];
            Pizza.RemoveAt(0);
        }
        _emptySemaphore.Release();
        return ret;
    }

	public async Task<bool> TryEnqueueAsync(T item, int timeoutMilliseconds, CancellationToken cancellationToken)
	{
        try
        {
            bool result = await _emptySemaphore.WaitAsync(timeoutMilliseconds, cancellationToken);
            if (!result) return default;
            _pauseEnqueueEvent.Wait(cancellationToken);
        }
        catch (Exception ex)
        {
            return false;
        }
        lock (Pizza)
        {
            Pizza.Add(item);
        }
        _itemSemaphore.Release();
        return true;
    }

	public void PauseDequeue()
	{
        _pauseDequeueEvent.Reset();
    }

	public void PauseEnqueue()
	{
        _pauseEnqueueEvent.Reset();
    }

	public void ResumeDequeue()
	{
        _pauseDequeueEvent.Set();
    }

	public void ResumeEnqueue()
	{
        _pauseEnqueueEvent.Set();
    }
}
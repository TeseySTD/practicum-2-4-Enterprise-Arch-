namespace Nimble.Modulith.Email.Interfaces;

public interface IQueueService<T>
{
    ValueTask EnqueueAsync(T item, CancellationToken ct = default);
    ValueTask<T> DequeueAsync(CancellationToken ct = default);
}
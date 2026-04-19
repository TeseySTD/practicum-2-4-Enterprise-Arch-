using System.Threading.Channels;
using Nimble.Modulith.Email.Interfaces;

namespace Nimble.Modulith.Email.Services;

public class ChannelQueueService<T> : IQueueService<T>
{
    private readonly Channel<T> _channel = Channel.CreateUnbounded<T>();

    public async ValueTask EnqueueAsync(T item, CancellationToken ct = default) =>
        await _channel.Writer.WriteAsync(item, ct);

    public async ValueTask<T> DequeueAsync(CancellationToken ct = default) => await _channel.Reader.ReadAsync(ct);
}
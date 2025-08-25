using System.Threading.Channels;
using UserService.Contract.Infrastructure;

namespace UserService.Infrastructure.Services;

public class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Channel<Func<CancellationToken, Task>> _queue = Channel.CreateUnbounded<Func<CancellationToken, Task>>();
    
    public void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem)
    {
        if (workItem == null) throw new ArgumentNullException(nameof(workItem));
        _queue.Writer.TryWrite(workItem);
    }

    public async ValueTask<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
    {
        var workItem = await _queue.Reader.ReadAsync(cancellationToken);
        return workItem;
    }
}

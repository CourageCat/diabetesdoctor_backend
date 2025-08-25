//using Kafka.Bentanik.Models;

//namespace AuthService.Api.Infrastructures.Services;

//public class OutboxStore : IOutboxBentanikStore
//{
//    private readonly ApplicationDbContext _context;

//    public OutboxStore(ApplicationDbContext context)
//    {
//        _context = context;
//    }

//    public async Task<List<OutboxBentanikMessage>> GetUnsentMessagesAsync(int maxCount, CancellationToken ct)
//    {
//        return await _context.OutboxMessages
//            .Where(x => !x.Processed && x.Status != "DeadLettered")
//            .OrderBy(x => x.CreatedAt)
//            .Take(maxCount)
//            .ToListAsync(ct);
//    }

//    public async Task MarkAsSentAsync(string id, CancellationToken ct)
//    {
//        var msg = await _context.OutboxMessages.FindAsync([id], ct);
//        if (msg != null)
//        {
//            msg.Processed = true;
//            msg.Status = "Success";
//            msg.ProcessedAt = DateTime.UtcNow;
//            await _context.SaveChangesAsync(ct);
//        }
//    }

//    public async Task IncrementRetryAsync(string id, CancellationToken ct)
//    {
//        var msg = await _context.OutboxMessages.FindAsync([id], ct);
//        if (msg != null)
//        {
//            msg.RetryCount++;
//            await _context.SaveChangesAsync(ct);
//        }
//    }

//    public async Task MoveToDeadLetterAsync(string id, CancellationToken ct)
//    {
//        var msg = await _context.OutboxMessages.FindAsync([id], ct);
//        if (msg != null)
//        {
//            msg.Status = "DeadLettered";
//            msg.Processed = true;
//            msg.ProcessedAt = DateTime.UtcNow;
//            await _context.SaveChangesAsync(ct);
//        }
//    }
//}

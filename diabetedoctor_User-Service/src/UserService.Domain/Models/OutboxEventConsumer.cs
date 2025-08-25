using System.ComponentModel.DataAnnotations;

namespace UserService.Domain.Models;

public class OutboxEventConsumer
{
    [Key]
    public string EventId { get; private init; } = null!;
    public string Name { get; private set; } = string.Empty;

    public static OutboxEventConsumer Create(string eventId, string name)
    {
        return new OutboxEventConsumer
        {
            EventId = eventId,
            Name = name
        };
    }
}
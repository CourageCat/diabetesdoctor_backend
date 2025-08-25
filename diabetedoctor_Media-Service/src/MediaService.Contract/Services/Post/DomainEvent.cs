namespace MediaService.Contract.Services.Post;

public record PostCreatedEvent(string Id, string Title, string ImageUrl) : IDomainEvent;

namespace ChatService.Contract.Services.Conversation.IntegrationCommands;

public record DeleteOldGroupAvatarCommand : ICommand
{
    public string ImagePublicId { get; init; } = null!;
}
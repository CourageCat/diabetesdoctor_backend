// using MediaService.Contract.Common.Constant.Event;
// using MediaService.Contract.EventBus.Abstractions;
// using MediaService.Contract.EventBus.Events;
// using MediaService.Contract.EventBus.Events.PostIntegrationEvents;
// using MediaService.Contract.Services.Post;
//
// namespace MediaService.Application.UseCase.V1.Events;
//
// public class WhenUserCreatedEventHandler : IDomainEventHandler<PostCreatedEvent>
// {
//     private readonly IEventPublisher _eventPublisher;
//
//     public WhenUserCreatedEventHandler(IEventPublisher eventPublisher)
//     {
//         _eventPublisher = eventPublisher;
//     }
//
//     public async Task Handle(PostCreatedEvent postCreatedEvent, CancellationToken cancellationToken)
//     {
//         var postCreatedIntegrationEvent = new PostCreatedIntegrationEvent
//         {
//             PostId = postCreatedEvent.Id,
//             Title = postCreatedEvent.Title,
//             Thumbnail = postCreatedEvent.ImageUrl,
//         };
//         await _eventPublisher.PublishAsync(MediaConstant.PostTopic, postCreatedIntegrationEvent);
//     }
// }

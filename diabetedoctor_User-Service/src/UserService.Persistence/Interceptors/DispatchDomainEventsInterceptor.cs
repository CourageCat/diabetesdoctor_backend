// namespace UserService.Persistence.Interceptors;
//
// public class DispatchDomainEventsInterceptor(IServiceScopeFactory scopeFactory) : SaveChangesInterceptor
// {
//     public override async ValueTask<int> SavedChangesAsync(
//         SaveChangesCompletedEventData eventData,
//         int result,
//         CancellationToken cancellationToken = default)
//     {
//         await DispatchDomainEvents(eventData.Context);
//         return await base.SavedChangesAsync(eventData, result, cancellationToken);
//     }
//
//     public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
//     {
//         _ = DispatchDomainEvents(eventData.Context);
//         return base.SavedChanges(eventData, result);
//     }
//
//     private async Task DispatchDomainEvents(DbContext? context)
//     {
//         if (context == null) return;
//
//         var aggregates = context.ChangeTracker
//             .Entries<IAggregateRoot>()
//             .Where(e => e.Entity.DomainEvents.Any())
//             .Select(e => e.Entity)
//             .ToList();
//
//         var domainEvents = aggregates
//             .SelectMany(a => a.DomainEvents)
//             .ToList();
//
//         aggregates.ForEach(a => a.ClearDomainEvents());
//
//         using var scope = scopeFactory.CreateScope();
//         var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
//
//         foreach (var domainEvent in domainEvents)
//         {
//             await mediator.Publish(domainEvent);
//         }
//     }
// }

using ChatService.Application.Mapping;
using ChatService.Contract.Services.User.Commands;

namespace ChatService.Application.UseCase.V1.IntegrationCommands.Users;

public class UpdateUserCommandHandler(
    IUserRepository userRepository, 
    IParticipantRepository participantRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateUserCommand>
{
    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.FindSingleAsync(x => x.UserId.Id.Equals(request.Id), cancellationToken: cancellationToken);

        if (user is null)
        {
            throw new UserExceptions.UserNotFoundException();
        }
        
        var avatar = !string.IsNullOrWhiteSpace(request.Avatar) ? Image.Of("avatar",request.Avatar) : null;
        var fullname = request.FullName is not null ? Mapper.MapFullName(request.FullName) : null;
        user.Modify(fullname, avatar, request.PhoneNumber, request.Email);
        
        await userRepository.ReplaceOneAsync(unitOfWork.ClientSession, user, cancellationToken);

        var participants = await participantRepository.FindListAsync(
            u => u.UserId == user.UserId,
            cancellationToken: cancellationToken);
        var participantIds = participants.Select(p => p.Id).ToList();
        
        var updates = new List<UpdateDefinition<Participant>>();
        if (fullname is not null)
        {
            updates.AddRange([
                Builders<Participant>.Update.Set(x => x.FullName, fullname),
                Builders<Participant>.Update.Set(x => x.DisplayName, fullname.ToString())
            ]);
        }

        if (avatar is not null)
        {
            updates.Add(Builders<Participant>.Update.Set(x => x.Avatar, avatar));    
        }

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            updates.Add(Builders<Participant>.Update.Set(x => x.PhoneNumber, request.PhoneNumber));
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            updates.Add(Builders<Participant>.Update.Set(x => x.Email, request.Email));
        }

        if (updates.Count > 0)
        {
            updates.Add(Builders<Participant>.Update.Set(x => x.ModifiedDate, CurrentTimeService.GetCurrentTime()));
        }
        
        var filters = Builders<Participant>.Filter.In(x => x.Id, participantIds);
        await participantRepository.UpdateManyAsync(unitOfWork.ClientSession,filters, Builders<Participant>.Update.Combine(updates), cancellationToken);
            
        return Result.Success();
    }
}
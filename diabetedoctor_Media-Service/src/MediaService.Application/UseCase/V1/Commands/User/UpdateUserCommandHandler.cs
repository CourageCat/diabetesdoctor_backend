using System.Drawing;
using MapsterMapper;
using MediaService.Contract.Services.User;
using MediaService.Domain.Abstractions.Repositories;
using MongoDB.Driver;
using Image = MediaService.Domain.ValueObjects.Image;
using Mapper = MediaService.Application.Mapping.Mapper;

namespace MediaService.Application.UseCase.V1.Commands.User;

public sealed class UpdateUserCommandHandler  : ICommandHandler<UpdateUserCommand, Success>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    } 
    public async Task<Result<Success>> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var userFound = await _userRepository.FindSingleAsync(user => user.UserId.Id == command.UserId, cancellationToken);
        if (userFound is null)
        {
            throw new Exception("User not found");
        }
        
        var update = new List<UpdateDefinition<Domain.Models.User>>();

        if (command.FullName is not null)
        {
            var fullname = Mapper.MapFullName(command.FullName);
            update.Add(Builders<Domain.Models.User>.Update.Set(x => x.FullName, fullname.ToString()));
        }
        if (!string.IsNullOrEmpty(command.Avatar))
        {
            var avatar = Image.Of(userFound.FullName + "_avatar", command.Avatar);
            update.Add(Builders<Domain.Models.User>.Update.Set(x => x.Avatar, avatar));
        }

        var updateDefinition = Builders<Domain.Models.User>.Update.Combine(update);
        await _userRepository.UpdateOneAsync(_unitOfWork.ClientSession, userFound.Id, updateDefinition, cancellationToken);
        return Result.Success(new Success("", ""));
    }
}
using MapsterMapper;
using MediaService.Contract.Services.User;
using MediaService.Domain.Abstractions;
using MediaService.Domain.Abstractions.Repositories;
using MediaService.Domain.ValueObjects;
using MongoDB.Bson;
using Mapper = MediaService.Application.Mapping.Mapper;

namespace MediaService.Application.UseCase.V1.Commands.User;

public sealed class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Success>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Success>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var id = ObjectId.GenerateNewId();
        var fullname = Mapper.MapFullName(command.FullName);
        var role = Mapper.MapRoleFromInt(command.Role);
        var userCreated = Domain.Models.User.Create(id, fullname.ToString(), Image.Of(fullname + "_avatar", command.PublicUrl),
            UserId.Of(command.Id), role);
        await _userRepository.CreateAsync(_unitOfWork.ClientSession, userCreated, cancellationToken);
        return Result.Success(new Success(UserMessage.CreateUserSuccessfully.GetMessage().Code,
            UserMessage.CreateUserSuccessfully.GetMessage().Message));
    }
}
using Role = AuthService.Api.Persistences.Data.Models.Role;

namespace AuthService.Api.Features.Hospital.Commands.Handlers;

public sealed class DoctorCreatedProfileCommandHandler : ICommandHandler<DoctorCreatedProfileCommand, Success>
{
    private readonly IPasswordHashService _passwordHashService;
    private readonly IRepositoryBase<User, Guid> _userRepository;
    private readonly IRepositoryBase<Role, Guid> _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DoctorCreatedProfileCommandHandler(IRepositoryBase<User, Guid> userRepository, IUnitOfWork unitOfWork, IRepositoryBase<Role, Guid> roleRepository, IPasswordHashService passwordHashService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _roleRepository = roleRepository;
        _passwordHashService = passwordHashService;
    }
    
    public async Task<Result<Success>> Handle(DoctorCreatedProfileCommand command, CancellationToken cancellationToken)
    {
        // Bước 1: Kiểm tra số điện thoại đã tồn tại trong database chưa (tránh trùng lặp)
        var doctorExists = await _userRepository.AnyAsync(u => u.PhoneNumber == command.PhoneNumber, cancellationToken);
        if (doctorExists)
            throw new Exception();

        // Bước 2: Tạo đối tượng doctor mới dựa trên dữ liệu từ event
        var doctor = await CreateDoctorAsync(command, cancellationToken);

        // Bước 3: Thêm doctor mới vào DbContext và lưu thay đổi
        _userRepository.Add(doctor);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new Success("", ""));
    }
    
    private async Task<User> CreateDoctorAsync(DoctorCreatedProfileCommand command, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.FindSingleAsync(r => r.RoleType == RoleType.Doctor, cancellationToken);
        if (role == null)
            throw new Exception($"Role '{RoleType.Doctor}' không tồn tại trong hệ thống.");

        var avatar = new Image(command.FullName + "_image", command.Avatar);

        var doctor = User.Create(email: "", avatar: avatar, phoneNumber: command.PhoneNumber, fullName: command.FullName, id: command.UserId);
        var passwordHash = _passwordHashService.HashPassword(AuthConstants.FirstPassword);
        doctor.AddPhoneLogin(command.PhoneNumber, passwordHash);
        doctor.AssignRole(role.Id);

        return doctor;
    }
}
using Role = AuthService.Api.Persistences.Data.Models.Role;

namespace AuthService.Api.Features.Hospital.Commands.Handlers;

public class HospitalStaffCreatedProfileCommandHandler : ICommandHandler<HospitalStaffCreatedProfileCommand, Success>
{
    private readonly IPasswordHashService _passwordHashService;
    private readonly IRepositoryBase<User, Guid> _userRepository;
    private readonly IRepositoryBase<Role, Guid> _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public HospitalStaffCreatedProfileCommandHandler(IPasswordHashService passwordHashService, IRepositoryBase<User, Guid> userRepository, IRepositoryBase<Role, Guid> roleRepository, IUnitOfWork unitOfWork)
    {
        _passwordHashService = passwordHashService;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }


    public async Task<Result<Success>> Handle(HospitalStaffCreatedProfileCommand command, CancellationToken cancellationToken)
    {
        // Bước 1: Kiểm tra email đã tồn tại trong database chưa (tránh trùng lặp)
        var hospitalStaffExists = await _userRepository.AnyAsync(u => u.Email == command.Email, cancellationToken);
        if (hospitalStaffExists)
            throw new Exception();

        // Bước 2: Tạo đối tượng hospitalStaff mới dựa trên dữ liệu từ event
        var hospitalStaff = await CreateHospitalStaffAsync(command, cancellationToken);

        // Bước 3: Thêm hospitalStaff mới vào DbContext và lưu thay đổi
        _userRepository.Add(hospitalStaff);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new Success("", ""));
    }
    
    private async Task<User> CreateHospitalStaffAsync(HospitalStaffCreatedProfileCommand command, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.FindSingleAsync(r => r.RoleType == RoleType.HospitalStaff, cancellationToken);
        if (role == null)
            throw new Exception($"Role '{RoleType.HospitalStaff}' không tồn tại trong hệ thống.");

        var avatar = new Image(command.FullName + "_image", command.Avatar);

        var hospitalStaff = User.Create(email: command.Email, avatar: avatar, phoneNumber: "", fullName: command.FullName, id: command.UserId);
        var passwordHash = _passwordHashService.HashPassword(AuthConstants.FirstPassword);
        hospitalStaff.AddEmailLogin(command.Email, passwordHash);
        hospitalStaff.AssignRole(role.Id);

        return hospitalStaff;
    }
}
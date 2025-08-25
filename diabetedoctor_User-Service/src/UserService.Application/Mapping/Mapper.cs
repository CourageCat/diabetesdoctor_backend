namespace UserService.Application.Mapping;

public static class Mapper
{
    /// <summary>
    /// Map int value sang Enum Role.
    /// </summary>
    public static RoleType MapRoleFromInt(int value)
    {
        return RoleMapper.MapFromInt(value);
    }
    
    /// <summary>
    /// Map DTO sang ValueObject FullName
    /// </summary>
    public static FullName MapFullName(FullNameDto dto)
    {
        return dto.ToDomain();
    }
    
    /// <summary>
    /// Map ValueObject sang DTO FullNameDto
    /// </summary>
    public static FullNameDto MapFullNameDto(FullName fullName)
    {
        return fullName.ToDto();
    }
}
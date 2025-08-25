using CloudinaryDotNet.Actions;
using MediaService.Contract.Enumarations;
using MediaService.Contract.Enumarations.User;

namespace MediaService.Application.Mapping;

public static class RoleMapper
{
    public static RoleType MapFromInt(int value)
    {
        if (Enum.IsDefined(typeof(RoleType), value))
        {
            return (RoleType)value;
        }

        throw new ArgumentException($"Không thể map giá trị [{value}] sang enum {nameof(RoleType)}");
    }
}
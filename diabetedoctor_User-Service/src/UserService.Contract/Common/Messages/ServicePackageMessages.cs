namespace UserService.Contract.Common.Messages;

public enum ServicePackageMessages
{
    [Message("Tạo gói dịch vụ thành công.", "service_package_01")]
    CreateServicePackageSuccessfully,
    [Message("Danh sách gói dịch vụ.", "service_package_02")]
    GetAllServicePackagesSuccessfully,
    [Message("Danh sách gói dịch vụ đã mua.", "service_package_03")]
    GetAllServicePackagesPurchasedSuccessfully,
    [Message("Chi tiết gói dịch vụ.", "service_package_04")]
    GetServicePackageByIdSuccessfully,
    
    [Message("Không tìm thấy gói dịch vụ!", "package_feature_error_01")]
    ServicePackageNotFound,
}
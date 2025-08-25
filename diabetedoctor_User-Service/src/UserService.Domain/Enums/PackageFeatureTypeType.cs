using System.ComponentModel;

namespace UserService.Domain.Enums;

public enum PackageFeatureTypeType
{
  [Description("Số lượt tư vấn")]
  MaxConsultation,
  [Description("Yêu cầu riêng")]
  AdditionalNotes
}
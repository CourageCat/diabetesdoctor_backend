using UserService.Contract.Helpers;
using UserService.Domain.ValueObjects;
using Image = UserService.Domain.ValueObjects.Image;

namespace UserService.Persistence.SeedData;

public static class SeedData
{
    public static void Seed(ApplicationDbContext context, IConfiguration configuration)
    {
        var moderatorId = new UuidV7().Value;
        var moderatorGuidId = Guid.Parse("83f040ab-1d35-42a1-b081-0df2b660eba6");
        var adminId = new UuidV7().Value;
        var adminGuidId = Guid.Parse("0f51cb9e-ec33-47e0-9af1-34a1182fe56d");
        // var hospitalId = new UuidV7().Value;
        // var hospitalAdminId = new UuidV7().Value;
        // var hospitalAdminGuidId = Guid.Parse("bcbc7f41-87e9-4f14-bef2-8efaac1af534");
        // var hospitalStaffId = new UuidV7().Value;
        // var hospitalStaffGuidId = Guid.Parse("785d54e6-92f6-4d06-9271-d2509a2c1e80");
        // var doctorId = new UuidV7().Value;
        // var doctorGuidId = Guid.Parse("9554b171-acdc-42c3-8dec-5d3aba44ca99");
        // var patient1Id = new UuidV7().Value;
        // var patient1GuidId = Guid.Parse("83cc8e98-9a98-4c4b-93bd-fe6d2dc7f99c");
        // var template1Id = Guid.NewGuid();
        // var template2Id = Guid.NewGuid();
        // var template1 = CarePlanMeasurementTemplate.CreateEmpty();
        // var template2 = CarePlanMeasurementTemplate.CreateEmpty();
        // var instance1Id = Guid.NewGuid();
        // var instance2Id = Guid.NewGuid();
        var servicePackage1Id = new UuidV7().Value;
        var servicePackage2Id = new UuidV7().Value;
        var servicePackage3Id = new UuidV7().Value;
        

        if (!context.UserInfos.Any())
        {
            context.UserInfos.AddRange(
                UserInfo.CreateForSeedData(moderatorGuidId, "moderator@diabetesdoctor.com", null,
                    Image.Of("professional-service-manager-vector-bicolor-icon-image_1322553-61939_wfhrfn",
                        "https://res.cloudinary.com/dc4eascme/image/upload/v1750172946/diabetesdoctor/professional-service-manager-vector-bicolor-icon-image_1322553-61939_wfhrfn.avif"),
                    FullName.Create("Quản", "trị", "viên"), DateTime.UtcNow.AddYears(-30), GenderType.Male,
                    RoleType.Moderator),
                UserInfo.CreateForSeedData(adminGuidId, "admin@diabetesdoctor.com", null,
                    Image.Of("admin_person_user_man_2839_tdcvdj",
                        "https://res.cloudinary.com/dc4eascme/image/upload/v1751550137/diabetesdoctor/admin_person_user_man_2839_tdcvdj.webp"),
                    FullName.Create("Admin", "", "1"), DateTime.UtcNow.AddYears(-30), GenderType.Male,
                    RoleType.SystemAdmin)
                // UserInfo.CreateForSeedData(hospitalAdminGuidId, "benhvienA@diabetesdoctor.com", null,
                //     Image.Of("1_rkjwzx",
                //         "https://res.cloudinary.com/dc4eascme/image/upload/v1754859337/1_rkjwzx.jpg"),
                //     FullName.Create("Admin", "", "bệnh viện A"), DateTime.UtcNow.AddYears(-30), GenderType.Male,
                //     RoleType.HospitalStaff),
                // UserInfo.CreateForSeedData(hospitalStaffGuidId, "nhanvienA@diabetesdoctor.com", null,
                //     Image.Of("professional-service-manager-vector-bicolor-icon-image_1322553-61939_wfhrfn",
                //         "https://res.cloudinary.com/dc4eascme/image/upload/v1750172946/diabetesdoctor/professional-service-manager-vector-bicolor-icon-image_1322553-61939_wfhrfn.avif"),
                //     FullName.Create("Nhân", "viên", "A"), DateTime.UtcNow.AddYears(-30), GenderType.Male,
                //     RoleType.HospitalStaff),
                // UserInfo.CreateForSeedData(doctorGuidId, null, "0987654321",
                //     Image.Of(
                //         "vector-illustration-doctor-avatar-photo-doctor-fill-out-questionnaire-banner-set-more-doctor-health-medical-icon_469123-417_nvqosc",
                //         "https://res.cloudinary.com/dc4eascme/image/upload/v1750172946/diabetesdoctor/vector-illustration-doctor-avatar-photo-doctor-fill-out-questionnaire-banner-set-more-doctor-health-medical-icon_469123-417_nvqosc.avif"),
                //     FullName.Create("Bác", "sĩ", "A"), DateTime.UtcNow.AddYears(-30), GenderType.Male, RoleType.Doctor),
                // UserInfo.CreateForSeedData(patient1GuidId, null, "0999999998",
                //     Image.Of(
                //         "1430453_lso7fr",
                //         "https://res.cloudinary.com/dc4eascme/image/upload/v1752100066/diabetesdoctor/1430453_lso7fr.png"),
                //     FullName.Create("Nguyễn", "Đỗ Chung", "Quý"), DateTime.UtcNow.AddYears(-30), GenderType.Male,
                //     RoleType.Patient)
            );
        }

        // if (!context.PatientProfiles.Any())
        // {
        //     var patient1Profile = PatientProfile.CreateForSeedData(patient1Id, patient1GuidId, DiabetesType.Type1,
        //         DiagnosisInfo.Of(DiagnosisRecencyType.OVER_1_YEAR, 2022), DiabetesCondition.Of(DiabetesType.Type1,
        //             InsulinInjectionFrequencyType.OncePerDay, TreatmentMethodType.InsulinInjection,
        //             ControlLevelType.GoodControl, true,
        //             new List<ComplicationType> { ComplicationType.Eye, ComplicationType.Other }, "Không có",
        //             ExerciseFrequencyType.None, EatingHabitType.Normal, false));
        //     var medicalHistories = new List<MedicalHistoryForDiabetesType>
        //     {
        //         MedicalHistoryForDiabetesType.HYPERTENSION,
        //         MedicalHistoryForDiabetesType.CHRONIC_LIVER_DISEASE,
        //         MedicalHistoryForDiabetesType.CARDIOVASCULAR_DISEASE,
        //         MedicalHistoryForDiabetesType.DYSPLIPIDEMIA
        //     };
        //     foreach (var medicalHistory in medicalHistories)
        //     {
        //         patient1Profile.AddMedicalHistory(medicalHistory);
        //     }
        //
        //     context.PatientProfiles.AddRange(patient1Profile);
        // }

        // if (!context.HealthRecords.Any())
        // {
        //     context.HealthRecords.AddRange(
        //         HealthRecord.Create(patient1Id, RecordType.Weight, WeightValue.Of(60, "kg"),
        //             DateTime.UtcNow.AddDays(-3)),
        //         HealthRecord.Create(patient1Id, RecordType.Weight, WeightValue.Of(61, "kg"), DateTime.UtcNow),
        //         HealthRecord.Create(patient1Id, RecordType.Height, HeightValue.Of(172.5, "cm"), DateTime.UtcNow)
        //     );
        // }

        // if (!context.CarePlanMeasurements.Any())
        // {
        //     template1 = CarePlanMeasurementTemplate.Create(template1Id, patient1Id, RecordType.BloodGlucose,
        //         HealthCarePlanPeriodType.Morning,
        //         HealthCarePlanSubTypeType.Fasting, "Theo dõi mức đường huyết khi đói");
        //     template2 = CarePlanMeasurementTemplate.Create(template2Id, patient1Id, RecordType.BloodPressure,
        //         HealthCarePlanPeriodType.BeforeSleep,
        //         null, "Kiểm tra huyết áp hàng ngày trước khi ngủ");
        //     context.CarePlanMeasurements.AddRange(template1, template2);
        // }
        //
        // if (!context.CarePlanMeasurementInstances.Any())
        // {
        //     context.CarePlanMeasurementInstances.AddRange(
        //         CarePlanMeasurementInstance.Create(instance1Id, patient1Id, template1,
        //             DateTime.UtcNow.Date.AddHours(1)),
        //         CarePlanMeasurementInstance.Create(instance2Id, patient1Id, template2,
        //             DateTime.UtcNow.Date.AddHours(15))
        //     );
        // }
        //
        if (!context.ModeratorProfiles.Any())
        {
            context.ModeratorProfiles.AddRange(
                ModeratorProfile.Create(moderatorId, moderatorGuidId)
            );
        }
        
        if (!context.AdminProfiles.Any())
        {
            context.AdminProfiles.AddRange(
                AdminProfile.Create(adminId, adminGuidId));
        }
        //
        // if (!context.HospitalProfiles.Any())
        // {
        //     context.HospitalProfiles.AddRange(
        //         HospitalProfile.CreateForSeedData(hospitalId, "Bệnh viện A", "benhvienA@diabetesdoctor.com",
        //             "0123123456",
        //             "https://benhvienA.com", "Thu Duc District, Ho Chi Minh City",
        //             "Bệnh viện A là cơ sở y tế hiện đại với đội ngũ y bác sĩ chuyên môn cao.",
        //             Image.Of("1116_upfront_microhospital_BSW_n6a1ex",
        //                 "https://res.cloudinary.com/dc4eascme/image/upload/v1752342971/diabetesdoctor/1116_upfront_microhospital_BSW_n6a1ex.webp"),
        //             adminId));
        // }
        //
        // if (!context.HospitalAdmins.Any())
        // {
        //     context.HospitalAdmins.AddRange(
        //         HospitalAdmin.Create(hospitalAdminId, hospitalAdminGuidId, hospitalId, adminId));
        // }
        //
        // if (!context.HospitalStaffs.Any())
        // {
        //     context.HospitalStaffs.AddRange(
        //         HospitalStaff.Create(hospitalStaffId, hospitalStaffGuidId, hospitalId, hospitalAdminId));
        // }
        //
        // if (!context.DoctorProfiles.Any())
        // {
        //     context.DoctorProfiles.AddRange(
        //         DoctorProfile.CreateForSeedData(doctorId, doctorGuidId, hospitalId, hospitalStaffId, 10,
        //             DoctorPositionType.Doctor,
        //             "Bác sĩ A là chuyên gia trong lĩnh vực nội tiết, với hơn 10 năm kinh nghiệm điều trị các bệnh tiểu đường."));
        // }

        if (!context.Medias.Any())
        {
            context.Medias.AddRange(
                Media.CreateForUserInfoForSeedData(new UuidV7().Value,
                    "professional-service-manager-vector-bicolor-icon-image_1322553-61939_wfhrfn",
                    "https://res.cloudinary.com/dc4eascme/image/upload/v1750172946/diabetesdoctor/professional-service-manager-vector-bicolor-icon-image_1322553-61939_wfhrfn.avif",
                    MediaType.Image, adminGuidId, moderatorGuidId),
                Media.CreateForUserInfoForSeedData(new UuidV7().Value,
                    "admin_person_user_man_2839_tdcvdj",
                    "https://res.cloudinary.com/dc4eascme/image/upload/v1751550137/diabetesdoctor/admin_person_user_man_2839_tdcvdj.webp",
                    MediaType.Image, adminGuidId, adminGuidId)
                // Media.CreateForUserInfoForSeedData(new UuidV7().Value,
                //     "professional-service-manager-vector-bicolor-icon-image_1322553-61939_wfhrfn",
                //     "https://res.cloudinary.com/dc4eascme/image/upload/v1750172946/diabetesdoctor/professional-service-manager-vector-bicolor-icon-image_1322553-61939_wfhrfn.avif",
                //     MediaType.Image, adminGuidId, hospitalStaffGuidId),
                // Media.CreateForUserInfoForSeedData(new UuidV7().Value,
                //     "vector-illustration-doctor-avatar-photo-doctor-fill-out-questionnaire-banner-set-more-doctor-health-medical-icon_469123-417_nvqosc",
                //     "https://res.cloudinary.com/dc4eascme/image/upload/v1750172946/diabetesdoctor/vector-illustration-doctor-avatar-photo-doctor-fill-out-questionnaire-banner-set-more-doctor-health-medical-icon_469123-417_nvqosc.avif",
                //     MediaType.Image, hospitalStaffGuidId, doctorGuidId),
                // Media.CreateForUserInfoForSeedData(new UuidV7().Value,
                //     "1430453_lso7fr",
                //     "https://res.cloudinary.com/dc4eascme/image/upload/v1752100066/diabetesdoctor/1430453_lso7fr.png",
                //     MediaType.Image, patient1GuidId, patient1GuidId),
                // Media.CreateForUserInfoForSeedData(new UuidV7().Value,
                //     "1_rkjwzx",
                //     "https://res.cloudinary.com/dc4eascme/image/upload/v1754859337/1_rkjwzx.jpg",
                //     MediaType.Image, hospitalAdminGuidId, hospitalAdminGuidId),
                // Media.CreateForHospitalForSeedData(new UuidV7().Value,
                //     "1116_upfront_microhospital_BSW_n6a1ex",
                //     "https://res.cloudinary.com/dc4eascme/image/upload/v1752342971/diabetesdoctor/1116_upfront_microhospital_BSW_n6a1ex.webp",
                //     MediaType.Image, adminGuidId, hospitalId),
                // Media.CreateForHospitalForSeedData(new UuidV7().Value,
                //     "sw_Hospital-patient-room-headwall-designed-by-white-HIMACS-solid-surface-material-1024x724_yhdbaf",
                //     "https://res.cloudinary.com/dc4eascme/image/upload/v1752342971/diabetesdoctor/sw_Hospital-patient-room-headwall-designed-by-white-HIMACS-solid-surface-material-1024x724_yhdbaf.jpg",
                //     MediaType.Image, adminGuidId, hospitalId),
                // Media.CreateForHospitalForSeedData(new UuidV7().Value,
                //     "ooking-ahead-the-outlook-for-australias-private-hospitals_x8fzws",
                //     "https://res.cloudinary.com/dc4eascme/image/upload/v1752342972/diabetesdoctor/ooking-ahead-the-outlook-for-australias-private-hospitals_x8fzws.jpg",
                //     MediaType.Image, adminGuidId, hospitalId)
            );
        }
        if (!context.ServicePackages.Any())
        {
            var servicePackage1 =
                ServicePackage.Create(servicePackage1Id, "Gói tư vấn riêng với 5 lượt có thời hạn 1 tháng",
                    "Gói dịch vụ tư vấn riêng cho khách hàng với bác sĩ, có 5 lượt tư vấn, có giá 10 nghìn VNĐ và có thời hạn 1 tháng",
                    10000, 5, 1, adminId);
            
            var servicePackage2 = 
                ServicePackage.Create(servicePackage2Id, "Gói tư vấn riêng với 20 lượt có thời hạn 3 tháng",
                    "Gói dịch vụ tư vấn riêng cho khách hàng với bác sĩ, có 20 lượt tư vấn, có giá 400 nghìn VNĐ và có thời hạn 3 tháng", 400000, 20, 3, adminId);
            
            var servicePackage3 = 
                ServicePackage.Create(servicePackage3Id, "Gói tư vấn riêng với 10 lượt có thời hạn 2 tháng",
                    "Gói dịch vụ tư vấn riêng cho khách hàng với bác sĩ, có 10 lượt tư vấn, có giá 250 nghìn VNĐ và có thời hạn 2 tháng", 250000, 10, 2, adminId);
            context.ServicePackages.AddRange(new List<ServicePackage>()
            {
                servicePackage1,
                servicePackage2,
                servicePackage3
            });
        }
        context.SaveChanges();
    }
}
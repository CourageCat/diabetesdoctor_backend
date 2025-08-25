using UserService.Contract.Common.DomainErrors;
using UserService.Contract.Services.Patients.Queries;
using UserService.Contract.Services.Patients.Responses;

namespace UserService.Application.UseCases.V1.Queries.Patients;

public sealed class GetPatientProfileQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetPatientProfileQuery, Success<PatientProfileResponse>>
{
    public async Task<Result<Success<PatientProfileResponse>>> Handle(GetPatientProfileQuery query,
        CancellationToken cancellationToken)
    {
        var userFound =
            await context.UserInfos
                .Include(user => user.PatientProfile)
                .FirstOrDefaultAsync(user => user.Id == query.UserId, cancellationToken);
        if (userFound is null)
        {
            return FailureFromMessage(PatientErrors.ProfileNotExist);
        }

        var diagnosisInfo = new DiagnosisInfoDto
        {
            DiagnosisRecency = userFound.PatientProfile.DiagnosisInfo.DiagnosisRecency.ToEnum<DiagnosisRecencyType, DiagnosisRecencyEnum>(),
            Year = userFound.PatientProfile.DiagnosisInfo.Year
        };

        var diabetesCondition = new DiabetesConditionDto
        {
            DiabetesType = userFound.PatientProfile.DiabetesCondition.DiabetesType.ToEnum<DiabetesType, DiabetesEnum>(),
            Type2TreatmentMethod = userFound.PatientProfile.DiabetesCondition.Type2TreatmentMethod
                .ToEnumNullable<TreatmentMethodType, TreatmentMethodEnum>(),
            ControlLevel = userFound.PatientProfile.DiabetesCondition.ControlLevel
                .ToEnumNullable<ControlLevelType, ControlLevelEnum>(),
            InsulinFrequency = userFound.PatientProfile.DiabetesCondition.InsulinFrequency
                .ToEnumNullable<InsulinInjectionFrequencyType, InsulinInjectionFrequencyEnum>(),
            HasComplications = userFound.PatientProfile.DiabetesCondition.HasComplications,
            Complications = userFound.PatientProfile.DiabetesCondition.Complications
                .Select(complication => complication.ToEnum<ComplicationType, ComplicationEnum>()).ToList(),
            OtherComplicationDescription = userFound.PatientProfile.DiabetesCondition.OtherComplicationDescription,
            ExerciseFrequency = userFound.PatientProfile.DiabetesCondition.ExerciseFrequency
                .ToEnumNullable<ExerciseFrequencyType, ExerciseFrequencyEnum>(),
            EatingHabit = userFound.PatientProfile.DiabetesCondition.EatingHabit.ToEnumNullable<EatingHabitType, EatingHabitEnum>(),
            UsesAlcoholOrTobacco = userFound.PatientProfile.DiabetesCondition.UsesAlcoholOrTobacco
        };
        

        var result = new PatientProfileResponse
        {
            Id =  userFound.Id.ToString(),
            PhoneNumber = userFound.PhoneNumber ?? string.Empty,
            Avatar = userFound.Avatar.Url,
            FullName = userFound.DisplayName,
            DateOfBirth = userFound.DateOfBirth,
            Gender = userFound.Gender.ToEnum<GenderType, GenderEnum>(),
            DiabetesType = userFound.PatientProfile.DiabetesType.ToEnum<DiabetesType, DiabetesEnum>(),
            DiagnosisInfo = diagnosisInfo,
            DiabetesCondition = diabetesCondition,
            MedicalHistories = userFound.PatientProfile.MedicalHistories.Select(medicalHistory => medicalHistory.ToEnum<MedicalHistoryForDiabetesType, MedicalHistoryForDiabetesEnum>()),
        };
        return Result.Success(new Success<PatientProfileResponse>(PatientMessages.GetPatientProfileSuccessfully.GetMessage().Code, PatientMessages.GetPatientProfileSuccessfully.GetMessage().Message, result));
    }
    
    private static Result<Success<PatientProfileResponse>> FailureFromMessage(Error error)
    {
        return Result.Failure<Success<PatientProfileResponse>>(error);
    }
}
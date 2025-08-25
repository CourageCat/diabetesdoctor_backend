namespace UserService.Contract.Services.Patients.Queries;

public record GetHealthRecordValuesQuery
    (Guid UserId,
    List<RecordEnum> RecordTypes,
    bool Newest = true,
    bool IsBelongToDoctorTemplate = false,
    DateTimeOffset? FromDate = null,
    DateTimeOffset? ToDate = null,
    bool OnePerType = true) : IQuery<Success<GetHealthRecordResponse>>;
namespace UserService.Contract.DTOs;

public record MinMax(double? Highest = 0, double? Lowest = 0);

public record GetHealthRecordResponse
    (List<HealthRecordDto> HealthRecords, MinMax? MinMax = null);

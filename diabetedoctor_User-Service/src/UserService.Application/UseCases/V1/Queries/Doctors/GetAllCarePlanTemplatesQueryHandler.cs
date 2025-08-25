using System.Text;
using UserService.Contract.Common.DomainErrors;
using UserService.Contract.Services.Doctors.Filters;
using UserService.Contract.Services.Doctors.Queries;
using UserService.Contract.Services.Doctors.Responses;

namespace UserService.Application.UseCases.V1.Queries.Doctors;

public sealed class GetAllCarePlanTemplatesQueryHandler(ApplicationDbContext context) : IQueryHandler<GetAllCarePlanTemplatesQuery, Success<CursorPagedResult<CarePlanTemplateResponse>>>
{
    public async Task<Result<Success<CursorPagedResult<CarePlanTemplateResponse>>>> Handle(GetAllCarePlanTemplatesQuery query, CancellationToken cancellationToken)
    {
        var patientFound = await context.PatientProfiles.FirstOrDefaultAsync(patient => patient.UserId == query.PatientId, cancellationToken: cancellationToken);
        if (patientFound is null)
        {
            return Result.Failure<Success<CursorPagedResult<CarePlanTemplateResponse>>>(PatientErrors.ProfileNotExist);
        }
        var doctorFound = await context.DoctorProfiles.FirstOrDefaultAsync(doctor => doctor.UserId == query.DoctorId, cancellationToken: cancellationToken);
        if (doctorFound is null)
        {
            return Result.Failure<Success<CursorPagedResult<CarePlanTemplateResponse>>>(DoctorErrors.ProfileNotExist);
        }
        var templateQuery = context.CarePlanMeasurements
            .AsSplitQuery();
        var cursorValues = new List<string>();
        if (query.Pagination.Cursor != null)
        {
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(query.Pagination.Cursor));
            cursorValues.AddRange(decoded.Split('|').ToList());
        }
        
        templateQuery = ApplyFilter(templateQuery, query.Filters, cursorValues, patientFound.Id, doctorFound.Id);
        templateQuery = ApplySorting(templateQuery, query.Filters);
        var templatesFoundAfterPagination = await templateQuery.Take(query.Pagination.PageSize + 1).ToListAsync(cancellationToken);
        var hasNext = templatesFoundAfterPagination.Count > query.Pagination.PageSize;
        var nextCursor = "";
        if (hasNext)
        {
            templatesFoundAfterPagination.RemoveRange(query.Pagination.PageSize, templatesFoundAfterPagination.Count - query.Pagination.PageSize);
            var lastDoctor = templatesFoundAfterPagination.ToList()[^1];
            nextCursor = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{GetSortCursorValue(lastDoctor, query.Filters.SortBy)}|{lastDoctor.Id}"));
        }
        var templateProjection = ApplyProjection(templatesFoundAfterPagination).ToList();
        var result = CursorPagedResult<CarePlanTemplateResponse>.Create(templateProjection, query.Pagination.PageSize, nextCursor, hasNext);
        return Result.Success(new Success<CursorPagedResult<CarePlanTemplateResponse>>(
            CarePlanTemplateMessages.GetAllCarePlanTemplatesSuccessfully.GetMessage().Code,
            CarePlanTemplateMessages.GetAllCarePlanTemplatesSuccessfully.GetMessage().Message,
            result));
    }

    private IQueryable<CarePlanMeasurementTemplate> ApplyFilter(IQueryable<CarePlanMeasurementTemplate> query, GetAllCarePlanTemplatesFilter filter, List<string> cursorValues, Guid patientId, Guid doctorId)
    {
        // Always filter out deleted templates
        query = query.Where(x =>
            x.PatientProfileId == patientId
            && x.DoctorProfileId == doctorId
            && x.IsDeleted == false);

        // Filter Cursor
        var isAsc = filter.SortDirection == SortDirectionEnum.Asc;
        if (cursorValues.Count != 0)
        {
            var isParseSecondCursorValueSuccess = Guid.TryParse(cursorValues[1], out Guid idInCursor);
            if (isParseSecondCursorValueSuccess)
            {
                if (filter.SortBy.ToLower().Equals("recordType"))
                {
                    var isParseFirstCursorValueSuccess = Enum.TryParse(cursorValues[0], out RecordType recordTypeInCursor);
                    if (isParseFirstCursorValueSuccess)
                    {
                        if (isAsc)
                        {
                            query = query.Where(x =>
                                x.RecordType > recordTypeInCursor ||
                                (x.RecordType == recordTypeInCursor && x.Id > idInCursor));
                        }
                        else
                        {
                            query = query.Where(x =>
                                x.RecordType < recordTypeInCursor ||
                                (x.RecordType == recordTypeInCursor && x.Id < idInCursor));
                        }
                    }
                }
                else if (filter.SortBy.ToLower().Equals("subType"))
                {
                    var isParseFirstCursorValueSuccess = Enum.TryParse(cursorValues[0], out HealthCarePlanSubTypeType subTypeInCursor);
                    if (isParseFirstCursorValueSuccess)
                    {
                        if (isAsc)
                        {
                            query = query.Where(x =>
                                x.Subtype > subTypeInCursor ||
                                (x.Subtype == subTypeInCursor && x.Id > idInCursor));
                        }
                        else
                        {
                            query = query.Where(x =>
                                x.Subtype < subTypeInCursor ||
                                (x.Subtype == subTypeInCursor && x.Id < idInCursor));
                        }
                    }
                }
                else if (filter.SortBy.ToLower().Equals("scheduledAt"))
                {
                    var isParseFirstCursorValueSuccess = TimeOnly.TryParse(cursorValues[0], out TimeOnly scheduledAtInCursor);
                    if (isParseFirstCursorValueSuccess)
                    {
                        if (isAsc)
                        {
                            query = query.Where(x =>
                                x.ScheduledAt > scheduledAtInCursor ||
                                (x.ScheduledAt == scheduledAtInCursor && x.Id > idInCursor));
                        }
                        else
                        {
                            query = query.Where(x =>
                                x.ScheduledAt < scheduledAtInCursor ||
                                (x.ScheduledAt == scheduledAtInCursor && x.Id < idInCursor));
                        }
                    }
                }
                else
                {
                    var isParseFirstCursorValueSuccess = DateTime.TryParse(cursorValues[0], out DateTime createdDateInCursor);
                    if (isParseFirstCursorValueSuccess)
                    {
                        if (isAsc)
                        {
                            query = query.Where(x =>
                                x.CreatedDate > createdDateInCursor ||
                                (x.CreatedDate == createdDateInCursor && x.Id > idInCursor));
                        }
                        else
                        {
                            query = query.Where(x =>
                                x.CreatedDate < createdDateInCursor ||
                                (x.CreatedDate == createdDateInCursor && x.Id > idInCursor));
                        }
                    }
                }
            }
        }

        // Search by reason
        if (!string.IsNullOrEmpty(filter.Search))
        {
            query = query.Where(x =>
                x.Reason!.ToLower().Contains(filter.Search.ToLower()));
        }

        // Filter by RecordType
        if (filter.RecordType != null)
        {
            var recordTypeFilter = filter.RecordType.Value.ToEnum<RecordEnum, RecordType>();
            query = query.Where(x => x.RecordType == recordTypeFilter);
        }

        // Filter by SubType
        if (filter.SubType != null)
        {
            var subTypeFilter = filter.SubType.Value.ToEnum<HealthCarePlanSubTypeEnum, HealthCarePlanSubTypeType>();
            query = query.Where(x => x.Subtype == subTypeFilter);
        }

        return query;
    }

    private IQueryable<CarePlanMeasurementTemplate> ApplySorting(IQueryable<CarePlanMeasurementTemplate> query, GetAllCarePlanTemplatesFilter filter)
    {
        var isAsc = filter.SortDirection == SortDirectionEnum.Asc;
        query = filter.SortBy switch
        {
            "recordType" => isAsc
                ? query.OrderBy(x => x.RecordType).ThenByDescending(x => x.Id)
                : query.OrderByDescending(x => x.RecordType).ThenByDescending(x => x.Id),
            "period" => isAsc
                ? query.OrderBy(x => x.Period).ThenByDescending(x => x.Id)
                : query.OrderByDescending(x => x.Period).ThenByDescending(x => x.Id),
            "subType" => isAsc
                ? query.OrderBy(x => x.Subtype).ThenByDescending(x => x.Id)
                : query.OrderByDescending(x => x.Subtype).ThenByDescending(x => x.Id),
            "scheduledAt" => isAsc
                ? query.OrderBy(x => x.ScheduledAt).ThenByDescending(x => x.Id)
                : query.OrderByDescending(x => x.ScheduledAt).ThenByDescending(x => x.Id),
            _ => isAsc
                ? query.OrderBy(x => x.CreatedDate).ThenByDescending(x => x.Id)
                : query.OrderByDescending(x => x.CreatedDate).ThenByDescending(x => x.Id),
        };

        return query;
    }

    private IEnumerable<CarePlanTemplateResponse> ApplyProjection(IEnumerable<CarePlanMeasurementTemplate> templates)
    {
        return templates
            .Select(template => new CarePlanTemplateResponse()
            {
                Id = template.Id.ToString(),
                RecordType = template.RecordType.ToEnum<RecordType, RecordEnum>(),
                Period = template.Period.ToEnumNullable<HealthCarePlanPeriodType, HealthCarePlanPeriodEnum>(),
                ScheduledAt = template.ScheduledAt,
                SubType = template.Subtype.ToEnumNullable<HealthCarePlanSubTypeType, HealthCarePlanSubTypeEnum>(),
                Reason =  template.Reason,
                CreatedDate = template.CreatedDate,
            });
    }
    
    private string GetSortCursorValue(CarePlanMeasurementTemplate template, string sortBy)
    {
        var result = sortBy switch
        {
            "recordType" => template.RecordType.ToString(),
            "period" => template.Period.ToString()!,
            "subType" => template.Subtype.ToString()!,
            "scheduledAt" => template.ScheduledAt.ToString()!,
            _ => template.CreatedDate.ToString()!,
        };
        return result;
    }
    
    private static Result<Success<CursorPagedResult<CarePlanTemplateResponse>>> FailureFromMessage(Error error)
    {
        return Result.Failure<Success<CursorPagedResult<CarePlanTemplateResponse>>>(error);
    }
}
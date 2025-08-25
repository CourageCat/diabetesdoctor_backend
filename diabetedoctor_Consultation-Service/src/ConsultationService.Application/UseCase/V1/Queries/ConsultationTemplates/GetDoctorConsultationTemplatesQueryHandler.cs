using System.Globalization;
using ConsultationService.Contract.DTOs.ConsultationTemplateDtos.Responses;
using ConsultationService.Contract.Enums;
using ConsultationService.Contract.Helpers;
using ConsultationService.Contract.Services.ConsultationTemplate.Queries;
using ConsultationService.Contract.Services.ConsultationTemplate.Responses;
using ConsultationService.Domain.Enums;

namespace ConsultationService.Application.UseCase.V1.Queries.ConsultationTemplates;

public sealed class GetDoctorConsultationTemplatesQueryHandler(
    IMongoDbContext mongoDbContext)
    : IQueryHandler<GetDoctorConsultationTemplatesQuery,
        Response<CursorPagedResult<GetDoctorConsultationTemplatesResponse>>>
{
    public async Task<Result<Response<CursorPagedResult<GetDoctorConsultationTemplatesResponse>>>> Handle(
        GetDoctorConsultationTemplatesQuery request, CancellationToken cancellationToken)
    {
        var filters = await BuildFilter(request, cancellationToken);
        if (filters.IsFailure)
        {
            return Result.Failure<Response<CursorPagedResult<GetDoctorConsultationTemplatesResponse>>>(filters.Error);
        }

        var sorter = BuildTemplateSort();

        var aggregateFluent = mongoDbContext.ConsultationTemplates
            .Aggregate()
            .Match(filters.Value)
            .Sort(sorter);

        var templates = await aggregateFluent
            .Group(
                template => template.Date,
                g => new GetDoctorConsultationTemplatesResponse
                {
                    Date = g.Key,
                    ConsultationTemplates = g.Select(x => new ConsultationTemplateResponseDto
                    {
                        Id = x.Id.ToString(),
                        StartTime = x.StartTime,
                        EndTime = x.EndTime,
                        Status = (ConsultationTemplateStatusEnum)x.Status
                    })
                })
            .SortBy(group => group.Date)
            .ToListAsync(cancellationToken);


        var result = CursorPagedResult<GetDoctorConsultationTemplatesResponse>.Create(templates, 0, 0,
            templates.Count > 0 ? templates[^1].Date.ToString("O", CultureInfo.InvariantCulture) : string.Empty,
            true);

        return Result.Success(new Response<CursorPagedResult<GetDoctorConsultationTemplatesResponse>>(
            ConsultationTemplateMessage.GetDoctorConsultationTemplates.GetMessage().Code,
            ConsultationTemplateMessage.GetDoctorConsultationTemplates.GetMessage().Message,
            result));
    }

    private async Task<Result<FilterDefinition<ConsultationTemplate>>> BuildFilter(
        GetDoctorConsultationTemplatesQuery query, CancellationToken cancellationToken)
    {
        var staffId = UserId.Of(query.UserId);
        var doctorId = UserId.Of(query.DoctorId);
        var builder = Builders<ConsultationTemplate>.Filter;
        var filters = new List<FilterDefinition<ConsultationTemplate>>
        {
            builder.Eq(ct => ct.DoctorId, doctorId),
            builder.Eq(ct => ct.IsDeleted, false)
        };

        if (query.Role == nameof(Role.HospitalStaff))
        {
            var staff = await mongoDbContext.Users
                .Find(u => u.UserId == staffId && u.IsDeleted == false && u.HospitalId != null)
                .FirstOrDefaultAsync(cancellationToken);
            if (staff is null) return Result.Failure<FilterDefinition<ConsultationTemplate>>(UserErrors.StaffNotFound);

            var doctorInHospital = await mongoDbContext.Users.Find(u =>
                    u.UserId == doctorId && u.HospitalId == staff.HospitalId && u.IsDeleted == false)
                .AnyAsync(cancellationToken);
            if (!doctorInHospital)
                return Result.Failure<FilterDefinition<ConsultationTemplate>>(UserErrors.DoctorNotBelongToHospital);

            if (query.Filter.FromDate == null || query.Filter.ToDate == null)
            {
                return Result.Success(builder.And(filters));
            }

            var exprFilter = new BsonDocument("$expr", new BsonDocument("$and", new BsonArray
            {
                new BsonDocument("$gte",
                    new BsonArray { new BsonDocument("$dayOfMonth", "$date"), query.Filter.FromDate.Value.Day }),
                new BsonDocument("$eq",
                    new BsonArray { new BsonDocument("$month", "$date"), query.Filter.FromDate.Value.Month }),
                new BsonDocument("$eq",
                    new BsonArray { new BsonDocument("$year", "$date"), query.Filter.FromDate.Value.Year }),
                new BsonDocument("$lte",
                    new BsonArray { new BsonDocument("$dayOfMonth", "$date"), query.Filter.ToDate.Value.Day }),
                new BsonDocument("$eq",
                    new BsonArray { new BsonDocument("$month", "$date"), query.Filter.ToDate.Value.Month }),
                new BsonDocument("$eq",
                    new BsonArray { new BsonDocument("$year", "$date"), query.Filter.ToDate.Value.Year })
            }));

            filters.Add(exprFilter);
        }
        else
        {
            filters.Add(builder.Eq(ct => ct.Status, ConsultationTemplateStatus.Available));

            var now = DateTime.SpecifyKind(CurrentTimeService.GetVietNamCurrentTime(), DateTimeKind.Utc);
            if (query.Filter.Month.HasValue)
            {
                var exprFilter = new BsonDocument("$expr", new BsonDocument("$and", new BsonArray
                {
                    new BsonDocument("$eq",
                        new BsonArray { new BsonDocument("$month", "$date"), query.Filter.Month.Value.Month }),
                    new BsonDocument("$eq",
                        new BsonArray { new BsonDocument("$year", "$date"), query.Filter.Month.Value.Year }),
                }));
                filters.Add(exprFilter);
            }
            else
            {
                var exprFilter = new BsonDocument("$expr", new BsonDocument("$and", new BsonArray
                {
                    new BsonDocument("$eq", new BsonArray { new BsonDocument("$month", "$date"), now.Month }),
                    new BsonDocument("$eq", new BsonArray { new BsonDocument("$year", "$date"), now.Year }),
                }));
                filters.Add(exprFilter);
            }

            filters.Add(new BsonDocument("$expr",
                new BsonDocument("$or", new BsonArray
                {
                    new BsonDocument("$and", new BsonArray
                    {
                        new BsonDocument("$eq", new BsonArray { new BsonDocument("$dayOfMonth", "$date"), now.Day }),
                        new BsonDocument("$gt", new BsonArray { "$start_time", now.ToString("HH:mm") })
                    }),
                    new BsonDocument("$gt", new BsonArray { "$date", now.Date })
                })
            ));
        }

        return Result.Success(builder.And(filters));
    }

    private static SortDefinition<ConsultationTemplate> BuildTemplateSort()
    {
        var builder = Builders<ConsultationTemplate>.Sort;
        var sorters = new List<SortDefinition<ConsultationTemplate>>
        {
            builder.Ascending(ct => ct.Date),
            builder.Ascending(ct => ct.StartTimeInMinutes)
        };
        return builder.Combine(sorters);
    }

    private static BsonDocument BuildTemplateProjection()
    {
        return new BsonDocument
        {
            { "_id", 1 },
            { "conversation_id", 1 },
            { "start_time", 1 },
            { "end_time", 1 },
            { "status", 1 }
        };
    }
}
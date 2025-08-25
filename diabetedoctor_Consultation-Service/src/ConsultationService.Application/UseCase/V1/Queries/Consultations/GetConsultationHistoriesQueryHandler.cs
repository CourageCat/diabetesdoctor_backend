using ConsultationService.Contract.DTOs.ConsultationDtos.Responses;
using ConsultationService.Contract.Enums;
using ConsultationService.Contract.Services.Consultation.Queries;
using ConsultationService.Contract.Services.ConsultationTemplate.Queries;
using ConsultationService.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace ConsultationService.Application.UseCase.V1.Queries.Consultations;

public sealed class GetConsultationHistoriesQueryHandler(
    IMongoDbContext mongoDbContext)
    : IQueryHandler<GetConsultationHistoriesQuery, Response<CursorPagedResult<ConsultationResponseDto>>>
{
    public async Task<Result<Response<CursorPagedResult<ConsultationResponseDto>>>> Handle(GetConsultationHistoriesQuery request, CancellationToken cancellationToken)
    {
        var pageSize = request.Pagination.PageSize > 0 ? request.Pagination.PageSize : 20;
        
        var filters = BuildFilter(request);
        var sorter = BuildConsultationSort();
        
        var isDoctor = request.Role == nameof(Role.Doctor);
        var projection = BuildConsultationProjection(isDoctor);
        var participantLookup = UserLookup();
        
        var consultations = await mongoDbContext.Consultations
            .Aggregate()
            .Match(filters)
            .Sort(sorter)
            .Limit(pageSize + 1)
            .Lookup<User, BsonDocument, IEnumerable<BsonDocument>, BsonDocument>(
                foreignCollection: mongoDbContext.Users,
                let: new BsonDocument("userId", isDoctor ? "$patient_id": "$doctor_id"),
                lookupPipeline: participantLookup,
                @as: "user")
            .Unwind("user", new AggregateUnwindOptions<BsonDocument>
            {
                PreserveNullAndEmptyArrays = true
            })
            .Project<ConsultationResponseDto>(projection)
            .ToListAsync(cancellationToken);
            
        var hasNext = consultations.Count > pageSize;

        if (hasNext)
        {
            consultations.RemoveRange(pageSize, consultations.Count - pageSize);
        }
        
        var result = CursorPagedResult<ConsultationResponseDto>.Create(consultations, 0, pageSize,
            hasNext ? consultations[^1].Id: string.Empty,
            hasNext);
            
        return Result.Success(new Response<CursorPagedResult<ConsultationResponseDto>>(
            ConsultationMessage.GetConsultationHistories.GetMessage().Code,
            ConsultationMessage.GetConsultationHistories.GetMessage().Message,
            result));
    }

    private static FilterDefinition<Consultation> BuildFilter(GetConsultationHistoriesQuery query)
    {
        var builder = Builders<Consultation>.Filter;
        var filters = new List<FilterDefinition<Consultation>>
        {
            builder.Eq(c => c.IsDeleted, false),
        };

        switch (query.Role)
        {
            case nameof(Role.Doctor):
                var doctorId = UserId.Of(query.UserId);
                filters.AddRange([
                    builder.Eq(c => c.DoctorId, doctorId)]);
                break;
            
            case nameof(Role.Patient):
                var patientId = UserId.Of(query.UserId);
                filters.AddRange([
                builder.Eq(c => c.PatientId, patientId)]);
                break;
        }
        
        if (query.Filter.Status is not null)
        {
            switch (query.Filter.Status.Value)
            {
                case ConsultationStatusEnum.Upcoming:
                    filters.Add(builder.In(c => c.Status, [
                        ConsultationStatus.Booked,
                        ConsultationStatus.OnProcessing
                    ]));
                    break;
                case ConsultationStatusEnum.Cancelled:
                    filters.Add(builder.In(c => c.Status, [
                        ConsultationStatus.Declined,
                        ConsultationStatus.Cancelled
                    ]));
                    break;
                case ConsultationStatusEnum.Completed:
                    filters.Add(builder.In(c => c.Status, [
                        ConsultationStatus.Done
                    ]));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        if (!string.IsNullOrWhiteSpace(query.Pagination.Cursor) && ObjectId.TryParse(query.Pagination.Cursor, out var cursor))
        {
            filters.Add(builder.Lt(c => c.Id, cursor));
        }
        
        return builder.And(filters);
    }
    
    private static SortDefinition<Consultation> BuildConsultationSort()
    {
        var builder = Builders<Consultation>.Sort;
        var sorters = new List<SortDefinition<Consultation>>
        {
            builder.Descending(c => c.Id)
        };
        
        return builder.Combine(sorters);
    }

    private static PipelineDefinition<User, BsonDocument> UserLookup()
    {
        return PipelineDefinition<User, BsonDocument>.Create(
            new BsonDocument("$match", new BsonDocument
            {
                { "$expr", new BsonDocument("$eq", new BsonArray { "$user_id", "$$userId" }) }
            }),
            new BsonDocument("$limit", 1));
    }

    private static BsonDocument BuildConsultationProjection(bool isDoctor)
    {
        var project = new BsonDocument
        {
            { "_id", 1 },
            { "date", new BsonDocument
                {
                    { "$dateToString", new BsonDocument
                        {
                            { "format", "%Y-%m-%d" },
                            { "date", "$start_time" }
                        }
                    }
                }
            },
            { "start_time", new BsonDocument
            {
                { "$dateToString", new BsonDocument
                    {
                        { "format", "%H:%M:%S" },
                        { "date", "$start_time" }
                    }
                }
            } 
            },
            { "end_time", new BsonDocument
            {
                { "$dateToString", new BsonDocument
                    {
                        { "format", "%H:%M:%S" },
                        { "date", "$end_time" }
                    }
                }
            } },
            { "status", 1 },
            { "user_full_name", "$user.display_name" }, 
            { "user_avatar", "$user.avatar.public_url" },
        };

        if (isDoctor)
        {
            project.Add("price",
                new BsonDocument
                {
                    {
                        "$cond", new BsonArray
                        {
                            new BsonDocument { { "$eq", new BsonArray { "$status", ConsultationStatus.Done } } }, 
                            "$user_package.price",
                            BsonNull.Value
                        }
                    }
                }
            );
        }
        else
        {
            project.Add("conversation_id", "$conversation_id._id");
        }

        return project;
    }
    
}
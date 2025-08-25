using ConsultationService.Contract.Abstractions.Shared;
using ConsultationService.Contract.Common.DomainErrors;
using ConsultationService.Contract.Helpers;
using ConsultationService.Domain.Abstractions;
using ConsultationService.Domain.Enums;
using ConsultationService.Domain.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ConsultationService.Domain.Models;

[BsonIgnoreExtraElements]
public class ConsultationTemplate : DomainEntity<ObjectId>
{
    [BsonElement("doctor_id")]
    public UserId DoctorId { get; private set; }
    
    [BsonElement("date")]
    public DateTime Date { get; private set; }
    
    [BsonElement("start_time")]
    public string StartTime { get; private set; }
    
    [BsonElement("start_time_minutes")]
    public double StartTimeInMinutes { get; private set; }
    
    [BsonElement("end_time")]
    public string EndTime { get; private set; }
    
    [BsonElement("end_time_minutes")]
    public double EndTimeInMinutes { get; private set; }
    
    [BsonElement("status")]
    public ConsultationTemplateStatus Status { get; private set; }
    
    private ConsultationTemplate() { }

    private ConsultationTemplate(ObjectId id, UserId doctorId, DateOnly date, string startTime, double startTimeInMinutes, string endTime, double endTimeInMinutes, ConsultationTemplateStatus status)
    {
        Id = id;
        DoctorId = doctorId;
        Date = DateTime.SpecifyKind(date.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
        StartTime = startTime;
        StartTimeInMinutes = startTimeInMinutes;
        EndTime = endTime;
        EndTimeInMinutes = endTimeInMinutes;
        Status = status;
        CreatedDate = CurrentTimeService.GetCurrentTimeUtc();
        ModifiedDate = CurrentTimeService.GetCurrentTimeUtc();
        IsDeleted = false;
    }

    public static Result<ConsultationTemplate> Create(ObjectId id, UserId doctorId, DateOnly date, TimeOnly startTime, TimeOnly endTime)
    {
        var errors = new List<Error>();
        if ((endTime - startTime).TotalMinutes < 15)
        {
            errors.Add(ConsultationTemplateErrors.MinimumDuration);
        }
        
        var template = new ConsultationTemplate(
            id: id,
            doctorId: doctorId,
            date: date,
            startTime: startTime.ToString("HH:mm"),
            startTime.ToTimeSpan().TotalMinutes,
            endTime: endTime.ToString("HH:mm"),
            endTime.ToTimeSpan().TotalMinutes,
            status: ConsultationTemplateStatus.Unavailable);
        
        return errors.Count > 0 ? ValidationResult<ConsultationTemplate>.WithErrors(errors.AsEnumerable().ToArray()) : Result.Success(template);
    }

    public Result Modify(TimeOnly? newStartTime, TimeOnly? newEndTime)
    {
        var errors = new List<Error>();
        var startTime = newStartTime?.ToTimeSpan() ?? TimeSpan.FromMinutes(StartTimeInMinutes);
        var endTime = newEndTime?.ToTimeSpan() ?? TimeSpan.FromMinutes(EndTimeInMinutes);

        if (endTime <= startTime)
        {
            errors.Add(ConsultationTemplateErrors.StartTimeAfterEndTime);
        }
        
        if ((endTime - startTime).TotalMinutes < 15)
        {
            errors.Add(ConsultationTemplateErrors.MinimumDuration);
        }
        
        if (errors.Count != 0)
            return ValidationResult.WithErrors(errors.AsEnumerable().ToArray());

        if (newStartTime is not null)
        {
            StartTime = startTime.ToString(@"hh\:mm");
            StartTimeInMinutes = startTime.TotalMinutes;
        }
        
        if (newEndTime is not null)
        {
            EndTime = endTime.ToString(@"hh\:mm");
            EndTimeInMinutes = endTime.TotalMinutes;
        }
        
        ModifiedDate = CurrentTimeService.GetCurrentTimeUtc();
        
        return Result.Success();
    }

    public void Available()
    {
        Status = ConsultationTemplateStatus.Available;
        ModifiedDate = CurrentTimeService.GetCurrentTimeUtc();
    }
    
    public void Unavailable()
    {
        Status = ConsultationTemplateStatus.Unavailable;
        ModifiedDate = CurrentTimeService.GetCurrentTimeUtc();
    }
    
    public void Booked()
    {
        Status = ConsultationTemplateStatus.Booked;
        ModifiedDate = CurrentTimeService.GetCurrentTimeUtc();
    }
    
}


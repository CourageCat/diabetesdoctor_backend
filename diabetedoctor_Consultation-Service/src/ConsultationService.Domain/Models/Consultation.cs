using ConsultationService.Contract.Abstractions.Shared;
using ConsultationService.Contract.Common.DomainErrors;
using ConsultationService.Contract.Helpers;
using ConsultationService.Domain.Abstractions;
using ConsultationService.Domain.Enums;
using ConsultationService.Domain.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ConsultationService.Domain.Models;

public class Consultation : DomainEntity<ObjectId>
{
    [BsonElement("start_time")]
    public DateTime StartTime { get; private set; }
    
    [BsonElement("end_time")]
    public DateTime EndTime { get; private set; }
    
    [BsonElement("status")]
    public ConsultationStatus Status { get; private set; }
    
    [BsonElement("doctor_id")]
    public UserId DoctorId { get; private set; }
    
    [BsonElement("patient_id")]
    public UserId PatientId { get; private set; }
    
    [BsonElement("consultation_template_id")]
    public ObjectId ConsultationTemplateId { get; private set; }
    
    [BsonElement("conversation_id")]
    public ConversationId? ConversationId { get; private set; }
    
    [BsonElement("user_package")]
    public UserPackage? UserPackage { get; private set; }
    
    [BsonElement("rating")]
    public double? Rating { get; private set; }
    
    [BsonElement("reason")]
    public string? Reason {get; private set; }
    
    private Consultation(){}

    private Consultation(ObjectId id, DateTime startTime, DateTime endTime, UserId doctorId, UserId patientId, ObjectId templateId, ConsultationStatus status, UserPackage userPackage)
    {
        Id = id;
        StartTime = startTime;
        EndTime = endTime;
        DoctorId = doctorId;
        PatientId = patientId;
        Status = status;
        ConsultationTemplateId = templateId;
        UserPackage = userPackage;
        CreatedDate = CurrentTimeService.GetCurrentTimeUtc();
        ModifiedDate = CurrentTimeService.GetCurrentTimeUtc();
        IsDeleted = false;
    }

    public static Result<Consultation> Create(ObjectId id, DateTime date, TimeSpan startTime, TimeSpan endTime, UserId doctorId, UserId patientId, ObjectId templateId, bool isOpened, UserPackage userPackage)
    {
        var errors = new List<Error>();
        var startDate = date + startTime; 
        var endDate = date + endTime;
        if (CurrentTimeService.GetVietNamCurrentTime() > startDate)
        {
            errors.Add(ConsultationErrors.BookingTimeExpired);
        }

        if (errors.Count > 0)
        {
            return ValidationResult<Consultation>.WithErrors(errors.AsEnumerable().ToArray());
        }
        
        var consultation = new Consultation(
            id: id,
            startTime: startDate,
            endTime: endDate,
            doctorId: doctorId,
            patientId: patientId,
            templateId: templateId,
            status: isOpened ? ConsultationStatus.OnProcessing: ConsultationStatus.Booked, 
            userPackage: userPackage);
        return Result.Success(consultation);
    }

    public void LinkConversation(ConversationId conversationId)
    {
        ConversationId = conversationId;
    }

    public void Rate(double rating)
    {
        Rating = rating;
    }
    
    public Result Cancel(string? reason = null)
    {
        var now = CurrentTimeService.GetVietNamCurrentTime();
        if (StartTime - now <= TimeSpan.FromMinutes(30))
        {
            return Result.Failure(ConsultationErrors.BookingTimeExpired);
        }
        Reason = reason;
        Status = ConsultationStatus.Cancelled;
        UserPackage = null;
        ModifiedDate = CurrentTimeService.GetCurrentTimeUtc();
        return Result.Success();
    }
    
    public Result Decline(string? reason = null)
    {
        var now = CurrentTimeService.GetVietNamCurrentTime();
        if (StartTime - now <= TimeSpan.FromMinutes(30))
        {
            return Result.Failure(ConsultationErrors.BookingTimeExpired);
        }
        Reason = reason;
        Status = ConsultationStatus.Declined;
        UserPackage = null;
        ModifiedDate = CurrentTimeService.GetCurrentTimeUtc();
        return Result.Success();
    }
    
    public void Start()
    {
        Status = ConsultationStatus.OnProcessing;
        ModifiedDate = CurrentTimeService.GetCurrentTimeUtc();
    }
    
    public void Done()
    {
        Status = ConsultationStatus.Done;
        ModifiedDate = CurrentTimeService.GetCurrentTimeUtc();
    }
    
    
}
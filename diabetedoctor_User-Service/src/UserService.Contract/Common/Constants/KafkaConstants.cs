namespace UserService.Contract.Common.Constants;

public static class KafkaConstants
{
    public const string PatientTopic = "patient_topic";
    public const string UserServicePatientConsumerGroup = "user_service-user_group";

    public const string HospitalTopic = "hospital_topic";
    public const string UserServiceHospitalConsumerGroup = "user_service-hospital_group";
}
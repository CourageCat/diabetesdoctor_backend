using UserService.Contract.Common.Messages;

namespace UserService.Contract.Common.DomainErrors;

public static class CarePlanTemplateErrors
{
    public static readonly Error DuplicatedCarePlanTemplate = Error.Conflict(CarePlanTemplateMessages.DuplicatedCarePlanTemplate.GetMessage().Code,
        CarePlanTemplateMessages.DuplicatedCarePlanTemplate.GetMessage().Message);
    public static readonly Error CarePlanTemplateNotExist = Error.NotFound(CarePlanTemplateMessages.CarePlanTemplateNotExist.GetMessage().Code,
        CarePlanTemplateMessages.CarePlanTemplateNotExist.GetMessage().Message);
    public static readonly Error CarePlanTemplateNotBelongToPatient = Error.Conflict(CarePlanTemplateMessages.CarePlanTemplateNotBelongToPatient.GetMessage().Code,
        CarePlanTemplateMessages.CarePlanTemplateNotBelongToPatient.GetMessage().Message);
    public static readonly Error CarePlanTemplateNotBelongToDoctor = Error.Conflict(CarePlanTemplateMessages.CarePlanTemplateNotBelongToDoctor.GetMessage().Code,
        CarePlanTemplateMessages.CarePlanTemplateNotBelongToDoctor.GetMessage().Message);
    public static readonly Error CanNotUpdateCarePlanTemplateBelongToDoctor = Error.Conflict(CarePlanTemplateMessages.CanNotUpdateCarePlanTemplateBelongToDoctor.GetMessage().Code,
        CarePlanTemplateMessages.CanNotUpdateCarePlanTemplateBelongToDoctor.GetMessage().Message);
    public static readonly Error CanNotDeleteCarePlanTemplateBelongToDoctor = Error.Conflict(CarePlanTemplateMessages.CanNotDeleteCarePlanTemplateBelongToDoctor.GetMessage().Code,
        CarePlanTemplateMessages.CanNotDeleteCarePlanTemplateBelongToDoctor.GetMessage().Message);
    public static readonly Error CanNotUpdateCarePlanTemplateBelongToPatient = Error.Conflict(CarePlanTemplateMessages.CanNotUpdateCarePlanTemplateBelongToPatient.GetMessage().Code,
        CarePlanTemplateMessages.CanNotUpdateCarePlanTemplateBelongToPatient.GetMessage().Message);
    public static readonly Error CanNotDeleteCarePlanTemplateBelongToPatient = Error.Conflict(CarePlanTemplateMessages.CanNotDeleteCarePlanTemplateBelongToPatient.GetMessage().Code,
        CarePlanTemplateMessages.CanNotDeleteCarePlanTemplateBelongToPatient.GetMessage().Message);
}
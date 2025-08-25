using UserService.Contract.Common.Messages;

namespace UserService.Contract.Common.DomainErrors;

public static class CarePlanInstanceErrors
{
    public static readonly Error DuplicatedCarePlanInstance = Error.Conflict(CarePlanInstanceMessages.DuplicatedCarePlanInstance.GetMessage().Code,
        CarePlanInstanceMessages.DuplicatedCarePlanInstance.GetMessage().Message);
    public static readonly Error CarePlanInstanceNotExist = Error.NotFound(CarePlanInstanceMessages.CarePlanInstanceNotExist.GetMessage().Code,
        CarePlanInstanceMessages.CarePlanInstanceNotExist.GetMessage().Message);
    public static readonly Error CarePlanInstanceNotBelongToPatient = Error.NotFound(CarePlanInstanceMessages.CarePlanInstanceNotBelongToPatient.GetMessage().Code,
        CarePlanInstanceMessages.CarePlanInstanceNotBelongToPatient.GetMessage().Message);
    public static readonly Error CarePlanInstanceNotBelongToDoctor = Error.NotFound(CarePlanInstanceMessages.CarePlanInstanceNotBelongToDoctor.GetMessage().Code,
        CarePlanInstanceMessages.CarePlanInstanceNotBelongToDoctor.GetMessage().Message);
    public static readonly Error CanNotUpdateCarePlanInstanceBelongToDoctor = Error.Conflict(CarePlanInstanceMessages.CanNotUpdateCarePlanInstanceBelongToDoctor.GetMessage().Code,
        CarePlanInstanceMessages.CanNotUpdateCarePlanInstanceBelongToDoctor.GetMessage().Message);
    public static readonly Error CanNotDeleteCarePlanInstanceBelongToDoctor = Error.Conflict(CarePlanInstanceMessages.CanNotDeleteCarePlanInstanceBelongToDoctor.GetMessage().Code,
        CarePlanInstanceMessages.CanNotDeleteCarePlanInstanceBelongToDoctor.GetMessage().Message);
    public static readonly Error CanNotUpdateCarePlanInstanceBelongToPatient = Error.Conflict(CarePlanInstanceMessages.CanNotUpdateCarePlanInstanceBelongToPatient.GetMessage().Code,
        CarePlanInstanceMessages.CanNotUpdateCarePlanInstanceBelongToPatient.GetMessage().Message);
    public static readonly Error CanNotDeleteCarePlanInstanceBelongToPatient = Error.Conflict(CarePlanInstanceMessages.CanNotDeleteCarePlanInstanceBelongToPatient.GetMessage().Code,
        CarePlanInstanceMessages.CanNotDeleteCarePlanInstanceBelongToPatient.GetMessage().Message);
}
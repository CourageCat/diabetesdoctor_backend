using MediaService.Contract.Common.Messages;
using MediaService.Contract.Exceptions;
using MediaService.Contract.Helpers;

namespace MediaService.Contract.Exceptions.BussinessExceptions;

public static class HospitalException
{
    public sealed class HospitalNameExistException : BadRequestException
    {
        public HospitalNameExistException()
                : base(HospitalMessage.HospitalNameExistException.GetMessage().Message,
                    HospitalMessage.HospitalNameExistException.GetMessage().Code)
        { }
    }
}

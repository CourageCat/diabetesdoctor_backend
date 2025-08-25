using MediaService.Contract.Common.Messages;

namespace MediaService.Contract.Exceptions.BussinessExceptions;
public static class PostException
{
    public sealed class PostNotFoundException : NotFoundException
    {
        public PostNotFoundException() : base(PostMessage.PostNotFoundException.GetMessage().Message, PostMessage.PostNotFoundException.GetMessage().Code) 
        { }
    }
}

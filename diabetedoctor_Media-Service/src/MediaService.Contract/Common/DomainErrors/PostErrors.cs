using MediaService.Contract.Common.Messages;

namespace MediaService.Contract.Common.DomainErrors;

public static class PostErrors
{
    public static readonly Error PostNotFound = Error.NotFound(PostMessage.PostNotFoundException.GetMessage().Code,
        PostMessage.PostNotFoundException.GetMessage().Message);
    public static readonly Error PostHasAlreadyBeenPublishedException = Error.Conflict(PostMessage.PostHasAlreadyBeenPublishedException.GetMessage().Code,
        PostMessage.PostHasAlreadyBeenPublishedException.GetMessage().Message);
    public static readonly Error PostHasAlreadyBeenReviewedException = Error.Conflict(PostMessage.PostHasAlreadyBeenReviewedException.GetMessage().Code,
        PostMessage.PostHasAlreadyBeenReviewedException.GetMessage().Message);
    public static readonly Error PostIsDraftedException = Error.Conflict(PostMessage.PostIsDraftedException.GetMessage().Code,
        PostMessage.PostIsDraftedException.GetMessage().Message);
    public static readonly Error PostIsNotBelongToStaffException = Error.Conflict(PostMessage.PostIsNotBelongToStaffException.GetMessage().Code,
        PostMessage.PostIsNotBelongToStaffException.GetMessage().Message);
}
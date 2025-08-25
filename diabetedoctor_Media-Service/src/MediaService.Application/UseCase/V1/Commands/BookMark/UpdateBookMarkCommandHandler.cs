using MediaService.Contract.Common.DomainErrors;
using MediaService.Contract.Services.BookMark;
using MediaService.Domain.Abstractions;
using MediaService.Domain.Abstractions.Repositories;
using MediaService.Domain.ValueObjects;
using MongoDB.Bson;

namespace MediaService.Application.UseCase.V1.Commands.BookMark;
public sealed class UpdateBookMarkCommandHandler : ICommandHandler<UpdateBookMarkCommand, Success>
{
    private readonly IBookMarkRepository _bookMarkRepository;
    private readonly IPostRepository _postRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBookMarkCommandHandler(IBookMarkRepository bookMarkRepository, IPostRepository postRepository, IUnitOfWork unitOfWork)
    {
        _bookMarkRepository = bookMarkRepository;
        _postRepository = postRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Success>> Handle(UpdateBookMarkCommand request, CancellationToken cancellationToken)
    {
        var postFound = await _postRepository.ExistAsync(x => x.Id == request.PostId, cancellationToken);
        if (!postFound)
        {
            return Result.Failure<Success>(PostErrors.PostNotFound);
        }
        var bookmarkedPostFound = await _bookMarkRepository.FindSingleAsync(x => x.PostId == request.PostId && x.UserId == UserId.Of(request.UserId), cancellationToken);
        try
        {
            await _unitOfWork.StartTransactionAsync(cancellationToken);
            // User has not bookmarked post => Bookmarked post
            if (bookmarkedPostFound is null)
            {
                var bookMarkAddedId = ObjectId.GenerateNewId();
                var bookMarkAdded = Domain.Models.BookMark.Create(bookMarkAddedId, request.PostId, UserId.Of(request.UserId));
                await _bookMarkRepository.CreateAsync(_unitOfWork.ClientSession, bookMarkAdded, cancellationToken);
            }
            // User has already bookmarked post => UnBookmarked post
            else
            {
                await _bookMarkRepository.DeleteAsync(_unitOfWork.ClientSession, bookmarkedPostFound.Id, cancellationToken);
            }
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await _unitOfWork.AbortTransactionAsync(cancellationToken);
            throw;
        }
        return Result.Success(bookmarkedPostFound == null ? new Success(BookMarkMessage.BookmarkedSuccessfully.GetMessage().Code, BookMarkMessage.BookmarkedSuccessfully.GetMessage().Message) : new Success(BookMarkMessage.UnBookmarkedSuccessfully.GetMessage().Code, BookMarkMessage.UnBookmarkedSuccessfully.GetMessage().Message));
    }
}

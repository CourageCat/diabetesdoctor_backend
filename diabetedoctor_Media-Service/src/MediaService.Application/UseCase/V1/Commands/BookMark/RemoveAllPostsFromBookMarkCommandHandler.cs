using MediaService.Contract.Services.BookMark;
using MediaService.Domain.Abstractions;
using MediaService.Domain.Abstractions.Repositories;
using MediaService.Domain.ValueObjects;

namespace MediaService.Application.UseCase.V1.Commands.BookMark;
public sealed class RemoveAllPostsFromBookMarkCommandHandler : ICommandHandler<RemoveAllPostsFromBookMarkCommand, Success>
{
    private readonly IBookMarkRepository _bookMarkRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveAllPostsFromBookMarkCommandHandler(IBookMarkRepository bookMarkRepository, IUnitOfWork unitOfWork)
    {
        _bookMarkRepository = bookMarkRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Success>> Handle(RemoveAllPostsFromBookMarkCommand request, CancellationToken cancellationToken)
    {
        var postsFromBookMark = await _bookMarkRepository.FindListAsync(x => x.UserId == UserId.Of(request.UserId), cancellationToken);
        var ids = postsFromBookMark.Select(x => x.Id).ToList();
        await _unitOfWork.StartTransactionAsync(cancellationToken);
        try
        {
            await _bookMarkRepository.DeleteManyAsync(_unitOfWork.ClientSession, ids, cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        } 
        catch (Exception ex)
        {
            await _unitOfWork.AbortTransactionAsync(cancellationToken);
            throw;
        }
        return Result.Success(new Success(BookMarkMessage.RemoveAllPostsFromBookMarkSuccessfully.GetMessage().Code, BookMarkMessage.RemoveAllPostsFromBookMarkSuccessfully.GetMessage().Message));
    }
}

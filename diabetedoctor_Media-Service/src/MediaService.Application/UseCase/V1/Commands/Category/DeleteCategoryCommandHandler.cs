// using MediaService.Contract.Common.DomainErrors;
// using MediaService.Contract.Infrastructure.Services;
// using MediaService.Contract.Services.Category;
// using MediaService.Domain.Abstractions;
// using MediaService.Domain.Abstractions.Repositories;
// using MongoDB.Bson;
//
// namespace MediaService.Application.UseCase.V1.Commands.Category;
//
// public sealed class DeleteCategoryCommandHandler : ICommandHandler<DeleteCategoryCommand, Success>
// {
//     private readonly ICategoryRepository _categoryRepository;
//     private readonly IMediaService _mediaService;
//     private readonly IUnitOfWork _unitOfWork;
//
//     public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository, IMediaService mediaService, IUnitOfWork unitOfWork)
//     {
//         _categoryRepository = categoryRepository;
//         _mediaService = mediaService;
//         _unitOfWork = unitOfWork;
//     }
//
//     public async Task<Result<Success>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
//     {
//         var categoryFound = await _categoryRepository.FindSingleAsync(x => x.Id == request.Id, cancellationToken);
//         if(categoryFound is null)
//         {
//             return Result.Failure<Success>(CategoryErrors.CategoryNotFoundException);
//         }
//         await _mediaService.DeleteFileAsync(categoryFound.Image.PublicId);
//         await _unitOfWork.StartTransactionAsync(cancellationToken);
//         try
//         {
//             await _categoryRepository.DeleteAsync(_unitOfWork.ClientSession, request.Id, cancellationToken);
//             await _unitOfWork.CommitTransactionAsync(cancellationToken);
//         }
//         catch (Exception ex)
//         {
//             await _unitOfWork.AbortTransactionAsync(cancellationToken);
//             throw;
//         }
//         return Result.Success(new Success(CategoryMessage.DeleteCategorySuccessfully.GetMessage().Code, CategoryMessage.DeleteCategorySuccessfully.GetMessage().Message));
//     }
// }

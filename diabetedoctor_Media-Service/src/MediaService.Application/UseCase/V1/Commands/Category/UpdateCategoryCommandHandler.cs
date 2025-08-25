// using MediaService.Contract.Common.DomainErrors;
// using MediaService.Contract.Infrastructure.Services;
// using MediaService.Contract.Services.Category;
// using MediaService.Domain.Abstractions;
// using MediaService.Domain.Abstractions.Repositories;
// using MongoDB.Bson;
//
// namespace MediaService.Application.UseCase.V1.Commands.Category;
//
// public sealed class UpdateCategoryCommandHandler : ICommandHandler<UpdateCategoryCommand, Success>
// {
//     private readonly ICategoryRepository _categoryRepository;
//     private readonly IMediaService _mediaService;
//     private readonly IUnitOfWork _unitOfWork;
//
//     public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, IMediaService mediaService, IUnitOfWork unitOfWork)
//     {
//         _categoryRepository = categoryRepository;
//         _mediaService = mediaService;
//         _unitOfWork = unitOfWork;
//     }
//
//     public async Task<Result<Success>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
//     {
//         var categoryFound = await _categoryRepository.FindSingleAsync(x => x.Id == request.Id, cancellationToken);
//         if (categoryFound is null)
//         {
//             return Result.Failure<Success>(CategoryErrors.CategoryNotFoundException);
//         }
//         var imageUpdated = categoryFound.Image;
//         // Check if request has image, then remove the old image in Media Service first, then upload the new image and assign new value to imageUpdated
//         if (request.Image != null)
//         {
//             await _mediaService.DeleteFileAsync(categoryFound.Image.PublicId);
//             var imageUploaded = await _mediaService.UploadImageAsync(request.Image.Name, request.Image);
//             var imageConverted = Domain.ValueObjects.Image.Of(imageUploaded.PublicImageId, imageUploaded.ImageUrl);
//             imageUpdated = imageConverted;
//         }
//         // Check if request does not include properties information, then use the old properties information
//         var nameUpdated = request.Name ?? categoryFound.Name;
//         var descriptionUpdated = request.Description ?? categoryFound.Description;
//         categoryFound.Update(nameUpdated, descriptionUpdated, imageUpdated);
//         await _unitOfWork.StartTransactionAsync(cancellationToken);
//         try
//         {
//             await _categoryRepository.UpdateAsync(_unitOfWork.ClientSession, request.Id, categoryFound, cancellationToken);
//             await _unitOfWork.CommitTransactionAsync(cancellationToken);
//             return Result.Success(new Success(CategoryMessage.UpdateCategorySuccessfully.GetMessage().Code, CategoryMessage.UpdateCategorySuccessfully.GetMessage().Message));
//         }
//         catch (Exception ex)
//         {
//             await _unitOfWork.AbortTransactionAsync(cancellationToken);
//             throw;
//         }
//     }
// }

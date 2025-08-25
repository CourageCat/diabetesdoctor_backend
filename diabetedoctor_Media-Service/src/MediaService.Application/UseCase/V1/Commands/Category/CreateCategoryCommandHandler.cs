using MediaService.Contract.Infrastructure.Services;
using MediaService.Contract.Services.Category;
using MediaService.Domain.Abstractions;
using MediaService.Domain.Abstractions.Repositories;
using MediaService.Domain.ValueObjects;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;
using MediaService.Contract.Common.DomainErrors;
using MediaService.Domain.Enums;

namespace MediaService.Application.UseCase.V1.Commands.Category;

public sealed class CreateCategoryCommandHandler : ICommandHandler<CreateCategoryCommand, Success>
{
    private readonly IRepositoryBase<Domain.Models.Category> _categoryRepository;
    private readonly IMediaRepository _mediaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(IRepositoryBase<Domain.Models.Category> categoryRepository, IMediaRepository mediaRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _mediaRepository = mediaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Success>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        // Checking if duplicated category name in request
        var existedCategoryByName = await _categoryRepository
            .ExistAsync(c => Regex.IsMatch(c.Name, $"^{Regex.Escape(request.Name)}$", RegexOptions.IgnoreCase), cancellationToken);
        if (existedCategoryByName)
        {
            return Result.Failure<Success>(CategoryErrors.CategoryNameExistException);
        }
        await _unitOfWork.StartTransactionAsync(cancellationToken);
        try
        {
            var imageFound = await _mediaRepository.FindSingleAsync(image => image.Id == ObjectId.Parse(request.Image), cancellationToken);
            if (imageFound is null)
            {
                return Result.Failure<Success>(MediaErrors.ImageNotFoundException);
            }
            var categoryAddedId = ObjectId.GenerateNewId();
            var categoryAdded = Domain.Models.Category.Create(categoryAddedId, request.Name, request.Description, Image.Of(imageFound.PublicId, imageFound.PublicUrl));
            var updateImageDefinition = Builders<Domain.Models.Media>.Update
                .Set(image => image.ExpiredAt, null)
                .Set(image => image.IsUsed, true)
                .Set(image => image.ModifiedDate, DateTime.UtcNow);
            await _categoryRepository.CreateAsync(_unitOfWork.ClientSession, categoryAdded, cancellationToken);
            await _mediaRepository.UpdateOneAsync(_unitOfWork.ClientSession, imageFound.Id, updateImageDefinition, cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await _unitOfWork.AbortTransactionAsync(cancellationToken);
            throw;
        }
        return Result.Success(new Success(CategoryMessage.CreateCategorySuccessfully.GetMessage().Code, CategoryMessage.CreateCategorySuccessfully.GetMessage().Message));
    }
}

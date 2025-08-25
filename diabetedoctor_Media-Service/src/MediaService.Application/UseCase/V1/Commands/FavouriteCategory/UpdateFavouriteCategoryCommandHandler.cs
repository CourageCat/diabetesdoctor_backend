using MediaService.Contract.Services.FavouriteCategory;
using MediaService.Domain.Abstractions;
using MediaService.Domain.Abstractions.Repositories;
using MediaService.Domain.ValueObjects;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq;
using System.Xml;
using MediaService.Contract.Common.DomainErrors;

namespace MediaService.Application.UseCase.V1.Commands.FavouriteCategory;
public sealed class UpdateFavouriteCategoryCommandHandler : ICommandHandler<UpdateFavouriteCategoryCommand, Success>
{
    private readonly IFavouriteCategoryRepository _favouriteCategoryRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateFavouriteCategoryCommandHandler(IFavouriteCategoryRepository favouriteCategoryRepository, ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _favouriteCategoryRepository = favouriteCategoryRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Success>> Handle(UpdateFavouriteCategoryCommand request, CancellationToken cancellationToken)
    {
        // old ===============================================================================================

        // Find the correct list Categories (Remove the case Request contains incorrect CategoryId)
        var existedCategories = (await _categoryRepository.FindListAsync(x => request.CategoryIds.Contains(x.Id), cancellationToken)).ToList();
        // Check if there are existed Category to Update then update, if not then do nothing
        if (!existedCategories.Any())
        {
            return Result.Failure<Success>(CategoryErrors.CategoriesNotFoundException);
        }
        var ids = existedCategories.Select(x => x.Id).ToList();
        // Find the duplicated Id between CategoryId in Request and CategoryId in Favourite Category
        var duplicatedCategory = (await _favouriteCategoryRepository.FindListAsync(x => ids.Contains(x.CategoryId) && x.UserId == UserId.Of(request.UserId), cancellationToken)).ToHashSet();
        var duplicatedIds = duplicatedCategory.Select(x => x.CategoryId).ToList();
        
        // new - Khang ===============================================================================================
        // only handle cate exist in system
        var existed = (await _categoryRepository.FindListAsync(x => request.CategoryIds.Contains(x.Id), cancellationToken)).ToList();
        if (existed.Count == 0)
        {
            return Result.Failure<Success>(CategoryErrors.CategoriesNotFoundException);
        }
        var existedIds = existed.Select(x => x.Id).ToHashSet();
        var userId = UserId.Of(request.UserId);
        var favoriteCate = (await _favouriteCategoryRepository.FindListAsync(x => x.UserId == userId, cancellationToken)).Select(x => x.CategoryId)
            .ToHashSet();
        
        // handle delete old cate not in request
        // xử lí "xóa" những cate cũ ko có trong request
        var deleteCate = favoriteCate.Where(cate => !existedIds.Contains(cate)).ToList();
        // xử lí "thêm" những cate chưa có trong cate cũ
        var addCate = existedIds.Where(cate => !favoriteCate.Contains(cate)).ToList();
        
        await _unitOfWork.StartTransactionAsync();
        try
        {
            // new - Khang ===============================================================================================
            if (deleteCate.Count != 0)
            {
                // xóa
                await _favouriteCategoryRepository.DeleteManyAsync(_unitOfWork.ClientSession, deleteCate, cancellationToken);
            }
            
            if (addCate.Count != 0)
            {
                // thêm
                var addList = addCate.Select(x =>
                {
                    var id = ObjectId.GenerateNewId();
                    return Domain.Models.FavouriteCategory.Create(id, x, userId);
                });
                await _favouriteCategoryRepository.CreateManyAsync(_unitOfWork.ClientSession, addList, cancellationToken);
            }
            
            // old ===============================================================================================
            await RemoveOldFavouriteCategoriesOfUserAsync(duplicatedIds, request.UserId, cancellationToken);
            await AddNewFavouriteCategoriesOfUserAsync(duplicatedIds, ids, request.UserId, cancellationToken);
            await _unitOfWork.CommitTransactionAsync();

        }
        catch (Exception ex)
        {
            await _unitOfWork.AbortTransactionAsync();
            throw;
        }
        return Result.Success(new Success(FavouriteCategoryMessage.UpdateFavouriteCategorySuccessfully.GetMessage().Code, FavouriteCategoryMessage.UpdateFavouriteCategorySuccessfully.GetMessage().Message));
    }

    private async Task RemoveOldFavouriteCategoriesOfUserAsync(List<ObjectId> duplicatedIds, string userId, CancellationToken cancellationToken)
    {
        var categoriesInFavourite = await _favouriteCategoryRepository.FindListAsync(x => x.UserId == UserId.Of(userId), cancellationToken);
        var categoriesInFavouriteIds = categoriesInFavourite.Select(x => x.CategoryId).ToList();
        var categoryIdsToRemove = categoriesInFavouriteIds.Where(x => !duplicatedIds.Contains(x)).ToList();
        if (categoryIdsToRemove.Count == 0)
        {
            return;
        }
        
        // bị dư
        var categoriesToRemove = await _favouriteCategoryRepository.FindListAsync(x => categoryIdsToRemove.Contains(x.CategoryId) && x.UserId == UserId.Of(userId));
        var idsToRemove = categoriesToRemove.Select(x => x.Id).ToList();
        await _favouriteCategoryRepository.DeleteManyAsync(_unitOfWork.ClientSession, idsToRemove, cancellationToken);
    }
    private async Task AddNewFavouriteCategoriesOfUserAsync(List<ObjectId> duplicatedIds, List<ObjectId> categoryInRequestIds, string userId, CancellationToken cancellationToken)
    {
        var idsToUpdate = categoryInRequestIds.Where(x => !duplicatedIds.Contains(x)).ToList();
        if (idsToUpdate.Count == 0)
        {
            return;
        }
        var favouriteCategoriesAdded = new List<Domain.Models.FavouriteCategory>();
        idsToUpdate.ForEach(categoryId =>
        {
            var favouriteCategoryAddedId = ObjectId.GenerateNewId();
            favouriteCategoriesAdded.Add(Domain.Models.FavouriteCategory.Create(favouriteCategoryAddedId, categoryId, UserId.Of(userId)));
        });
        await _favouriteCategoryRepository.CreateManyAsync(_unitOfWork.ClientSession, favouriteCategoriesAdded, cancellationToken);
    }
}

using MediaService.Contract.Common.Messages;

namespace MediaService.Contract.Common.DomainErrors;

public static class CategoryErrors
{
    public static readonly Error CategoryNameExistException = Error.Conflict(CategoryMessage.CategoryNameExistException.GetMessage().Code,
        CategoryMessage.CategoryNameExistException.GetMessage().Message);
    public static readonly Error CategoryNotFoundException = Error.NotFound(CategoryMessage.CategoryNotFoundException.GetMessage().Code,
        CategoryMessage.CategoryNotFoundException.GetMessage().Message);
    public static readonly Error CategoriesNotFoundException = Error.NotFound(CategoryMessage.CategoriesNotFoundException.GetMessage().Code,
        CategoryMessage.CategoriesNotFoundException.GetMessage().Message);
}
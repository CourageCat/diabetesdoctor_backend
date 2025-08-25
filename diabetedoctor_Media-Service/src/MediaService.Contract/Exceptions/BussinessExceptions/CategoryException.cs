using MediaService.Contract.Common.Messages;
using MediaService.Contract.Exceptions;
using MediaService.Contract.Helpers;

namespace MediaService.Contract.Exceptions.BussinessExceptions;

public static class CategoryException
{
    public sealed class CategoryNameExistException : BadRequestException
    {
        public CategoryNameExistException()
                : base(CategoryMessage.CategoryNameExistException.GetMessage().Message,
                    CategoryMessage.CategoryNameExistException.GetMessage().Code)
        { }
    }

    public sealed class CategoryNotFoundException : NotFoundException
    {
        public CategoryNotFoundException()
                : base(CategoryMessage.CategoryNotFoundException.GetMessage().Message,
                    CategoryMessage.CategoryNotFoundException.GetMessage().Code)
        { }
    }

    public sealed class CategoriesNotFoundException : NotFoundException
    {
        public CategoriesNotFoundException()
                : base(CategoryMessage.CategoriesNotFoundException.GetMessage().Message,
                    CategoryMessage.CategoriesNotFoundException.GetMessage().Code)
        { }
    }
}

using MediaService.Domain.Abstractions;
using MediaService.Domain.Abstractions.Repositories;
using MediaService.Domain.Models;
using MediaService.Persistence;
using MediaService.Persistence.Repositories;
using Microsoft.Extensions.Hosting;

namespace MediaService.Persistence.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    private static void AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IMongoDbContext, MongoDbContext>();
    }

    public static void AddPersistenceServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDatabaseConfiguration(builder.Configuration);
        builder.Services
            .AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork))
            .AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>))
            .AddScoped(typeof(ICategoryRepository), typeof(CategoryRepository))
            .AddScoped(typeof(IPostRepository), typeof(PostRepository))
            .AddScoped(typeof(IUserRepository), typeof(UserRepository))
            .AddScoped(typeof(IBookMarkRepository), typeof(BookMarkRepository))
            .AddScoped(typeof(IFavouriteCategoryRepository), typeof(FavouriteCategoryRepository))
            .AddScoped(typeof(IMediaRepository), typeof(MediaRepository))
            .AddScoped(typeof(ILikeRepository), typeof(LikeRepository))
            .AddScoped(typeof(IPostCategoryRepository), typeof(PostCategoryRepository))
            .AddScoped(typeof(IOutboxEventRepository), typeof(OutboxEventRepository))
            .AddScoped(typeof(IOutboxEventConsumerRepository), typeof(OutboxEventConsumerRepository));
    }
}

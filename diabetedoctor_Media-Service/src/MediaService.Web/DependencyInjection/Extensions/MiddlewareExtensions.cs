using MediaService.Presentation.Middlewares;

namespace MediaService.Web.DependencyInjection.Extensions;

public static class MiddlewareExtensions
{
    public static void ConfigureMiddleware(this WebApplication app)
    {
        // if (app.Environment.IsDevelopment())
        // {
            app.ConfigureSwagger();
        // }

        app.UseHttpsRedirection();

        app.MapCarter();
        app.UseCors("AllowLocalhost");

        app.UseMiddleware<ExceptionHandlingMiddleware>()
            .UseMiddleware<RequireRoleMiddleware>();

        app.NewVersionedApi(CategoryEndpoint.ApiName)
            .MapCategoryApiV1();
        app.NewVersionedApi(PostEndpoint.ApiName)
            .MapPostApiV1();
        // app.NewVersionedApi(UserEndpoint.ApiName)
        //     .MapUserApiV1();
        app.NewVersionedApi(BookMarkEndpoint.ApiName)
            .MapBookMarkApiV1();
        app.NewVersionedApi(FavouriteCategoryEndpoint.ApiName)
            .MapFavouriteCategoryApiV1();
        app.NewVersionedApi(MediaEndpoint.ApiName)
            .MapMediaApiV1();
        app.NewVersionedApi(LikeEndpoint.ApiName)
            .MapLikeApiV1();
    }
}

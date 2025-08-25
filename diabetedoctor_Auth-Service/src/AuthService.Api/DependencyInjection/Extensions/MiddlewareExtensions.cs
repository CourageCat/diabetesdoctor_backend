using AuthService.Api.Endpoints.V1;

namespace AuthService.Api.DependencyInjection.Extensions;

public static class MiddlewareExtensions
{
    public static void ConfigureMiddleware(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
        }

        app.ConfigureSwagger();

        app.UseMiddleware<ExceptionHandlingMiddleware>()
            .UseMiddleware<RequireRoleMiddleware>();

        app.MapCarter();

        app.NewVersionedApi("Auth")
           .MapAuthApiV1();
        
        app.UseHttpsRedirection();
    }
}

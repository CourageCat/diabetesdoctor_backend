using Carter;
using NotificationService.Presentation.Endpoints.V1;
using NotificationService.Presentation.Middlewares;

namespace NotificationService.Web.DependencyInjections.Extensions;

public static class MiddlewareExtensions
{
    public static void ConfigureMiddleware(this WebApplication app)
    {
        //if (app.Environment.IsDevelopment())
        //{
        //    app.ConfigureSwagger();
        //}

        app.ConfigureSwagger();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<ExceptionHandlingMiddleware>();
        
        app.MapCarter();

        app.NewVersionedApi(NotificationEndpoints.ApiName)
            .MapNotificationsApiV1();

        app.UseHttpsRedirection();
    }
}

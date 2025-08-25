using ConsultationService.Presentation.Hubs;
using ConsultationService.Presentation.Middlewares;
using ConsultationService.Presentation.V1;

namespace ConsultationService.Web.Extensions;

public static class MiddlewareExtensions
{
    public static void ConfigureMiddleware(this WebApplication app)
    {
        
        //if (app.Environment.IsDevelopment())
        //{
        //    app.ConfigureSwagger();
        //}

        app.ConfigureSwagger();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        
        app.UseAuthorization();
        
        app.UseCors();

        app.UseMiddleware<ExceptionHandlingMiddleware>();
        
        app.NewVersionedApi(DoctorEndpoints.ApiName)
            .MapDoctorApiV1();

        app.NewVersionedApi(ConsultationEndpoints.ApiName)
            .MapConsultationApiV1();
        
        app.MapHub<ConsultationHub>("/hub/consultation");
        // app.MapCarter();

    }
}
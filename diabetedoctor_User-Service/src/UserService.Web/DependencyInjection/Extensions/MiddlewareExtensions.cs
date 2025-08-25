using UserService.Presentation.Protos.Server.UserPackage;

namespace UserService.Api.DependencyInjection.Extensions;

public static class MiddlewareExtensions
{
    public static void ConfigureMiddleware(this WebApplication app)
    {
        app.ConfigureSwagger();
        app.MapGrpcService<UserPackageProtoService>();

        app.UseHttpsRedirection();

        // app.MapCarter();
        
        // app.UseAuthentication();
        //
        // app.UseAuthorization();

        app.UseMiddleware<ExceptionHandlingMiddleware>()
         .UseMiddleware<RequireRoleMiddleware>();

        app.NewVersionedApi(PatientEndpoint.ApiName)
           .MapPatientApiV1();
        
        app.NewVersionedApi(DoctorEndpoint.ApiName)
            .MapDoctorApiV1();
        
        app.NewVersionedApi(HospitalEndpoint.ApiName)
            .MapHospitalApiV1();
        
        app.NewVersionedApi(ModeratorEndpoint.ApiName)
            .MapModeratorApiV1();
        
        app.NewVersionedApi(ServicePackageEndpoint.ApiName)
            .MapServicePackageApiV1();
        
        app.NewVersionedApi(PaymentEndpoint.ApiName)
            .MapPaymentApiV1();
    }
}

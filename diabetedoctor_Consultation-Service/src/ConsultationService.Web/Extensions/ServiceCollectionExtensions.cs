using System.Text;
using System.Text.Json.Serialization;
using Carter;
using ConsultationService.Application.Protos.Client.UserPackage;
using ConsultationService.Contract.Settings;
using ConsultationService.Presentation.Extensions;
using ConsultationService.Presentation.Middlewares;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.IdentityModel.Tokens;

namespace ConsultationService.Web.Extensions;

public static class ServiceCollectionExtensions 
{
    public static void AddWebService(this IHostApplicationBuilder builder)
    {
        builder.AddConfigurationAppSetting();
        // builder.AddConfigureGrpcServices();
        
        // builder.Services.Configure<JsonOptions>(options =>
        // {
        //     options.SerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        // });

        builder.AddConfigureGrpcServices();
        
        builder.Services.AddSingleton<PresenceTracker>();
        
        builder.Services.AddSignalR();
        
        builder.Services.AddCarter();

        builder.Services.AddScoped<ExceptionHandlingMiddleware>();

        builder.Services.AddHttpContextAccessor();
        
        builder.AddAuthenticationAndAuthorization();

        builder.Services
            .AddSwaggerGenNewtonsoftSupport()
            .AddFluentValidationRulesToSwagger()
            .AddEndpointsApiExplorer()
            .AddSwagger();

        builder.Services
            .AddApiVersioning(options => options.ReportApiVersions = true)
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
        
        builder.Services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });
        
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(cors => 
                cors.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });
    }
    private static void AddConfigurationAppSetting(this IHostApplicationBuilder builder)
    {
        builder.Services
            .Configure<KafkaSettings>(builder.Configuration.GetSection(KafkaSettings.SectionName))
            .Configure<MongoDbSettings>(builder.Configuration.GetSection(MongoDbSettings.SectionName))
            .Configure<AuthSettings>(builder.Configuration.GetSection(AuthSettings.SectionName))
            .Configure<AblySetting>(builder.Configuration.GetSection(AblySetting.SectionName))
            .Configure<CloudinarySettings>(builder.Configuration.GetSection(CloudinarySettings.SectionName))
            .Configure<AppDefaultSettings>(builder.Configuration.GetSection(AppDefaultSettings.SectionName))
            .Configure<GrpcSettings>(builder.Configuration.GetSection(GrpcSettings.SectionName));
    }
    
    private static void AddAuthenticationAndAuthorization(this IHostApplicationBuilder builder)
    {
        var authSettings = builder.Configuration.GetSection(AuthSettings.SectionName).Get<AuthSettings>() ?? new AuthSettings();


        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = authSettings.Issuer,
                    ValidAudience = authSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.AccessSecretToken)),
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Append("IS-TOKEN-EXPIRED", "true");
                        }
                        return Task.CompletedTask;
                    },
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];    
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hub"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        builder.Services.AddAuthorizationBuilder();
    }

    private static void AddConfigureGrpcServices(this IHostApplicationBuilder builder)
    {
        // Client
        var grpcSettings = builder.Configuration
            .GetSection(GrpcSettings.SectionName)
            .Get<GrpcSettings>() ?? throw new InvalidOperationException("Grpc config missing");
        builder.Services.AddGrpcClient<UserPackage.UserPackageClient>(options =>
        {
            options.Address = new Uri(grpcSettings.UserServiceAddress);
        });
        
        // Server
    }

}
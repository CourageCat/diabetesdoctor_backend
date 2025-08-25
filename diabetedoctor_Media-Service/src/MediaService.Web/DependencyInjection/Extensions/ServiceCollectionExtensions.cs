using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MediaService.Presentation.Middlewares;

namespace MediaService.Web.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions 
{
    public static IServiceCollection AddConfigurationAppSetting
     (this IServiceCollection services, IConfiguration configuration)
    {
        services
            .Configure<KafkaSettings>(configuration.GetSection(KafkaSettings.SectionName))
            .Configure<MongoDbSetting>(configuration.GetSection(MongoDbSetting.SectionName))
            .Configure<CloudinarySetting>(configuration.GetSection(CloudinarySetting.SectionName));

        return services;
    }

    public static IServiceCollection AddAuthenticationAndAuthorization(this IServiceCollection services, IConfiguration configuration)
    {
        var authSettings = new AuthSettings();
        configuration.GetSection(AuthSettings.SectionName).Bind(authSettings);


        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
       .AddJwtBearer(options =>
       {
           options.SaveToken = true;
           options.TokenValidationParameters = new TokenValidationParameters
           {
               ValidateIssuer = true,
               ValidateAudience = true,
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
                       context.Response.Headers.Add("IS-TOKEN-EXPIRED", "true");
                   }
                   return Task.CompletedTask;
               },
           };
       });

        services.AddAuthorization(options =>
        {
        });

        return services;
    }

    public static void AddWebService(this IHostApplicationBuilder builder)
    {
        builder.Services.AddConfigurationAppSetting(builder.Configuration);

        builder.Services.AddCarter();

        builder.Services.AddScoped<ExceptionHandlingMiddleware>();
        builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);
        builder.Services.AddAuthentication();
        builder.Services.AddAuthorization();
        
        builder.Services
            .AddTransient<ExceptionHandlingMiddleware>()
            .AddTransient<RequireRoleMiddleware>();

        builder.Services
            .AddSwaggerGenNewtonsoftSupport()
            .AddFluentValidationRulesToSwagger()
            .AddEndpointsApiExplorer()
            .AddSwagger();
        builder.Services.AddHttpContextAccessor();

        builder.Services
            .AddApiVersioning(options => options.ReportApiVersions = true)
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        builder.Services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        });
        // Startup.cs hoặc Program.cs
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowLocalhost",
                builder =>
                {
                    builder.WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
        });
        //builder.Services.AddCors(options =>
        //{
        //    options.AddDefaultPolicy(cors => cors.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        //});
    }
}
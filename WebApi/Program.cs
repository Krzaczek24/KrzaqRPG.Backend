using FluentValidation;
using Krzaq.MediatR;
using Krzaq.MediatR.Interfaces;
using KrzaqRPG.Backend.Common;
using KrzaqRPG.Backend.Database.SQLite;
using KrzaqRPG.Backend.WebApi.Core.Converters;
using KrzaqRPG.Backend.WebApi.Core.Errors;
using KrzaqRPG.Backend.WebApi.Core.Middlewares;
using KrzaqRPG.Backend.WebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Controllers;
using NLog.Extensions.Logging;
using NLog.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KrzaqRPG.Backend.WebApi;

public class Program
{
    private const string OPENAPI_PREFIX = "/openapi";
    private const string DOC_NAME = "swagger";
    public const string ENDPOINT = $"{OPENAPI_PREFIX}/{DOC_NAME}.json";

    public static void Main(string[] args)
    {
        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
#if DEBUG
        TravelService.Settings.SlidingExpiration = TimeSpan.FromSeconds(10);
#endif

        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddJsonFile("config.json", optional: false);

        builder.Logging.ClearProviders();
        builder.Logging.SetMinimumLevel(LogLevel.Trace);

        builder.Host.UseNLog();

        builder.Services
            .AddControllers(options =>
            {
                options.Conventions.Add(
                    new RouteTokenTransformerConvention(
                        new KebabCaseTransformer()));
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

        builder.Services.AddOpenApi(DOC_NAME, options =>
        {
            options.AddOperationTransformer(async (operation, context, cancellationToken) =>
            {
                if (context.Description.ActionDescriptor is ControllerActionDescriptor action)
                {
                    operation.OperationId = action.MethodInfo.Name;
                }
            });
        });

        builder.Services.ConfigureHttpJsonOptions(opts =>
        {
            opts.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddMemoryCache();

        //builder.Services.Configure<DatabaseConfig>(builder.Configuration.GetSection("database:mikrus"));

        builder.Services.AddAppDatabase(
#if DEBUG
            new LoggerFactory([new NLogLoggerProvider()])
#endif
            );

        builder.Services.AddMediator().AddHandlers().AddValidators().AddSingleton<IRequestErrorsHandler, ErrorHandler>();
        builder.Services.AddScoped<ITravelService, TravelService>();

        builder.Services.Configure<ApiBehaviorOptions>(opts =>
        {
            opts.SuppressModelStateInvalidFilter = true;
            opts.SuppressConsumesConstraintForFormFileParameters = true;
        });

        var allowedOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin", policy =>
            {
                policy.WithOrigins(allowedOrigins ?? [])
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        var app = builder.Build();

        app.MapOpenApi();

#if DEBUG
        app.UseSwaggerUI(x => x.SwaggerEndpoint(ENDPOINT, nameof(KrzaqRPG)));
#endif

        app.UseWhen(
            context => !context.Request.Path.StartsWithSegments(OPENAPI_PREFIX),
            app => app
                .UseCopyingHeadersMiddleware()
                .UseLoggingMiddleware()
                .UseExceptionHandlingMiddleware()
        );
        
        app.UseCors("AllowSpecificOrigin");

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}

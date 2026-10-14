using FluentValidation;
using Krzaq.MediatR;
using Krzaq.MediatR.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using NLog.Extensions.Logging;
using NLog.Web;
using RpgAdventureGame.Backend.Common;
using RpgAdventureGame.Backend.Database.SQLite;
using RpgAdventureGame.Backend.WebApi.Core.Converters;
using RpgAdventureGame.Backend.WebApi.Core.Errors;
using RpgAdventureGame.Backend.WebApi.Core.Middlewares;
using RpgAdventureGame.Backend.WebApi.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RpgAdventureGame.Backend.WebApi
{
    public class Program
    {
        private const string OPENAPI_PREFIX = "/openapi";
        private const string DOC_NAME = "swagger";
        public const string ENDPOINT = $"{OPENAPI_PREFIX}/{DOC_NAME}.json";

        public static void Main(string[] args)
        {
            ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
            if (EnvInfo.IsDebug)
            {
                TravelService.Settings.SlidingExpiration = TimeSpan.FromSeconds(10);
            }

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

            builder.Services.AddOpenApi(DOC_NAME);

            builder.Services.ConfigureHttpJsonOptions(opts =>
            {
                opts.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddMemoryCache();

            //builder.Services.Configure<DatabaseConfig>(builder.Configuration.GetSection("database:mikrus"));

            builder.Services.AddAppDatabase(EnvInfo.IsDebug ? new LoggerFactory([new NLogLoggerProvider()]) : null);

            builder.Services.AddMediator().AddHandlers().AddValidators().AddSingleton<IRequestErrorsHandler, ErrorHandler>();
            builder.Services.AddScoped<ITravelService, TravelService>();

            builder.Services.Configure<ApiBehaviorOptions>(opts =>
            {
                opts.SuppressModelStateInvalidFilter = true;
                opts.SuppressConsumesConstraintForFormFileParameters = true;
            });

            var app = builder.Build();

            app.MapOpenApi();

            if (EnvInfo.IsDebug)
            {
                app.UseSwaggerUI(x => x.SwaggerEndpoint(ENDPOINT, nameof(RpgAdventureGame)));
            }

            app.UseWhen(
                context => !context.Request.Path.StartsWithSegments(OPENAPI_PREFIX),
                app => app
                    .UseCopyingHeadersMiddleware()
                    .UseLoggingMiddleware()
                    .UseExceptionHandlingMiddleware()
            );

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}

using Asp.Versioning;
using Microsoft.Extensions.Options;
using Serilog;
using SMeta.Web.BackgroundServices;
using SMeta.Web.Endpoints;
using SMeta.Web.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Default";
Log.Information("Application starting up in {Environment} mode", env);

try
{
    RunApplication(args);

    Log.Information("Stopped cleanly");
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "An unhandled exception occured during bootstrapping");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

static void RunApplication(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services));

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
    builder.Services.AddSwaggerGen(options => options.OperationFilter<SwaggerDefaultValues>());
    builder.Services.AddApiVersioning(options =>
    {
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    }).AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVVV";
        options.SubstituteApiVersionInUrl = true;
    });

    builder.Services.AddHostedService<SubscriptionService>();

    var app = builder.Build();

    app.UseSerilogRequestLogging(options =>
    {
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            var apiVersion = httpContext.GetRequestedApiVersion();
            diagnosticContext.Set("ApiVersion", apiVersion?.ToString("'v'VVVV"));
        };
        options.MessageTemplate =
            "HTTP {RequestMethod} {RequestPath} version {ApiVersion} responded {StatusCode} in {Elapsed:0.0000} ms";
    });

    var apiVersionSet = app.NewApiVersionSet()
        .HasApiVersion(new ApiVersion(1.0, "alpha"))
        .HasApiVersion(new ApiVersion(1.0))
        .HasApiVersion(new ApiVersion(1.1))
        .HasApiVersion(new ApiVersion(2.19))
        .ReportApiVersions()
        .Build();

    app.MapProjectEndpoints(apiVersionSet);

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var descriptions = app.DescribeApiVersions();
        foreach (var description in descriptions)
        {
            var url = $"/swagger/{description.GroupName}/swagger.json";
            var name = description.GroupName;
            options.SwaggerEndpoint(url, name);
        }
    });

    app.Run();
}
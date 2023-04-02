using Asp.Versioning;
using Microsoft.Extensions.Options;
using SMeta.Web.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(options => options.OperationFilter<SwaggerDefaultValues>());
builder.Services.AddApiVersioning(options =>
{
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

var app = builder.Build();

var versionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1.0))
    .HasApiVersion(new ApiVersion(2.0))
    .ReportApiVersions()
    .Build();

var baseApiUrl = "api/v{version:apiVersion}";

app.MapGet($"{baseApiUrl}/hello", () => "Hello world")
    .WithApiVersionSet(versionSet)
    .MapToApiVersion(new ApiVersion(1.0));

app.MapGet($"{baseApiUrl}/hello", () => "Hello world v2")
    .WithApiVersionSet(versionSet)
    .MapToApiVersion(new ApiVersion(2.0));

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    var descriptions = app.DescribeApiVersions();
    foreach (var description in descriptions)
    {
        var url = $"/swagger/{description.GroupName}/swagger.json";
        var name = description.GroupName.ToUpperInvariant();
        options.SwaggerEndpoint(url, name);
    }
});

app.Run();
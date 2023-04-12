using Asp.Versioning.Builder;
using Asp.Versioning;

namespace SMeta.Web.Extensions;

public static class EndpointRouteBuilderExtensions
{
    private const string BaseApiUrl = "api/v{version:apiVersion}";

    public static IEndpointRouteBuilder GetVersionedRouteGroup(this IEndpointRouteBuilder endpoints, ApiVersionSet apiVersionSet,
        string urlPrefix, string groupName, ApiVersion apiVersion)
    {
        var routeGroup = endpoints.MapGroup($"{BaseApiUrl}/{urlPrefix}")
            .WithTags(groupName)
            .WithApiVersionSet(apiVersionSet)
            .MapToApiVersion(apiVersion);
        return routeGroup;
    }
}

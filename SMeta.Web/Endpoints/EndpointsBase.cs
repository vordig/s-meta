using Asp.Versioning.Builder;
using Asp.Versioning;

namespace SMeta.Web.Endpoints;

public static class EndpointsBase
{
    private const string BaseApiUrl = "api/v{version:apiVersion}";

    public static RouteGroupBuilder GetRouteGroup(IEndpointRouteBuilder endpoints, ApiVersionSet apiVersionSet,
        string urlPrefix, string groupName, ApiVersion apiVersion)
    {
        var routeGroup = endpoints.MapGroup($"{BaseApiUrl}/{urlPrefix}")
            .WithTags(groupName)
            .WithApiVersionSet(apiVersionSet)
            .MapToApiVersion(apiVersion);
        return routeGroup;
    }
}

using Asp.Versioning;
using Asp.Versioning.Builder;
using SMeta.Web.Extensions;

namespace SMeta.Web.Endpoints;

public static class ProjectEndpoints
{
    public static void MapProjectEndpoints(this IEndpointRouteBuilder endpoints, ApiVersionSet apiVersionSet)
    {
        var projectsV1 = endpoints.GetVersionedRouteGroup(apiVersionSet, "projects", "Projects", new ApiVersion(1.0, "alpha"));
        var projectsV1_1 = endpoints.GetVersionedRouteGroup(apiVersionSet, "projects", "Projects", new ApiVersion(1.1));
        var projectsV2 = endpoints.GetVersionedRouteGroup(apiVersionSet, "projects", "Projects", new ApiVersion(2.19));

        MapProjectV1Endpoints(projectsV1);
        MapProjectV1Endpoints(projectsV1_1);

        MapProjectV2Endpoints(projectsV2);
    }

    private static void MapProjectV1Endpoints(IEndpointRouteBuilder projects)
    {
        projects.MapGet("", () => "Projects v1");
    }

    private static void MapProjectV2Endpoints(IEndpointRouteBuilder projects)
    {
        projects.MapGet("", () => "Projects v2");
    }
}

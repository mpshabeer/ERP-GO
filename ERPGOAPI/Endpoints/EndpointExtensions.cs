using Microsoft.AspNetCore.Routing;

namespace ERPGOAPI.Endpoints;

public static class EndpointExtensions
{
    public static void MapAllEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapAuthEndpoints();
        app.MapMasterDataEndpoints();
        app.MapPartyEndpoints();
        app.MapStockEndpoints();
        app.MapTransactionEndpoints();
        app.MapReportEndpoints();
    }
}

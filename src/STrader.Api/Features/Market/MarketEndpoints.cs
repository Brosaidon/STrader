namespace STrader.Api.Features.Market;

public static class MarketEndpoints
{
    public static void MapMarket(this WebApplication app)
    {
        app.MapGet("/market", (HttpRequest request) =>
        {
            var html = """
            <section>
            <h2>Market</h2>
                <p>Market data will be displayed here.</p>
            </section>
            """;

            //HTMX request - return partial HTML
            if (request.Headers.ContainsKey("HX-Request"))
                return Results.Content(html, "text/html");
            //Normal request - return full page
            return Results.Content(
            Layout.LayoutHtml.Page(html),
            "text/html");

        });
    }
}
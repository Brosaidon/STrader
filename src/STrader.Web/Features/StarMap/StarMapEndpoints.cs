namespace STrader.Web.Features.StarMap;

using STrader.Application.Services;
using STrader.Application.Interfaces;

public static class StarMapEndpoints
{
    public static void MapStarMap(this WebApplication app)
    {
        app.MapPost("/travel", (
            IMarketService service,
            SessionService session,
            PendingActionStore store,
            HttpRequest request) =>
        {
            service.ExecuteTurn(session, store.Actions);

            // 🔥 Clear after commit
            store.Actions.Clear();

            var html = MarketView.Render(session, store.Actions);
            return WebHelpers.Html(request, html);
        });

        app.MapGet("/star-map", (HttpRequest request) =>
        {
            var html = """
                <section>
                    <h2>Star Map</h2> 
                    <ul>
                        <li>Sectors</li>
                        <li>Stations</li>
                    </ul>
                </section>
                """;

            if (request.Headers.ContainsKey("HX-Request"))
                return Results.Content(html, "text/html");

            return Results.Content(
                Layout.LayoutHtml.Page(html),
                "text/html");
        });
    }
}
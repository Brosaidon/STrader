namespace STrader.Web.Features.StarMap;

using STrader.Application.Services;

public static class StarMapEndpoints
{
    public static void MapStarMap(this WebApplication app)
    {
        app.MapPost("/travel", (
            ITurnService turnService,
            SessionService session,
            NetTradeStore store
            ) =>
        {
            turnService.ExecuteTurn(session, store);

            // Here we might start some post travel events.

            return Results.Redirect("/market");
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
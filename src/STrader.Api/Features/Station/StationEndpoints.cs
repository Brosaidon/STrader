namespace STrader.Api.Features.Station;

public static class StationEndpoints
{
    public static void MapStation(this WebApplication app)
    {
        app.MapGet("/station", (HttpRequest request) =>
        {
            var html = """
                <ul>
                    <li>Hire Crew</li>
                    <li>Repair Services</li>
                    <li>Outfitting</li>
                </ul>
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
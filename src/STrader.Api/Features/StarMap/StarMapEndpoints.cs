namespace STrader.Api.Features.StarMap;

public static class StarMapEndpoints
{
    public static void MapStarMap(this WebApplication app)
    {
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
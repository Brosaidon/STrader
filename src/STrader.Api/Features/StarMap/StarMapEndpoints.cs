namespace STrader.Api.Features.StarMap;

public static class StarMapEndpoints
{
    public static void MapStarMap(this WebApplication app)
    {
        app.MapGet("/star-map", () =>
            Results.Content(
                """
                <h2>Star Map Overview</h2>
                <p>Explore the galaxy with our interactive star map. Click on different sectors to discover resources.</p>
                <ul>
                    <li>Sectors</li>
                    <li>Stations</li>
                """,
                "text/html"));
    }
}
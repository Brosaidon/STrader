namespace STrader.Api.Features.Station;

public static class StationEndpoints
{
    public static void MapStation(this WebApplication app)
    {
        app.MapGet("/station", () =>
            Results.Content(
                """
                <h2>Station Overview</h2>
                <p>Welcome to the station! Here you can find various services and facilities.</p>
                <ul>
                    <li>Hire Crew</li>
                    <li>Repair Services</li>
                    <li>Outfitting</li>
                </ul>
                """,
                "text/html"));
    }
}
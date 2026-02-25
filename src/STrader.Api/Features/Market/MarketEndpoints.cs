namespace STrader.Api.Features.Market;

public static class MarketEndpoints
{
    public static void MapMarket(this WebApplication app)
    {
        app.MapGet("/market", () =>
            Results.Content(
                """
                <table>
                    <thead>
                        <tr>
                            <th>Commodity</th>
                            <th>Price</th>
                            <th>Supply</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td>Ore</td>
                            <td>120</td>
                            <td>High</td>
                        </tr>
                        <tr>
                            <td>Food</td>
                            <td>80</td>
                            <td>Low</td>
                        </tr>
                    </tbody>
                </table>
                """,
                "text/html"));
    }
}
using STrader.Web.State;

namespace STrader.Web.Features.Market;

public static class MarketView
{
    public static string Render()
    {
        var commodities = GameState.MarketCommodities;

        //this here is for each commodity. i think its cool.
        var rows = string.Join("", commodities.Select(MarketHelpers.RenderCommodityRow));

        var html = $"""
        <h2>Market</h2>

        <p>Credits: {GameState.Credits}</p>

        <table>
          <thead>
            <tr>
                <th>Name</th>
                <th>Icon</th>
                <th>Available</th>
                <th>Actions</th>
                <th>In Cargo</th>
            </tr>
        </thead>
        <tbody>
            {rows}
        </tbody>
        </table>
        """;

        return html;
    }
}
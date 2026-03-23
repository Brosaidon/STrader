using STrader.Application.Services;
using STrader.Domain.Entities;

namespace STrader.Web.Features.Market;

public static class MarketView
{
    public static string Render(SessionService session)
    {
        var commodities = session.Market;

        //this here is for each commodity. i think its cool.
        var rows = string.Join("", commodities.Select(item => MarketHelpers.RenderCommodityRow(session, item)));
        var html = $"""
        <h2>Market</h2>

        <p>Credits: {session.Credits}</p>

        <table>
          <thead>
            <tr>
                <th>Name</th>
                <th>Icon</th>
                <th>Available</th>
                <th>Price</th>
                <th>Actions</th>
                <th>Price Change</th> <!-- reserved for later -->
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
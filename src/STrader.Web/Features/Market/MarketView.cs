using STrader.Application.Services;
using STrader.Application.Models;
using STrader.Domain.Entities;

namespace STrader.Web.Features.Market;

public static class MarketView
{
    public static string Render(SessionService session, List<PendingAction> pendingActions)
    {
        var commodities = session.Market;

        // Render each commodity row using MarketHelpers, passing pendingActions for projections
        var rows = string.Join("", commodities.Select(item =>
            MarketHelpers.RenderCommodityRow(session, item, pendingActions)
        ));

        // Use ProjectionHelper for optimistic credits and cargo
        var projectedCredits = ProjectionHelper.GetProjectedCredits(session, pendingActions);
        var projectedCargoLeft = ProjectionHelper.GetProjectedCargoLeft(session, pendingActions);

        var html = $"""
        <h2>Market</h2>

        <p>Credits: {projectedCredits}</p>
        <p>Cargo left: {projectedCargoLeft} / {session.CargoSpace}</p>

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
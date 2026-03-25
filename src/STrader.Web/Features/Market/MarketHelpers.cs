using STrader.Application.Services;
using STrader.Domain.Entities;
using STrader.Application.Models;

namespace STrader.Web.Features.Market
{
    public static class MarketHelpers
    {
        // Updated to accept pending actions for optimistic rendering
        public static string RenderCommodityRow(SessionService session, MarketItem item, List<PendingAction> pendingActions)
        {
            var (name, icon, inCargo, projectedAvailable, projectedInCargo) = Resolve(session, item, pendingActions);

            return $"""
<tr>
    <td>{EscapeHtml(name)}</td>
    <td>{icon}</td>
    <td>{projectedAvailable}</td>
    <td>{item.Price}</td>
    <td>
        <button hx-post="/market/buy/{item.ItemId}" hx-target="#content" hx-swap="innerHTML">Buy</button>
        <button hx-post="/market/buy-max/{item.ItemId}" hx-target="#content" hx-swap="innerHTML">Buy Max</button>
        <button hx-post="/market/sell/{item.ItemId}" hx-target="#content" hx-swap="innerHTML">Sell</button>
        <button hx-post="/market/sell-all/{item.ItemId}" hx-target="#content" hx-swap="innerHTML">Sell All</button>
    </td>
    <td></td> <!-- Price change column reserved -->
    <td>{projectedInCargo}</td>
</tr>
""";
        }

        // Calculates projected stock and cargo based on pending actions
        private static (string name, string icon, int inCargo, int projectedAvailable, int projectedInCargo)
            Resolve(SessionService session, MarketItem item, List<PendingAction> pendingActions)
        {
            var catalogItem = ItemCatalog.Items.First(i => i.Id == item.ItemId);

            var cargoItem = session.Cargo.FirstOrDefault(c => c.ItemId == item.ItemId);
            var inCargo = cargoItem?.Quantity ?? 0;

            // Apply pending actions for projections
            var pendingBuy = pendingActions
                .Where(a => a.ItemId == item.ItemId && a.ActionType == ActionType.Buy)
                .Sum(a => a.Quantity);

            var pendingSell = pendingActions
                .Where(a => a.ItemId == item.ItemId && a.ActionType == ActionType.Sell)
                .Sum(a => a.Quantity);

            var projectedAvailable = Math.Max(0, item.Available - pendingBuy + pendingSell);
            var projectedInCargo = Math.Max(0, inCargo + pendingBuy - pendingSell);

            return (catalogItem.Name, catalogItem.Icon, inCargo, projectedAvailable, projectedInCargo);
        }

        private static string EscapeHtml(string text)
        {
            return text
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&#39;");
        }
    }
}
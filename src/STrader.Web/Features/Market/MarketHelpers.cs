using STrader.Application.Services;
using STrader.Domain.Entities;

namespace STrader.Web.Features.Market
{
    public static class MarketHelpers
    {
        // This method generates an HTML table row for a commodity, including its name, price, and price changes.
        public static string RenderCommodityRow(SessionService session, MarketItem item)
        {
            var (name, icon, inCargo) = Resolve(session, item);

            return $"""
    <tr>
        <td>{EscapeHtml(name)}</td>
        <td>{icon}</td>
        <td>{item.Available}</td>
        <td>{item.Price}</td>
        <td>
            <button hx-post="/market/buy/{item.ItemId}" hx-target="#content" hx-swap="innerHTML">Buy</button>
            <button hx-post="/market/buy-max/{item.ItemId}" hx-target="#content" hx-swap="innerHTML">Buy Max</button>
            <button hx-post="/market/sell/{item.ItemId}" hx-target="#content" hx-swap="innerHTML">Sell</button>
            <button hx-post="/market/sell-all/{item.ItemId}" hx-target="#content" hx-swap="innerHTML">Sell All</button>
        </td>
        <td></td> <!-- Price change column reserved -->
        <td>{inCargo}</td>
    </tr>
    """;
        }

        //This method looks up items in the catalog, resulting in leaking logic into the web layer. This is not optimal, we keep this for now.
        private static (string name, string icon, int inCargo) Resolve(SessionService session, MarketItem item)
        {
            var catalogItem = ItemCatalog.Items
                .First(i => i.Id == item.ItemId);

            var cargoItem = session.Cargo.FirstOrDefault(c => c.ItemId == item.ItemId);

            return (
                catalogItem.Name,
                catalogItem.Icon,
                cargoItem?.Quantity ?? 0
            );
        }

        //This makes sure that any user-generated content is safely escaped before being rendered in the HTML, preventing XSS attacks.
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
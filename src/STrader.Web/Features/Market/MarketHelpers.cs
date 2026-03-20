using System;
using System.Text;
using STrader.Web.State;

namespace STrader.Web.Features.Market
{
    public static class MarketHelpers
    {
        // This method generates an HTML table row for a commodity, including its name, price, and price changes.
        public static string RenderCommodityRow(Commodity commodity)
        {
            return $"""
            <tr>
                <td>{EscapeHtml(commodity.Name)}</td>
                <td>{commodity.Icon}</td>
                <td>{commodity.Available}</td>

                <td>
                    <button
                        hx-post="/market/buy/{commodity.Name}"
                        hx-target="#content"
                        hx-swap="innerHTML">
                        Buy
                    </button>
                    <button
                        hx-post="/market/buy-max/{commodity.Name}"
                        hx-target="#content"
                        hx-swap="innerHTML">
                        Buy Max
                    </button>
                    <button
                        hx-post="/market/sell/{commodity.Name}"
                        hx-target="#content"
                        hx-swap="innerHTML">
                        Sell
                    </button>
                    <button
                        hx-post="/market/sell-all/{commodity.Name}"
                        hx-target="#content"
                        hx-swap="innerHTML">
                        Sell All
                    </button>
                </td>

                <td>{commodity.InCargo}</td>
            </tr>
            """;
            //TODO: Add price change indicators.
            //var change = commodity.Price - commodity.PreviousPrice;
            //var changePercent = (double)change / commodity.PreviousPrice * 100;
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
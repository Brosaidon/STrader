using System;
using System.Text;

namespace STrader.Web.Features.Market
{
    public static class MarketHelpers
    {
        // This method generates an HTML table row for a commodity, including its name, price, and price changes.
        public static string RenderCommodityRow(string name, decimal price, decimal change, decimal changePercent)
        {
            return $"""
                <tr>
                <td>{EscapeHtml(name)}</td>
                <td>{price:F2}</td>
                <td class="{(change >= 0 ? "positive" : "negative")}">{change:+0.00;-0.00}</td>
                <td class="{(changePercent >= 0 ? "positive" : "negative")}">{changePercent:+0.00;-0.00}%</td>
                </tr>
                """;
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
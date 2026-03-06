namespace STrader.Web.WebHelpers;

using STrader.Web.Layout;

public static class WebHelpers
{
    // Generates a HTML table row for a commodity.
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

    //Escapes user-generated content to prevent XSS.
    private static string EscapeHtml(string text)
    {
        return text
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&#39;");
    }

    //Returns either HTMX fragment or full layout page based on request type.
    public static IResult Html(HttpRequest request, string html)
    {
        if (request.Headers.ContainsKey("HX-Request"))
            return Results.Content(html, "text/html");

        return Results.Content(LayoutHtml.Page(html), "text/html");
    }
}

namespace STrader.Web.Features.Market;

using STrader.Application.Models;

public static class MarketView
{
  public static string Render(MarketStateDto model)
  {
    var rows = string.Join("", model.Items.Select(RenderRow));

    return $"""
        <h2>Market</h2>

        <p>Credits: {model.Credits}</p>
        <p>Cargo: {model.CargoUsed} / {model.CargoUsed + model.CargoLeft}</p>

        <table>
            <thead>
                <tr>
                    <th>Name</th>
                    <th>Icon</th>
                    <th>Available</th>
                    <th>Price</th>
                    <th>Actions</th>
                    <th>Price Change</th>
                    <th>In Cargo</th>
                </tr>
            </thead>
            <tbody>
                {rows}
            </tbody>
        </table>
        """;
  }

  private static string RenderRow(MarketItemDto item)
  {
    return $"""
        <tr>
            <td>{EscapeHtml(item.Name)}</td>
            <td>{item.Icon}</td>
            <td>{item.ProjectedAvailable}</td>
            <td>{item.Price}</td>
            <td>
                <button hx-post="/market/buy/{item.ItemId}" hx-target="#content" hx-swap="innerHTML">Buy</button>
                <button hx-post="/market/buy-max/{item.ItemId}" hx-target="#content" hx-swap="innerHTML">Buy Max</button>
                <button hx-post="/market/sell/{item.ItemId}" hx-target="#content" hx-swap="innerHTML">Sell</button>
                <button hx-post="/market/sell-all/{item.ItemId}" hx-target="#content" hx-swap="innerHTML">Sell All</button>
            </td>
            <td></td>
            <td>{item.ProjectedInCargo}</td>
        </tr>
        """;
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
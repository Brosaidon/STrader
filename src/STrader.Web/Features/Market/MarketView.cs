using STrader.Application.Services;
using STrader.Application.Models;
using STrader.Domain.Entities;

namespace STrader.Web.Features.Market;

public static class MarketView
{
  public static string Render(MarketStateDto model)
  {
    var rows = string.Join("", model.Items.Select(MarketHelpers.RenderCommodityRow));

    return $"""
    <h2>Market</h2>

    <p>Credits: {model.Credits}</p>
    <p>Cargo: {model.CargoUsed} / {model.CargoUsed + model.CargoLeft}</p>

    <table>
        <tbody>
            {rows}
        </tbody>
    </table>
    """;
  }
}
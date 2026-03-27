using STrader.Application.Services;
using STrader.Domain.Entities;
using STrader.Application.Models;

namespace STrader.Web.Features.Market
{
    public static class MarketHelpers
    {
        // Updated to accept pending actions for optimistic rendering
        public static string RenderCommodityRow(MarketItemDto item)
        {
            return $"""
        <tr>
            <td>{item.Name}</td>
            <td>{item.Icon}</td>
            <td>{item.Available}</td>
            <td>{item.Price}</td>
            <td>
                <button hx-post="/market/buy/{item.ItemId}">Buy</button>
                <button hx-post="/market/sell/{item.ItemId}">Sell</button>
            </td>
            <td>{item.InCargo}</td>
        </tr>
        """;
        }
    }
}
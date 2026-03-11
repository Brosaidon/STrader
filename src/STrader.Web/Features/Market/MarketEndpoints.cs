namespace STrader.Web.Features.Market;

using STrader.Web;
using STrader.Web.State;
using STrader.Web.WebHelpers;



public static class MarketEndpoints
{
    public static void MapMarket(this WebApplication app)
    {
        app.MapPost("/market/buy/{name}", BuyCommodity);
        app.MapPost("/market/buy-max/{name}", BuyMaxCommodity);
        app.MapPost("/market/sell/{name}", SellCommodity);
        app.MapPost("/market/sell-all/{name}", SellAllCommodity);
    }

    private static int GetCommodityIndex(string name)
    {
        return GameState.MarketCommodities.FindIndex(c => c.Name == name);
    }
    private static IResult RenderMarket(HttpRequest request, string name)
    {
        var commodity = GameState.MarketCommodities[GetCommodityIndex(name)];

        var html = MarketView.Render();
        return WebHelpers.Html(request, html);

    }

    private static IResult BuyCommodity(HttpRequest request, string name)
    {
        var commodity = GameState.MarketCommodities[GetCommodityIndex(name)];
        if (commodity.Available <= 0) return RenderMarket(request, name);

        if (commodity.InCargo >= GameState.CargoCapacity) return RenderMarket(request, name);

        if (GameState.Credits < commodity.Price) return RenderMarket(request, name);

        GameState.Credits -= commodity.Price;
        GameState.MarketCommodities[GetCommodityIndex(name)] = commodity with
        {
            Available = commodity.Available - 1,
            InCargo = commodity.InCargo + 1
        };

        return RenderMarket(request, name);
    }

    private static IResult BuyMaxCommodity(HttpRequest request, string name)
    {
        var commodity = GameState.MarketCommodities[GetCommodityIndex(name)];
        var space = GameState.CargoCapacity - commodity.InCargo;
        var maxAffordable = GameState.Credits / commodity.Price;
        var amount = Math.Min(commodity.Available, Math.Min(space, maxAffordable));

        GameState.Credits -= amount * commodity.Price;
        GameState.MarketCommodities[GetCommodityIndex(name)] = commodity with
        {
            Available = commodity.Available - amount,
            InCargo = commodity.InCargo + amount
        };

        return RenderMarket(request, name);
    }

    private static IResult SellCommodity(HttpRequest request, string name)
    {
        var commodity = GameState.MarketCommodities[GetCommodityIndex(name)];
        if (commodity.InCargo <= 0) return RenderMarket(request, name);

        GameState.Credits += commodity.Price;

        GameState.MarketCommodities[GetCommodityIndex(name)] = commodity with
        {
            Available = commodity.Available + 1,
            InCargo = commodity.InCargo - 1
        };

        return RenderMarket(request, name);
    }
    private static IResult SellAllCommodity(HttpRequest request, string name)
    {
        var commodity = GameState.MarketCommodities[GetCommodityIndex(name)];
        var amount = commodity.InCargo;

        GameState.Credits += amount * commodity.Price;

        GameState.MarketCommodities[GetCommodityIndex(name)] = commodity with
        {
            Available = commodity.Available + amount,
            InCargo = 0
        };

        return RenderMarket(request, name);
    }
}

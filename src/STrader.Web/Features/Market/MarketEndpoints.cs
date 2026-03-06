namespace STrader.Web.Features.Market;

using STrader.Web;
using STrader.Web.State;
using STrader.Web.WebHelpers;



public static class MarketEndpoints
{
    public static void MapMarket(this WebApplication app)
    {
        app.MapGet("/market", RenderMarket);
        app.MapPost("/market/buy", BuyFood);
        app.MapPost("/market/buy-max", BuyMaxFood);
        app.MapPost("/market/sell", SellFood);
        app.MapPost("/market/sell-all", SellAllFood);
    }
    private static IResult RenderMarket(HttpRequest request)
    {
        var food = GameState.Food;

        var html = MarketView.Render();
        return WebHelpers.Html(request, html);

    }

    private static IResult BuyFood(HttpRequest request)
    {
        var food = GameState.Food;
        if (food.Available <= 0) return RenderMarket(request);

        if (food.InCargo >= GameState.CargoCapacity) return RenderMarket(request);

        if (GameState.Credits < food.Price) return RenderMarket(request);

        GameState.Credits -= food.Price;
        GameState.MarketCommodities[0] = food with
        {
            Available = food.Available - 1,
            InCargo = food.InCargo + 1
        };

        return RenderMarket(request);
    }

    private static IResult BuyMaxFood(HttpRequest request)
    {
        var food = GameState.Food;
        var space = GameState.CargoCapacity - food.InCargo;
        var maxAffordable = GameState.Credits / food.Price;
        var amount = Math.Min(food.Available, Math.Min(space, maxAffordable));

        GameState.Credits -= amount * food.Price;
        GameState.MarketCommodities[0] = food with
        {
            Available = food.Available - amount,
            InCargo = food.InCargo + amount
        };

        return RenderMarket(request);
    }

    private static IResult SellFood(HttpRequest request)
    {
        var food = GameState.MarketCommodities[0];
        if (food.InCargo <= 0) return RenderMarket(request);

        GameState.Credits += food.Price;

        GameState.MarketCommodities[0] = food with
        {
            Available = food.Available + 1,
            InCargo = food.InCargo - 1
        };

        return RenderMarket(request);
    }
    private static IResult SellAllFood(HttpRequest request)
    {
        var food = GameState.MarketCommodities[0];
        var amount = food.InCargo;

        GameState.Credits += amount * food.Price;

        GameState.MarketCommodities[0] = food with
        {
            Available = food.Available + amount,
            InCargo = 0
        };

        return RenderMarket(request);
    }
}

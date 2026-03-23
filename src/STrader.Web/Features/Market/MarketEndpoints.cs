namespace STrader.Web.Features.Market;

using STrader.Application.Services;
using STrader.Web.WebHelpers;



public static class MarketEndpoints
{
    public static void MapMarket(this WebApplication app)
    {
        //Here We will inject the SessionService from the Application Layer.
        app.MapGet("/market", (SessionService session, HttpRequest request) =>
        {
            var html = MarketView.Render(session);
            return WebHelpers.Html(request, html);
        });

        app.MapPost("/market/buy/{itemId}", BuyCommodity);
        app.MapPost("/market/buy-max/{itemId}", BuyMaxCommodity);
        app.MapPost("/market/sell/{itemId}", SellCommodity);
        app.MapPost("/market/sell-all/{itemId}", SellAllCommodity);
    }

    private static IResult RenderMarket(HttpRequest request, SessionService session)
    {
        var html = MarketView.Render(session);
        return WebHelpers.Html(request, html);

    }

    private static IResult BuyCommodity(int itemId, SessionService session, HttpRequest request)
    {
        var item = session.GetMarketItem(itemId);
        if (item == null || item.Available <= 0) return RenderMarket(request, session);

        // check credits
        if (session.Credits < item.Price) return RenderMarket(request, session);

        // add to cargo
        var cargoItem = session.GetCargoItem(itemId);
        if (cargoItem == null)
        {
            session.Cargo.Add(new CargoItem { ItemId = itemId, Quantity = 1 });
        }
        else
        {
            cargoItem.Quantity += 1;
        }

        // update market
        item.Available -= 1;

        session.Credits -= item.Price;

        return RenderMarket(request, session);
    }

    private static IResult BuyMaxCommodity(int itemId, SessionService session, HttpRequest request)
    {
        var item = session.GetMarketItem(itemId);
        if (item == null || item.Available <= 0) return RenderMarket(request, session);

        var cargoItem = session.GetCargoItem(itemId);
        var inCargo = cargoItem?.Quantity ?? 0;
        var space = int.MaxValue; // if you have a cargo limit, replace with (CargoCapacity - inCargo)
        var maxAffordable = session.Credits / item.Price;
        var amount = Math.Min(item.Available, Math.Min(space, maxAffordable));

        // update market and cargo
        if (cargoItem == null)
            session.Cargo.Add(new CargoItem { ItemId = itemId, Quantity = amount });
        else
            cargoItem.Quantity += amount;

        item.Available -= amount;
        session.Credits -= amount * item.Price;

        return RenderMarket(request, session);
    }

    private static IResult SellCommodity(int itemId, SessionService session, HttpRequest request)
    {
        var item = session.GetMarketItem(itemId);
        var cargoItem = session.GetCargoItem(itemId);
        if (item == null || cargoItem == null || cargoItem.Quantity <= 0)
            return RenderMarket(request, session);

        cargoItem.Quantity -= 1;
        item.Available += 1;
        session.Credits += item.Price;

        return RenderMarket(request, session);
    }

    private static IResult SellAllCommodity(int itemId, SessionService session, HttpRequest request)
    {
        var item = session.GetMarketItem(itemId);
        var cargoItem = session.GetCargoItem(itemId);
        if (item == null || cargoItem == null || cargoItem.Quantity <= 0)
            return RenderMarket(request, session);

        var amount = cargoItem.Quantity;
        cargoItem.Quantity = 0;
        item.Available += amount;
        session.Credits += amount * item.Price;

        return RenderMarket(request, session);
    }
}

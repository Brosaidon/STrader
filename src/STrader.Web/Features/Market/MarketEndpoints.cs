namespace STrader.Web.Features.Market;

using STrader.Application.Services;
using STrader.Web.WebHelpers;
using STrader.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

public static class MarketEndpoints
{
    public static void MapMarket(this WebApplication app)
    {
        //Always get market via service.
        app.MapGet("/market", (
            SessionService session,
            [FromServices] NetTradeStore store,
            [FromServices] IMarketService service,
            HttpRequest request) =>
        {
            // Use projections for UI rendering
            var model = service.GetMarket(session, store.Net);
            return WebHelpers.Html(request, MarketView.Render(model));
        });

        app.MapPost("/market/buy/{itemId}", (
            int itemId,
            SessionService session,
            [FromServices] IMarketService service,
            [FromServices] NetTradeStore store,
            HttpRequest request) =>
        {
            return HandleNetTrade(itemId, session, service, store, request, 1);
        });

        //Buy max

        app.MapPost("/market/buy-max/{itemId}", (
            int itemId,
            SessionService session,
            [FromServices] IMarketService service,
            [FromServices] NetTradeStore store,
            HttpRequest request) =>
             {
                 var qty = service.GetMaxBuyQuantity(session, store.Net, itemId);

                 return HandleNetTrade(itemId, session, service, store, request, qty);
             });

        //Sell 1
        app.MapPost("/market/sell/{itemId}", (
            int itemId,
            SessionService session,
            [FromServices] IMarketService service,
            [FromServices] NetTradeStore store,
            HttpRequest request) =>
        {
            return HandleNetTrade(itemId, session, service, store, request, -1);
        });

        //Sell max
        app.MapPost("/market/sell-all/{itemId}", (
            int itemId,
            SessionService session,
            [FromServices] IMarketService service,
            [FromServices] NetTradeStore store,
            HttpRequest request) =>
        {
            var qty = service.GetMaxSellQuantity(session, store.Net, itemId);

            return HandleNetTrade(itemId, session, service, store, request, qty);
        });
    }

    //Queue and rerender via service.
    private static IResult HandleNetTrade(
        int itemId,
        SessionService session,
        IMarketService service,
        NetTradeStore store,
        HttpRequest request,
        int quantity)
    {
        if (quantity <= 0)
            return RenderMarket(session, service, store, request);

        store.Add(itemId, quantity);

        return RenderMarket(session, service, store, request);
    }

    // Rerender via service -> DTO -> View.
    private static IResult RenderMarket(
        SessionService session,
        IMarketService service,
        NetTradeStore store,
        HttpRequest request)
    {
        var model = service.GetMarket(session, store.Net);
        var html = MarketView.Render(model);
        return WebHelpers.Html(request, html);
    }
}
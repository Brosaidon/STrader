namespace STrader.Web.Features.Market;

using STrader.Application.Services;
using STrader.Application.Models;
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
            [FromServices] PendingActionStore store,
            [FromServices] IMarketService service,
            HttpRequest request) =>
        {
            // Use projections for UI rendering
            var model = service.GetMarket(session, store.Actions);
            return WebHelpers.Html(request, MarketView.Render(model));
        });

        app.MapPost("/market/buy/{itemId}", (
            int itemId,
            SessionService session,
            [FromServices] IMarketService service,
            [FromServices] PendingActionStore store,
            HttpRequest request) =>
        {
            return HandleQueued(itemId, session, service, store, request, ActionType.Buy, 1);
        });

        //Buy max

        app.MapPost("/market/buy-max/{itemId}", (
            int itemId,
            SessionService session,
            [FromServices] IMarketService service,
            [FromServices] PendingActionStore store,
            HttpRequest request) =>
             {
                 var qty = service.GetMaxBuyQuantity(session, store.Actions, itemId);

                 return HandleQueued(itemId, session, service, store, request, ActionType.Buy, qty);
             });

        //Sell 1
        app.MapPost("/market/sell/{itemId}", (
            int itemId,
            SessionService session,
            [FromServices] IMarketService service,
            [FromServices] PendingActionStore store,
            HttpRequest request) =>
        {
            return HandleQueued(itemId, session, service, store, request, ActionType.Sell, 1);
        });

        //Sell max
        app.MapPost("/market/sell-all/{itemId}", (
            int itemId,
            SessionService session,
            [FromServices] IMarketService service,
            [FromServices] PendingActionStore store,
            HttpRequest request) =>
        {
            var qty = service.GetMaxSellQuantity(session, store.Actions, itemId);

            return HandleQueued(itemId, session, service, store, request, ActionType.Sell, qty);
        });
    }

    //Queue and rerender via service.
    private static IResult HandleQueued(
        int itemId,
        SessionService session,
        [FromServices] IMarketService service,
        [FromServices] PendingActionStore store,
        HttpRequest request,
        ActionType type,
        int quantity)
    {
        if (quantity <= 0)
            return RenderMarket(session, service, store, request);

        service.QueueAction(session, store.Actions, new MarketActionRequest
        {
            ItemId = itemId,
            ActionType = type,
            Quantity = quantity
        });

        return RenderMarket(session, service, store, request);
    }
    // Rerender via service -> DTO -> View.
    private static IResult RenderMarket(
        SessionService session,
        IMarketService service,
        PendingActionStore store,
        HttpRequest request)
    {
        var model = service.GetMarket(session, store.Actions);
        var html = MarketView.Render(model);
        return WebHelpers.Html(request, html);
    }
}
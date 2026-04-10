namespace STrader.Web.Features.Market;

using STrader.Application.Services;
using STrader.Application.Models;
using STrader.Web.WebHelpers;
using STrader.Application.Interfaces;

public static class MarketEndpoints
{
    public static void MapMarket(this WebApplication app)
    {
        //Always get market via service.
        app.MapGet("/market", (
            SessionService session,
            PendingActionStore store,
            IMarketService service,
            HttpRequest request) =>
        {
            // Use projections for UI rendering
            var model = service.GetMarket(session, store.Actions);
            return WebHelpers.Html(request, MarketView.Render(model));
        });

        //Buy 1
        app.MapPost("/market/buy/{itemId}",
        HandleAction(ActionType.Buy, 1));

        //Buy max

        app.MapPost("/market/buy-max/{itemId}", (
            int itemId,
            IMarketService service,
            SessionService session,
            PendingActionStore store,
            HttpRequest request) =>
             {
                 var qty = ProjectionHelper.GetMaxAffordableQuantity(session, store.Actions, itemId);

                 return HandleQueued(request, service, session, store, itemId, ActionType.Buy, qty);
             });

        //Sell 1
        app.MapPost("/market/sell/{itemId}",
        HandleAction(ActionType.Sell, 1));

        //Sell max
        app.MapPost("/market/sell-all/{itemId}", (
            int itemId,
            IMarketService service,
            SessionService session,
            PendingActionStore store,
            HttpRequest request) =>
        {
            var qty = ProjectionHelper.GetMaxSellableQuantity(session, store.Actions, itemId);

            return HandleQueued(request, service, session, store, itemId, ActionType.Sell, qty);
        });
    }

    // 🔥 Generic handler
    private static Func<int, IMarketService, SessionService, PendingActionStore, HttpRequest, IResult>
        HandleAction(ActionType type, int quantity) =>
        (itemId, service, session, store, request) =>
            HandleQueued(request, service, session, store, itemId, type, quantity);

    //Queue and rerender via service.
    private static IResult HandleQueued(
        HttpRequest request,
        IMarketService service,
        SessionService session,
        PendingActionStore store,
        int itemId,
        ActionType type,
        int quantity)
    {
        if (quantity <= 0)
            return RenderMarket(request, session, store);

        service.QueueAction(session, store.Actions, new MarketActionRequest
        {
            ItemId = itemId,
            ActionType = type,
            Quantity = quantity
        });

        return RenderMarket(request, session, store);
    }
    // Rerender via service -> DTO -> View.
    private static IResult RenderMarket(
        HttpRequest request,
        IMarketService service,
        SessionService session,
        PendingActionStore store)
    {
        var model = service.GetMarket(session, store.Actions);
        var html = MarketView.Render(model);
        return WebHelpers.Html(request, html);
    }
}
namespace STrader.Web.Features.Market;

using STrader.Application.Services;
using STrader.Application.Models;
using STrader.Web.WebHelpers;
using STrader.Application.Interfaces;

public static class MarketEndpoints
{
    public static void MapMarket(this WebApplication app)
    {
        app.MapGet("/market", (SessionService session, PendingActionStore store, HttpRequest request) =>
        {
            // Use projections for UI rendering
            var html = MarketView.Render(session, store.Actions);
            return WebHelpers.Html(request, html);
        });

        app.MapPost("/market/buy/{itemId}", HandleAction(ActionType.Buy, 1));

        app.MapPost("/market/buy-max/{itemId}", (int itemId, IMarketService service, SessionService session, PendingActionStore store, HttpRequest request) =>
             {
                 var qty = ProjectionHelper.GetMaxAffordableQuantity(session, store.Actions, itemId);

                 return HandleQueued(request, service, session, store, itemId, ActionType.Buy, qty);
             });

        app.MapPost("/market/sell/{itemId}", HandleAction(ActionType.Sell, 1));

        app.MapPost("/market/sell-all/{itemId}", (int itemId, IMarketService service, SessionService session, PendingActionStore store, HttpRequest request) =>
        {
            var qty = ProjectionHelper.GetMaxSellableQuantity(session, store.Actions, itemId);

            return HandleQueued(request, service, session, store, itemId, ActionType.Sell, qty);
        });
    }
    // 🔥 Generic handler (clean)
    private static Func<int, IMarketService, SessionService, PendingActionStore, HttpRequest, IResult>
        HandleAction(ActionType type, int quantity) =>
        (itemId, service, session, store, request) =>
            HandleQueued(request, service, session, store, itemId, type, quantity);
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

    private static IResult RenderMarket(HttpRequest request, SessionService session, PendingActionStore store)
    {
        var html = MarketView.Render(session, store.Actions);
        return WebHelpers.Html(request, html);
    }
}
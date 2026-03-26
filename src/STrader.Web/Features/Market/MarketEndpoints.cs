namespace STrader.Web.Features.Market;

using STrader.Application.Services;
using STrader.Application.Models;
using STrader.Web.WebHelpers;
using STrader.Application.Interfaces;

public static class MarketEndpoints
{
    // Simulate a PendingActions service (could be injected)
    private static List<PendingAction> PendingActions = new();

    public static void MapMarket(this WebApplication app)
    {
        app.MapGet("/market", (SessionService session, HttpRequest request) =>
        {
            // Use projections for UI rendering
            var html = MarketView.Render(session, PendingActions);
            return WebHelpers.Html(request, html);
        });

        app.MapPost("/market/buy/{itemId}", (int itemId, IMarketService service, SessionService session) =>
        {
            service.QueueAction(session, PendingActions, new MarketActionRequest
            {
                ItemId = itemId,
                ActionType = ActionType.Buy,
                Quantity = 1
            });

            return Results.Redirect("/market");
        }
        );

        app.MapPost("/market/buy-max/{itemId}", (int itemId, IMarketService service, SessionService session) =>
        {
            service.QueueAction(session, PendingActions, new MarketActionRequest
            {
                ItemId = itemId,
                ActionType = ActionType.Buy,
                Quantity = ProjectionHelper.GetMaxAffordableQuantity(session, PendingActions, itemId)
            });

            return Results.Redirect("/market");
        });

        app.MapPost("/market/sell/{itemId}", (int itemId, IMarketService service, SessionService sessiond) =>
        service.QueueAction(session, PendingActions, new MarketActionRequest
        {
            ItemId = itemId,
            ActionType = ActionType.Sell,
            Quantity = 1
        });

        return Results.Redirect("/market");

        app.MapPost("/market/sell-all/{itemId}", (int itemId, SessionService session, HttpRequest request) =>
        {
            var cargoItem = session.GetCargoItem(itemId);
            if (cargoItem == null || cargoItem.Quantity <= 0) return RenderMarket(request, session);

            return QueuePendingAction(session, itemId, ActionType.Sell, cargoItem.Quantity, request);
        });
    }

    private static IResult QueuePendingAction(SessionService session, int itemId, ActionType type, int quantity, HttpRequest request)
    {
        // Only add action if quantity > 0
        if (quantity <= 0) return RenderMarket(request, session);

        PendingActions.Add(new PendingAction
        {
            ItemId = itemId,
            ActionType = type,
            Quantity = quantity
        });

        return RenderMarket(request, session);
    }

    private static IResult RenderMarket(HttpRequest request, SessionService session)
    {
        var html = MarketView.Render(session, PendingActions);
        return WebHelpers.Html(request, html);
    }
}
namespace STrader.Application.Validation;

using STrader.Application.Models;
using STrader.Application.Services;
using STrader.Domain.Entities;

public static class ActionValidator
{
    public static int ClampBuyQuantity(
        SessionService session,
        List<PendingAction> actions,
        MarketItem item,
        int requestedQuantity)
    {
        var price = item.Price;

        var credits = session.Credits
            - actions
                .Where(a => a.ActionType == ActionType.Buy)
                .Sum(a => a.Quantity * price);

        var available = item.Available
            - actions
                .Where(a => a.ItemId == item.ItemId && a.ActionType == ActionType.Buy)
                .Sum(a => a.Quantity);

        var cargoUsed = session.Cargo.Sum(c => c.Quantity)
            + actions
                .Where(a => a.ActionType == ActionType.Buy)
                .Sum(a => a.Quantity)
            - actions
                .Where(a => a.ActionType == ActionType.Sell)
                .Sum(a => a.Quantity);

        var cargoCapacity = 100;
        var cargoLeft = cargoCapacity - cargoUsed;

        var maxAffordable = price > 0 ? credits / price : 0;

        var max = Math.Min(requestedQuantity,
                  Math.Min(available,
                  Math.Min(cargoLeft, maxAffordable)));

        return Math.Max(0, max);
    }

    public static int ClampSellQuantity(
        SessionService session,
        List<PendingAction> actions,
        int itemId,
        int requestedQuantity)
    {
        var inCargo = session.Cargo
            .FirstOrDefault(c => c.ItemId == itemId)?.Quantity ?? 0;

        var pendingSell = actions
            .Where(a => a.ItemId == itemId && a.ActionType == ActionType.Sell)
            .Sum(a => a.Quantity);

        var pendingBuy = actions
            .Where(a => a.ItemId == itemId && a.ActionType == ActionType.Buy)
            .Sum(a => a.Quantity);

        var projected = inCargo + pendingBuy - pendingSell;

        var max = Math.Min(requestedQuantity, projected);

        return Math.Max(0, max);
    }
    /// <summary>
    /// Determines if a pending action can be executed given the current session state.
    /// </summary>
    public static bool CanExecute(SessionService session, List<PendingAction> pending, PendingAction action)
    {
        // Lookup market and cargo
        var marketItem = session.GetMarketItem(action.ItemId);
        var cargoItem = session.GetCargoItem(action.ItemId);
        var inCargo = cargoItem?.Quantity ?? 0;

        if (marketItem == null)
            return false; // No such item in market

        return action.ActionType switch
        {
            ActionType.Buy =>
                action.Quantity > 0 && session.Credits >= action.Quantity * marketItem.Price &&
                GetProjectedCargoUsed(session, pending, action.ItemId) + action.Quantity <= session.CargoSpace &&
                marketItem.Available >= action.Quantity,

            ActionType.Sell =>
                action.Quantity > 0 && inCargo >= action.Quantity,

            _ => true // Default: allow other action types
        };
    }

    /// <summary>
    /// Returns projected cargo including other pending buy actions for this item.
    /// </summary>
    private static int GetProjectedCargoUsed(SessionService session, List<PendingAction> pending, int itemId)
    {
        var cargoUsed = session.Cargo.Sum(c => c.Quantity);

        // Sum all other pending buy actions except the current one
        var pendingBuys = pending
            .Where(a => a.ActionType == ActionType.Buy && a.ItemId != itemId)
            .Sum(a => a.Quantity);

        return cargoUsed + pendingBuys;
    }
}
namespace STrader.Application.Services;

using STrader.Domain.Entities;
using STrader.Application.Models;

public static class ProjectionHelper
{
    /// <summary>
    ///  Calculates projected credits after all pending actions are executed.
    ///  Use prebuilt lookups for faster item access.
    /// </summary>
    public static int GetProjectedCredits(SessionService session, List<PendingAction> pendingActions)
    {
        var marketById = session.Market.ToDictionary(m => m.ItemId);

        return session.Credits - pendingActions.Sum(action =>
        {
            if (!marketById.TryGetValue(action.ItemId, out var marketItem))
                return 0; // Invalid item, ignore

            return action.CreditImpact(marketItem);
        });
    }

    /// <summary>
    /// Calculates projected cargo used after all pending actions.
    /// </summary>
    public static int GetProjectedCargoUsed(SessionService session, List<PendingAction> pendingActions)
    {
        var marketById = session.Market.ToDictionary(m => m.ItemId);

        var pendingCargo = pendingActions
            .Where(a => a.AffectsCargo())
            .Sum(a =>
            {
                if (!marketById.TryGetValue(a.ItemId, out var item))
                    return 0;
                return a.Quantity;
            });

        return Math.Max(0, session.Cargo.Sum(c => c.Quantity) + pendingCargo);
    }

    /// <summary>
    /// Calculates projected cargo left based on session capacity.
    /// </summary>
    public static int GetProjectedCargoLeft(SessionService session, List<PendingAction> pendingActions)
    {
        return session.CargoSpace - GetProjectedCargoUsed(session, pendingActions);
    }
}

/// <summary>
/// Extension methods for PendingAction to determine impacts
/// </summary>
public static class PendingActionExtensions
{
    public static bool AffectsCargo(this PendingAction action)
    {
        return action.ActionType switch
        {
            ActionType.Buy => true,
            ActionType.Sell => true,
            ActionType.Upgrade => true, // if upgrade affects cargo
            _ => false
        };
    }

    public static bool AffectsCredits(this PendingAction action)
    {
        return action.ActionType switch
        {
            ActionType.Buy => true,
            ActionType.Sell => true,
            ActionType.Hire => true,
            ActionType.Upgrade => true,
            _ => false
        };
    }

    public static int CreditImpact(this PendingAction action, MarketItem item)
    {
        return action.ActionType switch
        {
            ActionType.Buy => item.Price * action.Quantity,
            ActionType.Sell => -item.Price * action.Quantity,
            _ => 0
        };
    }
}
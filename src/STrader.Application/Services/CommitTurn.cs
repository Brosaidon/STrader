namespace STrader.Application.Services;

using STrader.Application.Models;

public static class TurnExecutor
{
    public static void ExecuteTurn(SessionService session, List<PendingAction> pendingActions)
    {
        foreach (var action in pendingActions)
        {
            ApplyAction(session, action);
        }

        // Clear actions after execution
        pendingActions.Clear();
    }

    private static void ApplyAction(SessionService session, PendingAction action)
    {
        var item = session.GetMarketItem(action.ItemId);

        switch (action.ActionType)
        {
            case ActionType.Buy:
                if (item == null) return;

                var cargo = session.GetCargoItem(action.ItemId);
                if (cargo == null)
                {
                    session.Cargo.Add(new CargoItem
                    {
                        ItemId = action.ItemId,
                        Quantity = action.Quantity
                    });
                }
                else
                {
                    cargo.Quantity += action.Quantity;
                }

                item.Available -= action.Quantity;
                session.Credits -= item.Price * action.Quantity;
                break;

            case ActionType.Sell:
                if (item == null) return;

                var cargoItem = session.GetCargoItem(action.ItemId);
                if (cargoItem == null) return;

                cargoItem.Quantity -= action.Quantity;
                item.Available += action.Quantity;
                session.Credits += item.Price * action.Quantity;
                break;
        }
    }
}
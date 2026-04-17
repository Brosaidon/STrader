namespace STrader.Application.Models;

public enum ActionType
{
    Unknown = 0,
    Buy,
    Sell,

    //Later we will have 
    Hire,
    Upgrade
}

public class pendingMarketAction
{
    public int ItemId { get; init; }
    public int Quantity { get; init; }
}

public class UserAction
{
    public ActionType ActionType { get; init; }

    //What the action targets.
    public int ItemId { get; init; }
    //How many units for this specific action.
    public int Quantity { get; init; }

    // this here is just safety.
    public bool IsValid()
    {
        return ActionType != ActionType.Unknown && Quantity > 0;
    }
}
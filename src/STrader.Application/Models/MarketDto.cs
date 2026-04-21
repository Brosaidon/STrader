namespace STrader.Application.Models;

public class MarketStateDto
{
    public int Credits { get; set; }
    public int CargoUsed { get; set; }
    public int CargoLeft { get; set; }

    public List<MarketItemDto> Items { get; set; } = new();
}

public class MarketItemDto
{
    public int ItemId { get; set; }
    public string Name { get; set; } = "";
    public string Icon { get; set; } = "";


    public int Price { get; set; }

    public int Available { get; set; }
    public int InCargo { get; set; }

    public int ProjectedAvailable { get; set; }
    public int ProjectedInCargo { get; set; }

    public int PendingNet { get; init; }


    // ✅ NEW (important)
    public int MaxBuy { get; init; }
    public int MaxSell { get; init; }

    public bool CanBuy => MaxBuy > 0;
    public bool CanSell => MaxSell > 0;
}

public class MarketActionRequest
{
    public int ItemId { get; set; }
    public ActionType ActionType { get; set; }
    public int Quantity { get; set; }
}
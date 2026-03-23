using STrader.Domain.Entities;
namespace STrader.Application.Services;

public class CargoItem
{
    public int ItemId { get; init; }
    public int Quantity { get; set; }
}

public class MarketItem
{
    public int ItemId { get; init; }
    public int Price { get; set; }
    public int Available { get; set; }
}

public class SessionService
{
    //Application state
    public List<MarketItem> Market { get; set; } = new();
    public List<CargoItem> Cargo { get; set; } = new();

    public int Credits { get; set; } = 1000; //starting credits

    //Initialize market with scommodities from the catalog.

    public void InitializeMarket()
    {
        Market = ItemCatalog.Items
            .Where(i => i.Type == ItemType.Commodity)
            .Select(i => new MarketItem
            {
                ItemId = i.Id,
                Price = i.BasePrice ?? 0,
                Available = 100 // starting stock
            })
            .ToList();
    }


    //Cargo
    public CargoItem? GetCargoItem(int itemId) =>
        Cargo.FirstOrDefault(c => c.ItemId == itemId);

    //Market
    public MarketItem? GetMarketItem(int itemId) =>
        Market.FirstOrDefault(m => m.ItemId == itemId);
}

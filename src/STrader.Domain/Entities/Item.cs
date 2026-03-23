namespace STrader.Domain.Entities;

public enum ItemType
{
    Commodity = 0,
    Artifact = 1,
    Upgrade = 2,
    Crew = 3
}

public class Item
{
    public int Id { get; init; }
    public ItemType Type { get; init; }
    public string Name { get; init; } = "";
    public string Icon { get; init; } = "";
    public bool UseCargoSpace { get; init; }

    public int? BasePrice { get; init; }
    public int? Fluctuation { get; init; }
    public int? Rarity { get; init; }
}
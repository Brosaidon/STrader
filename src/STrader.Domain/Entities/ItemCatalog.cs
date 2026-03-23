namespace STrader.Domain.Entities;

public static class ItemCatalog
{
    // ID ranges:
    // 0–9   = Basic items
    // 10–19 = Market commodities
    // 20–29 = Special items (artifacts, upgrades, crew)
    public static readonly List<Item> Items = new()
    {

        // Basics:

        new Item
        {
            Id = 0,
            Type = ItemType.Commodity,
            Name = "Fuel",
            Icon = "⛽",
            UseCargoSpace = false,
            BasePrice = 50,
            Fluctuation = 10,
            Rarity = 90
        },

        // Common market items:
        new Item
        {
            Id = 10,
            Type = ItemType.Commodity,
            Name = "Food",
            Icon = "🍎",
            UseCargoSpace = true,
            BasePrice = 100,
            Fluctuation = 20,
            Rarity = 50
        },
        new Item
        {
            Id = 11,
            Type = ItemType.Commodity,
            Name = "Ore",
            Icon = "⛏️",
            UseCargoSpace = true,
            BasePrice = 200,
            Fluctuation = 30,
            Rarity = 30
        },

        // Utilities and special items:
        new Item
        {
            Id = 20,
            Type = ItemType.Artifact,
            Name = "Ancient Relic",
            Icon = "🗿",
            UseCargoSpace = true,
            BasePrice = 1000,
            Fluctuation = 100,
            Rarity = 10
        },
        new Item
        {
            Id = 21,
            Type = ItemType.Upgrade,
            Name = "Cargo Expansion",
            Icon = "📦",
            UseCargoSpace = false,
        },
        new Item
        {
            Id = 22,
            Type = ItemType.Crew,
            Name = "Mechanic",
            Icon = "👩‍🔧",
            UseCargoSpace = false,
        }
    };


    public static readonly Dictionary<int, Item> ById =
        Items.ToDictionary(i => i.Id);

}
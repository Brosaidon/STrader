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
            BasePrice = 80,
            Fluctuation = 20,
            Rarity = 50
        },
        new Item
        {
            Id = 11,
            Type = ItemType.Commodity,
            Name = "Metal",
            Icon = "🔩",
            UseCargoSpace = true,
            BasePrice = 150,
            Fluctuation = 25,
            Rarity = 40
        },
        new Item
        {
            Id = 12,
            Type = ItemType.Commodity,
            Name = "Water",
            Icon = "💧",
            UseCargoSpace = true,
            BasePrice = 30,
            Fluctuation = 15,
            Rarity = 60
        },
        new Item
        {
            Id = 13,
            Type = ItemType.Commodity,
            Name = "Medical Supplies",
            Icon = "💊",
            UseCargoSpace = true,
            BasePrice = 500,
            Fluctuation = 40,
            Rarity = 35
        },
        new Item
        {
            Id = 14,
            Type = ItemType.Commodity,
            Name = "Industrial Parts",
            Icon = "⚙️",
            UseCargoSpace = true,
            BasePrice = 350,
            Fluctuation = 50,
            Rarity = 25
        },
        new Item
        {
            Id = 15,
            Type = ItemType.Commodity,
            Name = "Tools",
            Icon = "🛠️",
            UseCargoSpace = true,
            BasePrice = 250,
            Fluctuation = 70,
            Rarity = 20
        },
        new Item
        {
            Id = 16,
            Type = ItemType.Commodity,
            Name = "Textiles",
            Icon = "🧵",
            UseCargoSpace = true,
            BasePrice = 120,
            Fluctuation = 20,
            Rarity = 45
        },
        new Item
        {
            Id = 17,
            Type = ItemType.Commodity,
            Name = "Chemicals",
            Icon = "⚗️",
            UseCargoSpace = true,
            BasePrice = 220,
            Fluctuation = 35,
            Rarity = 30
        },
        new Item
        {
            Id = 18,
            Type = ItemType.Commodity,
            Name = "Electronics",
            Icon = "💻",
            UseCargoSpace = true,
            BasePrice = 400,
            Fluctuation = 60,
            Rarity = 20
        },
        new Item
        {
            Id = 19,
            Type = ItemType.Commodity,
            Name = "Luxury Goods",
            Icon = "💎",
            UseCargoSpace = true,
            BasePrice = 900,
            Fluctuation = 80,
            Rarity = 15
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
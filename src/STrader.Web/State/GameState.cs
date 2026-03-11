namespace STrader.Web.State;

/*
    TEMPORARY DEVELOPMENT SCAFFOLD
    ==============================

    This class intentionally violates Clean Architecture boundaries.
    It exists ONLY to allow rapid UX prototyping while the Domain,
    Application, and Infrastructure layers are not yet implemented.

    Current responsibilities (temporary):
    - Holds in-memory game state
    - Allows the Market UI to mutate cargo, credits, and stock
    - Enables HTMX interaction without persistence

    Why this exists:
    During early development we want to validate the user interaction loop:
        Market → Buy/Sell → Cargo changes → UI updates

    Implementing full Domain + Application + LiteDB at this stage would
    slow iteration and obscure UI behaviour.

    This file will be REMOVED later.

    ------------------------------------------------------------------

    FUTURE ARCHITECTURE

    When the real layers are implemented:

    Web Layer
        MarketEndpoints
            ↓
    Application Layer
        BuyCommodityUseCase
        SellCommodityUseCase
        ExecuteTravelUseCase
            ↓
    Domain Layer
        World
        Market
        Cargo
        Commodity
            ↓
    Infrastructure Layer
        LiteDbWorldRepository

    At that point this static state container will be replaced by:

        World currentWorld = repository.Load();

    All mutations will happen through Application use cases.

    ------------------------------------------------------------------

    MIGRATION PLAN

    Step 1
        Introduce Domain models:
            World
            Cargo
            Market

    Step 2
        Replace primitive fields here with a single:
            World Current

    Step 3
        Move mutation logic to Application use cases.

    Step 4
        Replace in-memory storage with LiteDB repository.

    Step 5
        Delete this class.

    ------------------------------------------------------------------
*/

public static class GameState
{
    // Player economy
    public static int Credits { get; set; } = 1000;

    // Cargo limit
    public static int CargoCapacity { get; set; } = 20;

    // Market state
    public static List<Commodity> MarketCommodities { get; } = new()
{
    new Commodity("Food", "🍎", 12, 50, 0)
};
}

public record Commodity(
    string Name,
    string Icon,
    int Price,
    int Available,
    int InCargo
    );

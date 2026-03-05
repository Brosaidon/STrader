using STrader.Web.State;

namespace STrader.Web.Features.Market;

public static class MarketEndpoints
{
    public static void MapMarket(this WebApplication app)
    {
        app.MapGet("/market", RenderMarket);

        app.MapPost("/market/buy", BuyFood);
        app.MapPost("/market/buy-max", BuyMaxFood);
        app.MapPost("/market/sell", SellFood);
        app.MapPost("/market/sell-all", SellAllFood);
    }

    private static IResult RenderMarket()
    {
        //this here must be optimesed somehow, and what about prices?
        //right now it does now check if the player has enough credits to buy max, 
        //it just checks cargo space and availability, 
        //but it should also check credits and adjust the max accordingly.
        var buyMax = Math.Min(
            GameState.FoodAvailable,
            GameState.CargoCapacity - GameState.CargoFood
        );

        return Results.Content($$"""
        <h2>Market</h2>

        <p>Credits: {{GameState.Credits}}</p>

        <table>
        <thead>
        <tr>
            <th>Name</th>
            <th>Icon</th>
            <th>Available</th>
            <th>Actions</th>
            <th>In Cargo</th>
        </tr>
        </thead>

        <tbody>
        <tr>
            <td>Food</td>
            <td>🍎</td>
            <td>{{GameState.FoodAvailable}}</td>

            <td>

            <button
                hx-post="/market/buy"
                hx-target="#content"
                hx-swap="innerHTML">
                Buy
            </button>

            <button
                hx-post="/market/buy-max"
                hx-target="#content"
                hx-swap="innerHTML">
                Buy Max
            </button>

            <button
                hx-post="/market/sell"
                hx-target="#content"
                hx-swap="innerHTML">
                Sell
            </button>

            <button
                hx-post="/market/sell-all"
                hx-target="#content"
                hx-swap="innerHTML">
                Sell All
            </button>

            </td>

            <td>{{GameState.CargoFood}}</td>
        </tr>
        </tbody>
        </table>
        """, "text/html");
    }

    private static IResult BuyFood()
    {
        if (GameState.FoodAvailable <= 0) return RenderMarket();

        if (GameState.CargoFood >= GameState.CargoCapacity) return RenderMarket();

        if (GameState.Credits < GameState.FoodPrice) return RenderMarket();

        GameState.FoodAvailable--;
        GameState.CargoFood++;
        GameState.Credits -= GameState.FoodPrice;

        return RenderMarket();
    }

    private static IResult BuyMaxFood()
    {
        var space = GameState.CargoCapacity - GameState.CargoFood;
        var maxAffordable = GameState.Credits / GameState.FoodPrice;

        var amount = Math.Min(GameState.FoodAvailable, Math.Min(space, maxAffordable));

        GameState.FoodAvailable -= amount;
        GameState.CargoFood += amount;
        GameState.Credits -= amount * GameState.FoodPrice;

        return RenderMarket();
    }

    private static IResult SellFood()
    {
        if (GameState.CargoFood <= 0) return RenderMarket();

        GameState.CargoFood--;
        GameState.FoodAvailable++;
        GameState.Credits += GameState.FoodPrice;

        return RenderMarket();
    }

    private static IResult SellAllFood()
    {
        var amount = GameState.CargoFood;

        GameState.CargoFood = 0;
        GameState.FoodAvailable += amount;
        GameState.Credits += amount * GameState.FoodPrice;

        return RenderMarket();
    }
}
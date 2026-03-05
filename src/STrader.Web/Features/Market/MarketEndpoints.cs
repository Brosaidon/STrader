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

    private static IResult RenderMarket(HttpRequest request)
    {
        //this here must be optimesed somehow, and what about prices?
        //right now it does now check if the player has enough credits to buy max, 
        //it just checks cargo space and availability, 
        //but it should also check credits and adjust the max accordingly.
        var buyMax = Math.Min(
            GameState.FoodAvailable,
            GameState.CargoCapacity - GameState.CargoFood
        );
        //NOTE: the $ here is VEEERY important, it allows us to inject the GameState values directly into the HTML string.
        var html = $"""
        <h2>Market</h2>

        <p>Credits: {GameState.Credits}</p>
        <p>Cargo: {GameState.CargoFood} / {GameState.CargoCapacity}</p>

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
            <td>{GameState.FoodAvailable}</td>

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

            <td>{GameState.CargoFood}</td>
        </tr>
        </tbody>
        </table>
        """;

        //Since this part will be used multiple times, 
        //its logical to extract it to a helper method, 
        //to avoid repeating the same code in every endpoint.
        // HTMX request → return fragment only
        if (request.Headers.ContainsKey("HX-Request"))
            return Results.Content(html, "text/html");

        // Normal request → wrap in layout
        return Results.Content(
            Layout.LayoutHtml.Page(html),
            "text/html");
    }

    private static IResult BuyFood(HttpRequest request)
    {
        if (GameState.FoodAvailable <= 0) return RenderMarket(request);

        if (GameState.CargoFood >= GameState.CargoCapacity) return RenderMarket(request);

        if (GameState.Credits < GameState.FoodPrice) return RenderMarket(request);

        GameState.FoodAvailable--;
        GameState.CargoFood++;
        GameState.Credits -= GameState.FoodPrice;

        return RenderMarket(request);
    }

    private static IResult BuyMaxFood(HttpRequest request)
    {
        var space = GameState.CargoCapacity - GameState.CargoFood;
        var maxAffordable = GameState.Credits / GameState.FoodPrice;

        var amount = Math.Min(GameState.FoodAvailable, Math.Min(space, maxAffordable));

        GameState.FoodAvailable -= amount;
        GameState.CargoFood += amount;
        GameState.Credits -= amount * GameState.FoodPrice;

        return RenderMarket(request);
    }

    private static IResult SellFood(HttpRequest request)
    {
        if (GameState.CargoFood <= 0) return RenderMarket(request);

        GameState.CargoFood--;
        GameState.FoodAvailable++;
        GameState.Credits += GameState.FoodPrice;

        return RenderMarket(request);
    }

    private static IResult SellAllFood(HttpRequest request)
    {
        var amount = GameState.CargoFood;

        GameState.CargoFood = 0;
        GameState.FoodAvailable += amount;
        GameState.Credits += amount * GameState.FoodPrice;

        return RenderMarket(request);
    }
}
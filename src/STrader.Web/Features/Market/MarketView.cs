using STrader.Web.State;

namespace STrader.Web.Features.Market;

public static class MarketView
{
    public static string Render()
    {
        var food = GameState.MarketCommodities[0];

        var buyMax = Math.Min(
            food.Available,
            GameState.CargoCapacity - food.InCargo
            );
        //@html
        var html = $"""
        <h2>Market</h2>

        <p>Credits: {GameState.Credits}</p>
        <p>Cargo: {food.InCargo} / {GameState.CargoCapacity}</p>

        <table>
        <thead>
        <tr>
            <th>{food.Name}</th>
            <th>{food.Icon}</th>
            <th>{food.Available}</th>
            <th>Actions</th>
            <th>{food.InCargo}</th>
        </tr>
        </thead>

        <tbody>
        <tr>
            <td>Food</td>
            <td>🍎</td>
            <td>{food.Available}</td>

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

            <td>{food.InCargo}</td>
        </tr>
        </tbody>
        </table>
        """;

        return html;
    }
}
namespace STrader.Web.Features.Market;

public static class MarketEndpoints
{
    public static void MapMarket(this WebApplication app)
    {
        //page endpoint for market.
        app.MapGet("/market", (HttpRequest request) =>
        {
            var html = """
            <section>
                <h2>Market</h2>

                <table>
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Icon</th>
                            <th>Available</th>
                            <th>Price</th>
                            <th colspan="4">Actions</th>
                            <th>In Cargo</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td>Food</td>
                            <td>🍎</td>
                            <td>120</td>
                            <td>120</td>

                            <td>
                                <button hx-post="/market/buy"
                                        hx-target="#market"
                                        hx-swap="outerHTML">
                                    Buy
                                </button>
                            </td>
                            <td>
                                <button hx-post="/market/buy-max"
                                        hx-target="#market"
                                        hx-swap="outerHTML">
                                    Buy Max
                                </button>
                            </td>
                            <td>
                                <button hx-post="/market/sell"
                                        hx-target="#market"
                                        hx-swap="outerHTML">
                                    Sell
                                </button>
                            </td>
                            <td>
                                <button hx-post="/market/sell-all"
                                        hx-target="#market"
                                        hx-swap="outerHTML">
                                    Sell All
                                </button>
                            </td>

                            <td>10</td>
                        </tr>
                    </tbody>
                </table>
            </section>
            """;

            if (request.Headers.ContainsKey("HX-Request"))
                return Results.Content($"<div id=\"market\">{html}</div>", "text/html");

            return Results.Content(
                Layout.LayoutHtml.Page($"<div id=\"market\">{html}</div>"),
                "text/html");
        });

        // action endpoints for market.
        app.MapPost("/market/buy", () =>
        {
            // later: Application.BuyCommodity(...)
            return Results.Redirect("/market");
        });

        app.MapPost("/market/buy-max", () =>
        {
            return Results.Redirect("/market");
        });

        app.MapPost("/market/sell", () =>
        {
            return Results.Redirect("/market");
        });

        app.MapPost("/market/sell-all", () =>
        {
            return Results.Redirect("/market");
        });
    }

}
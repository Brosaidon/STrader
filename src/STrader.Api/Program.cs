using STrader.Api.Features.Market;
using STrader.Api.Features.StarMap;
using STrader.Api.Features.Station;
using STrader.Api.Layout;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseStaticFiles();

app.MapGet("/", () =>
    Results.Content(
        LayoutHtml.Page("<p>Welcome to STrader</p>"),
        "text/html"));

app.MapMarket();
app.MapStation();
app.MapStarMap();

app.Run();
using STrader.Web.Features.Market;
using STrader.Web.Features.StarMap;
using STrader.Web.Features.Station;
using STrader.Web.Layout;

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
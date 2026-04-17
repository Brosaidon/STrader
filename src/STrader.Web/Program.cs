using STrader.Web.Features.Market;
using STrader.Web.Features.StarMap;
using STrader.Web.Features.Station;
using STrader.Web.Layout;
using STrader.Web.WebHelpers;
using STrader.Application.Services;
using STrader.Application.Interfaces;
using STrader.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<SessionService>();
builder.Services.AddSingleton<IMarketService, MarketService>();
builder.Services.AddSingleton<ISessionRepository, InMemorySessionRepository>();
builder.Services.AddSingleton<PendingActionStore>();
var app = builder.Build();

var session = app.Services.GetRequiredService<SessionService>();
session.InitializeMarket(); // now Market has default commodities

app.UseStaticFiles();

app.MapGet("/", () =>
    Results.Content(
        LayoutHtml.Page("<p>Welcome to STrader</p>"),
        "text/html"));

app.MapMarket();
app.MapStation();
app.MapStarMap();

app.Run();
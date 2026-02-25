using STrader.Api.Features.Market;
using STrader.Api.Features.StarMap;
using STrader.Api.Features.Station;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseStaticFiles();

app.MapGet("/", () =>
    Results.Content(
        """
        <!doctype html>
        <html>
        <head>
            <meta charset="utf-8">
            <title>STrader</title>
            <link rel="stylesheet" href="https://unpkg.com/@picocss/pico@latest/css/pico.min.css">
        </head>
        <body class="container">
        <header>
            <h1>STrader</h1>
        </header>
        <nav>
        <div>
            <a href="/">MARKET</a>
        </div>
        <div>
            <a href="#"
            hx-get="/station"
            hx-target="#content"
            hx-swap="innerHTML">
            STATION
            </a>
        </div>
        <div>
            <a href="#"
            hx-get="/map"
            hx-target="#content"
            hx-swap="innerHTML">
            MAP
            </a>
        </div> 
        </nav>
        
            <div id="content"></div>

            <script src="https://unpkg.com/htmx.org@1.9.10"></script>
            <footer>
                <p>STrader &copy; 2026</p>
            </footer>
        </body>
        </html>
        """,
        "text/html"));

app.MapMarket();
app.MapStation();
app.MapStarMap();

app.Run();
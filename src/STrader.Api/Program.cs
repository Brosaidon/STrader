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
            <h1>STrader</h1>
            <p>Market overview</p>

            <button hx-get="/market"
                    hx-target="#content"
                    hx-swap="innerHTML">
                Load market
            </button>

            <div id="content"></div>

            <script src="https://unpkg.com/htmx.org@1.9.10"></script>
        </body>
        </html>
        """,
        "text/html"));
app.Run();
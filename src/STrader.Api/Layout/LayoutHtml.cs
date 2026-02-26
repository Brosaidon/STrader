namespace STrader.Api.Layout;

public static class LayoutHtml
{
    public static string Page(string content)
    => $"""
        <!doctype html>
        <html>
        <head>
            <meta charset="utf-8">
            <title>STrader</title>
            <link rel="stylesheet" href="https://unpkg.com/@picocss/pico@latest/css/pico.min.css">
            <script src="https://unpkg.com/htmx.org@1.9.10"></script>
        </head>
        <body class="container">
        <header>
            <h1>STrader</h1>
        </header>

        <nav>
        <a hx-get="/market" hx-target="#content" hx-push-url="true">MARKET</a>
        <a hx-get="/station" hx-target="#content" hx-push-url="true">STATION</a>
        <a hx-get="/star-map" hx-target="#content" hx-push-url="true">STAR MAP</a>
        </nav>

            <main id="content">
           {content} 
            </main>

            <footer>
                <p>STrader &copy; 2026</p>
            </footer>
        </body>
        </html>
        """;
}
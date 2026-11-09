// Next Step C# — Web API, Part 1 — Chapter 5: a tiny web page that calls our API
// Run it in a SECOND terminal with:  dotnet run page.cs
// Then open:  http://localhost:5500  and click the button.
// It runs on port 5500, the API runs on port 5000: two different "origins".

#:sdk Microsoft.NET.Sdk.Web
#:property PublishAot=false

const string Html = """
    <!DOCTYPE html>
    <html>
    <head><title>Expense Page</title></head>
    <body style="font-family: sans-serif; padding: 24px">
      <h2>My Expenses</h2>
      <button onclick="load()">Load expenses from the API</button>
      <pre id="output"></pre>
      <script>
        async function load() {
          const output = document.getElementById("output");
          try {
            const response = await fetch("http://localhost:5000/expenses");
            const expenses = await response.json();
            output.textContent = JSON.stringify(expenses, null, 2);
          } catch (error) {
            output.textContent = "Failed: " + error.message + "\n(Press F12 and open the Console to see why)";
          }
        }
      </script>
    </body>
    </html>
    """;

var app = WebApplication.Create(args);
app.MapGet("/", () => Results.Content(Html, "text/html"));
app.Run("http://localhost:5500");

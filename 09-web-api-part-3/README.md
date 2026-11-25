# Web API, Part 3: PUT, validation and PATCH (.NET 10)

Next Step C# — free code for the video.

| Folder | What it shows |
|---|---|
| Chapter1 | A quick PUT endpoint with problems left in on purpose |
| Chapter2 | The fix with manual `if` checks |
| Chapter3 | .NET 10 built-in validation: `builder.Services.AddValidation()` + attributes |
| Chapter4 | PATCH for partial updates (PUT stays for full updates) |

Each chapter: open the folder in Visual Studio 2026 (or run `dotnet run`), then send the requests in `expenses.http`.
The database is reset on every run, so every test starts from the same 3 expenses.

Needs: .NET 10 SDK. Packages: Microsoft.EntityFrameworkCore.Sqlite 10.0.*

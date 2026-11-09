# Web API Part 1: A Whole Web API in ONE File (.NET 10 Minimal API)

Free code for the Next Step C# video **"Build a Web API in ONE File with .NET 10 (Minimal API for Beginners) | Web API Part 1"**.

| Folder | What it shows | Needs |
|---|---|---|
| [Chapter1](Chapter1/api.cs) | A whole Web API in one file: `dotnet run api.cs` | **.NET 10** |
| [Chapter2](Chapter2/Program.cs) | The same endpoint as a controller and as a Minimal API | .NET 10 |
| [Chapter3](Chapter3/Program.cs) | The expense API, first version. **Has 4 problems left in on purpose** | .NET 10 |
| [Chapter4](Chapter4/Program.cs) | All 4 fixed: 404, 201 Created, GET for total, 204/404 on delete, `MapGroup` | .NET 10 |
| [Chapter5](Chapter5/Program.cs) | CORS: let our web page (port 5500) call the API (port 5000) | .NET 10 |
| [ExpensePage](ExpensePage/page.cs) | A tiny one-file web page that calls the API (for the CORS test) | .NET 10 |
| [Chapter6](Chapter6/Program.cs) | An AI endpoint: `POST /expenses/categorize` (400 and 503 handled) | .NET 10 + OllamaSharp + Ollama |

Every chapter has a `.http` file with the requests from the video.

## Before you start

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (free)
- Visual Studio 2026, or VS Code with the REST Client extension (to send requests from `.http` files)
- Chapter 6 only: [Ollama](https://ollama.com) with `ollama pull phi3:mini`

## How to run

**Chapter 1** (one file, no project):
```
cd Chapter1
dotnet run api.cs
```
Open http://localhost:5000/expenses. To turn it into a normal project: `dotnet project convert api.cs`

**Chapters 2 to 6** (normal projects):
```
cd Chapter4
dotnet run
```
Then open `expenses.http` and click **Send request** above each request. Only one API can run at a time (they all use port 5000).

**The CORS test (Chapter 5):** run the Chapter4 API, then in a second terminal:
```
cd ExpensePage
dotnet run page.cs
```
Open http://localhost:5500, press F12 (Console) and click the button: blocked by CORS. Now run the Chapter5 API instead and click again: it works.

**Chapter 6:**
```
cd Chapter6
dotnet add package OllamaSharp
dotnet run
```

## Results on my laptop (.NET 10)

| Request | Chapter 3 (first version) | Chapter 4 (fixed) |
|---|---|---|
| GET /expenses/99 | 200 OK, body `null` | **404 Not Found** |
| POST /expenses | 200 OK, empty body | **201 Created**, with id 4 |
| GET /expenses/total | **405 Method Not Allowed** | 200 OK `{"total":373}` |
| DELETE /expenses/99 | 200 OK, nothing deleted | **404 Not Found** |
| DELETE /expenses/4 | | **204 No Content** |

Chapter 6 (phi3:mini): Pizza with friends → Food (17.14 s, first call), Uber to airport → Transport (1.42 s), Electricity bill → Bills (1.46 s). Empty description → 400. Ollama stopped → 503.

## Your challenge

Add a `PUT /expenses/{id}` endpoint to update an expense. What should it return when the id doesn't exist?

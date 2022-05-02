open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting

let builder = WebApplication.CreateBuilder()
let app = builder.Build()

app.MapGet("/", Func<string>(fun () -> "Hello World!")) |> ignore

app.Run()


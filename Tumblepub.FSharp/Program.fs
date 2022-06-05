open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Giraffe

let builder = WebApplication.CreateBuilder()
let app = builder.Build()

let webApp = choose [
    route "/" >=> text "Hello World!"
]

app.UseGiraffe webApp

app.Run()

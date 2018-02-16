namespace Spile.Example.AspNetCore

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Spile
open Spile.AspNetCore

type MySpile () =
  inherit AbstractSpile()

  override __.Call _options conn =
    let context = conn.Private.Item("aspnetcore_httpcontext") :?> HttpContext
    context.Response.StatusCode <- 201
    context.Response.WriteAsync("Hello World!") |> ignore
    Ok conn

type Startup() =
  member __.ConfigureServices(_services: IServiceCollection) =
    ()

  member __.Configure(app: IApplicationBuilder, env: IHostingEnvironment) =
    if env.IsDevelopment() then
      app.UseDeveloperExceptionPage() |> ignore

    let spiles = [
      spile MySpile defaultSpileOptions
    ]

    app.UseSpile(spiles) |> ignore

    app.Run(SpileResponse.Finalize)

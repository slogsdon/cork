namespace Cork.Example.AspNetCore

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Cork
open Cork.AspNetCore

type MyCork () =
  inherit AbstractCork()

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

    let corks = [
      cork MyCork defaultCorkOptions
    ]

    app.UseCork(corks) |> ignore

    app.Run(CorkResponse.Finalize)

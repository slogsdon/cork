namespace Cork.Example.AspNetCore

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Cork
open Cork.Connection
open Cork.AspNetCore

type MyCork () =
  inherit BaseCork()

  override __.Call _options conn =
    conn
    |> resp 200 "Hello world!"
    |> Ok

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

namespace Cork.Example.AspNetCore

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting

open Cork
open Cork.Connection
open Cork.AspNetCore

type MyCork () =
  inherit BaseCork()

  override __.Call _options conn =
    conn
    |> resp 200 "Hi There!"
    |> Ok

type Startup() =

  // This method gets called by the runtime. Use this method to add services to the container.
  // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
  member _.ConfigureServices(services: IServiceCollection) = ()

  // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
  member _.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =
    if env.IsDevelopment() then
      app.UseDeveloperExceptionPage() |> ignore

    let corks = [
      cork MyCork defaultCorkOptions
    ]

    app.UseCork(corks) |> ignore

    app.Run(CorkResponse.Finalize)

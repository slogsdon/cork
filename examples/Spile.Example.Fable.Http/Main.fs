module Spile.Example.Fable.Http.Main

open Fable.Core.JsInterop
open Fable.Import.Node
open Spile
open Spile.Fable.Http

type MySpile () =
  inherit AbstractSpile()

  override __.Call _options conn =
    let response = conn.Private.Item(connectionPrivateKey "response") :?> Http.ServerResponse
    response.writeHead 201
    response.write("Hello World!", null) |> ignore
    Ok conn

type FinalizeSpile () =
  inherit AbstractSpile()

  override __.Call _options conn =
    let response = conn.Private.Item(connectionPrivateKey "response") :?> Http.ServerResponse
    response.``end``()
    Ok conn

let spiles = [
  spile MySpile defaultSpileOptions
  spile FinalizeSpile defaultSpileOptions
]

let http: Http.IExports = importAll "http"
let server = http.createServer(useSpile(spiles))

server.listen(5000) |> ignore

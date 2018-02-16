module Cork.Example.Fable.Http.Main

open Fable.Core.JsInterop
open Fable.Import.Node
open Cork
open Cork.Fable.Http

type MyCork () =
  inherit AbstractCork()

  override __.Call _options conn =
    let response = conn.Private.Item(connectionPrivateKey "response") :?> Http.ServerResponse
    response.writeHead 201
    response.write("Hello World!", null) |> ignore
    Ok conn

type FinalizeCork () =
  inherit AbstractCork()

  override __.Call _options conn =
    let response = conn.Private.Item(connectionPrivateKey "response") :?> Http.ServerResponse
    response.``end``()
    Ok conn

let corks = [
  cork MyCork defaultCorkOptions
  cork FinalizeCork defaultCorkOptions
]

let http: Http.IExports = importAll "http"
let server = http.createServer(useCork(corks))

server.listen(5000) |> ignore

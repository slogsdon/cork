module Cork.Example.Fable.Http.Main

open Fable.Core.JsInterop
// open Fable.Node
open Cork
open Cork.Connection
open Cork.Fable.Http

type MyCork () =
  inherit BaseCork()

  override __.Call _options conn =
    conn
    |> resp 200 "Hello world!"
    |> Ok

let corks = [
  cork MyCork defaultCorkOptions
]

let http: Http.IExports = importAll "http"
let server = http.createServer(useCork(corks))

server.listen(5000) |> ignore

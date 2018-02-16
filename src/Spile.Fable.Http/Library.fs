[<AutoOpen>]
module Spile.Fable.Http.Library

open Fable.Import.Node.Http
open Spile
open Spile.Connection

[<AutoOpen>]
module Middleware =
  /// Creates the dictionary key used to store the Node.js HTTP context
  /// objects in the `Connection.Private` property.
  ///
  /// Warning: If using this to access the raw objects, changes
  /// may be overwritten once a Spile callstack is invoked.
  let connectionPrivateKey key = String.concat "" ["node_http"; key]

  /// Converts an Node.js HTTP context to a Spile client connection record.
  let connectionOfHttpContext (req: IncomingMessage, res: ServerResponse): Connection =
    { defaultConnection with
        Private =
          dict [
            connectionPrivateKey "request", req :> obj
            connectionPrivateKey "response", res :> obj
          ] }

  /// Converts a Spile result to its corresponding Node.js HTTP context.
  ///
  /// If an error occurred as a result of a thrown exception, the
  /// the exception is rethrown to allow Node.js to handle.
  let httpContextOfResult (result: Result): (IncomingMessage * ServerResponse) =
    let getRequest connection =
      connection.Private.Item(
        connectionPrivateKey "request"
      ) :?> IncomingMessage
    let getResponse connection =
      connection.Private.Item(
        connectionPrivateKey "response"
      ) :?> ServerResponse

    match result with
    | Ok connection ->
      connection |> getRequest, connection |> getResponse
    | Error error ->
      match error.Exception with
      | Some e -> raise e
      | None -> ()

      error.Connection |> getRequest, error.Connection |> getResponse

  let useSpile (spiles: (ISpile * Options) list) =
    fun (req: IncomingMessage) (res: ServerResponse) ->
      (req, res)
      |> connectionOfHttpContext
      |> run spiles
      |> httpContextOfResult
      |> ignore


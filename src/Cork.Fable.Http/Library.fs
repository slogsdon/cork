[<AutoOpen>]
module Cork.Fable.Http.Library

open System
open Fable.Core
open Node
open Node.Base
open Node.Stream

open Cork
open Cork.Connection

module rec Http =
  type [<AllowNullLiteral>] RequestOptions =
    abstract protocol: string option with get, set
    abstract host: string option with get, set
    abstract hostname: string option with get, set
    abstract family: int option with get, set
    abstract port: int option with get, set
    abstract localAddress: string option with get, set
    abstract socketPath: string option with get, set
    abstract ``method``: Methods option with get, set
    abstract path: string option with get, set
    abstract headers: obj option with get, set
    abstract auth: string option with get, set
    abstract agent: U2<Agent, bool> option with get, set

  type [<AllowNullLiteral>] Server =
    inherit Net.Server
    abstract maxHeadersCount: int with get, set
    abstract timeout: int with get, set
    abstract listening: bool with get, set
    abstract setTimeout: msecs: int * callback: (unit -> unit) -> unit

  type [<AllowNullLiteral>] ServerRequest =
    inherit IncomingMessage
    abstract connection: Net.Socket with get, set

  type [<AllowNullLiteral>] ServerResponse =
    inherit Writable<Buffer>
    abstract statusCode: int with get, set
    abstract statusMessage: string with get, set
    abstract headersSent: bool with get, set
    abstract sendDate: bool with get, set
    abstract finished: bool with get, set
    abstract write: buffer: Buffer -> bool
    abstract write: buffer: Buffer * ?cb: ('FIn->'FOut) -> bool
    abstract write: str: string * ?cb: ('FIn->'FOut) -> bool
    abstract write: str: string * ?encoding: string * ?cb: ('FIn->'FOut) -> bool
    abstract write: str: string * ?encoding: string * ?fd: string -> bool
    abstract writeContinue: unit -> unit
    abstract writeHead: statusCode: int -> unit
    abstract writeHead: statusCode: int * reasonPhrase: string * ?headers: obj -> unit
    abstract writeHead: statusCode: int * headers: obj -> unit
    abstract setHeader: name: string * value: U2<string, ResizeArray<string>> -> unit
    abstract setTimeout: msecs: int * callback: ('FIn->'FOut) -> ServerResponse
    abstract getHeader: name: string -> string
    abstract removeHeader: name: string -> unit
    abstract write: chunk: obj * ?encoding: string -> obj
    abstract addTrailers: headers: obj -> unit
    abstract ``end``: unit -> unit
    abstract ``end``: buffer: Buffer * ?cb: ('FIn->'FOut) -> unit
    abstract ``end``: str: string * ?cb: ('FIn->'FOut) -> unit
    abstract ``end``: str: string * encoding: string * ?cb: ('FIn->'FOut) -> unit

  type [<AllowNullLiteral>] ClientRequest<'a> =
    inherit Writable<'a>
    abstract abort: unit -> unit
    abstract setTimeout: timeout: int * ?callback: ('FIn->'FOut) -> unit
    abstract setNoDelay: ?noDelay: bool -> unit
    abstract setSocketKeepAlive: ?enable: bool * ?initialDelay: int -> unit
    abstract setHeader: name: string * value: string -> unit
    abstract setHeader: name: string * value: ResizeArray<string> -> unit
    abstract getHeader: name: string -> string
    abstract removeHeader: name: string -> unit
    abstract addTrailers: headers: obj -> unit

  type [<AllowNullLiteral>] IncomingMessage =
    inherit Readable<Buffer>
    abstract httpVersion: string with get, set
    abstract httpVersionMajor: int with get, set
    abstract httpVersionMinor: int with get, set
    abstract connection: Net.Socket with get, set
    abstract headers: obj with get, set
    abstract rawHeaders: ResizeArray<string> with get, set
    abstract trailers: obj with get, set
    abstract rawTrailers: obj with get, set
    abstract ``method``: string option with get, set
    abstract url: string option with get, set
    abstract statusCode: int option with get, set
    abstract statusMessage: string option with get, set
    abstract socket: Net.Socket with get, set
    abstract setTimeout: msecs: int * callback: (unit -> unit) -> Timer
    abstract destroy: ?error: Error -> unit

  type [<AllowNullLiteral>] ClientResponse =
    inherit IncomingMessage

  type [<AllowNullLiteral>] AgentOptions =
    abstract keepAlive: bool option with get, set
    abstract keepAliveMsecs: int option with get, set
    abstract maxSockets: int option with get, set
    abstract maxFreeSockets: int option with get, set

  type [<AllowNullLiteral>] Agent =
    abstract maxSockets: int with get, set
    abstract sockets: obj with get, set
    abstract requests: obj with get, set
    abstract destroy: unit -> unit

  type [<AllowNullLiteral>] AgentStatic =
    [<Emit("new $0($1)")>] abstract Create<'a> : ?opts:AgentOptions -> Agent


  type [<AllowNullLiteral>] STATUS_CODESType =
    // [<Emit("$0[$1]{{=$2}}")>] abstract Item: errorCode: int -> string with get, set
    [<Emit("$0[$1]{{=$2}}")>] abstract Item: errorCode: string -> string with get, set

  type [<StringEnum>] Methods =
    | [<CompiledName("ACL")>] Acl
    | [<CompiledName("BIND")>] Bind
    | [<CompiledName("CHECKOUT")>] Checkout
    | [<CompiledName("CONNECT")>] Connect
    | [<CompiledName("COPY")>] Copy
    | [<CompiledName("DELETE")>] Delete
    | [<CompiledName("GET")>] Get
    | [<CompiledName("HEAD")>] Head
    | [<CompiledName("LINK")>] Link
    | [<CompiledName("LOCK")>] Lock
    | [<CompiledName("M-SEARCH")>] ``M-search``
    | [<CompiledName("MERGE")>] Merge
    | [<CompiledName("MKACTIVITY")>] Mkactivity
    | [<CompiledName("MKCALENDAR")>] Mkcalendar
    | [<CompiledName("MKCOL")>] Mkcol
    | [<CompiledName("MOVE")>] Move
    | [<CompiledName("NOTIFY")>] Notify
    | [<CompiledName("OPTIONS")>] Options
    | [<CompiledName("PATCH")>] Patch
    | [<CompiledName("POST")>] Post
    | [<CompiledName("PROPFIND")>] Propfind
    | [<CompiledName("PROPPATCH")>] Proppatch
    | [<CompiledName("PURGE")>] Purge
    | [<CompiledName("PUT")>] Put
    | [<CompiledName("REBIND")>] Rebind
    | [<CompiledName("REPORT")>] Report
    | [<CompiledName("SEARCH")>] Search
    | [<CompiledName("SUBSCRIBE")>] Subscribe
    | [<CompiledName("TRACE")>] Trace
    | [<CompiledName("UNBIND")>] Unbind
    | [<CompiledName("UNLINK")>] Unlink
    | [<CompiledName("UNLOCK")>] Unlock
    | [<CompiledName("UNSUBSCRIBE")>] Unsubscribe

  type IExports =
    abstract Agent: AgentStatic with get, set
    abstract METHODS: Methods with get, set
    abstract STATUS_CODES: STATUS_CODESType with get, set
    abstract globalAgent: Agent with get, set
    abstract createServer: ?requestListener: (IncomingMessage -> ServerResponse -> unit) -> Server
    abstract createClient: ?port: int * ?host: string -> obj
    abstract request: options: RequestOptions * ?callback: (IncomingMessage -> unit) -> ClientRequest<_>
    abstract get: options: RequestOptions * ?callback: (IncomingMessage -> unit) -> ClientRequest<_>
    abstract get: options: string * ?callback: (IncomingMessage -> unit) -> ClientRequest<_>

module Response =
  let setStatus (connection: Connection) (response: Http.ServerResponse) =
    connection |> getStatus |> response.writeHead
    response

  let setResponseBody (connection: Connection) (response: Http.ServerResponse) =
    response.write(connection |> getResponseBody, null) |> ignore
    response

[<AutoOpen>]
module Middleware =
  /// Creates the dictionary key used to store the Node.js HTTP context
  /// objects in the `Connection.Private` property.
  ///
  /// Warning: If using this to access the raw objects, changes
  /// may be overwritten once a Cork callstack is invoked.
  let connectionPrivateKey key = String.concat "" ["node_http_"; key]

  /// Converts an Node.js HTTP context to a Cork client connection record.
  let connectionOfHttpContext (req: Http.IncomingMessage, res: Http.ServerResponse): Connection =
    defaultConnection
    |> putPrivate (connectionPrivateKey "request") req
    |> putPrivate (connectionPrivateKey "response") res

  /// Converts a Cork result to its corresponding Node.js HTTP context.
  ///
  /// If an error occurred as a result of a thrown exception, the
  /// the exception is rethrown to allow Node.js to handle.
  let httpContextOfResult (result: Result): (Http.IncomingMessage * Http.ServerResponse) =
    let getRequest connection =
      connection |> getPrivate(
        connectionPrivateKey "request"
      ) :?> Http.IncomingMessage
    let getResponse connection =
      connection |> getPrivate(
        connectionPrivateKey "response"
      ) :?> Http.ServerResponse

    match result with
    | Ok connection ->
      let response =
        connection
        |> getResponse
        |> Response.setStatus connection
        |> Response.setResponseBody connection

      response.``end``()

      connection |> getRequest, response

    | Error error ->
      match error.Exception with
      | Some e -> raise e
      | None -> ()

      error.Connection |> getRequest, error.Connection |> getResponse

  let useCork (corks: (BaseCork * Options) list) =
    fun (req: Http.IncomingMessage) (res: Http.ServerResponse) ->
      (req, res)
      |> connectionOfHttpContext
      |> run corks
      |> httpContextOfResult
      |> ignore

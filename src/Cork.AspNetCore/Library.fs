namespace Cork.AspNetCore

open Microsoft.AspNetCore.Http
open Cork
open Cork.Connection
open System.Threading.Tasks
open Microsoft.AspNetCore.Builder

[<AutoOpen>]
module Middleware =
  /// The dictionary key used to store the ASP.NET HTTP conext in the
  /// `Connection.Private` property.
  ///
  /// Warning: If using this to access the raw `HttpContext" object,
  /// changes may be overwritten once a Cork callstack is invoked.
  let connectionPrivateKey = "aspnetcore_httpcontext"

  /// Converts an ASP.NET HTTP context to a Cork client connection record.
  let connectionOfHttpContext (context: HttpContext): Connection =
    { defaultConnection with
        Private = dict [connectionPrivateKey, context :> obj] }

  /// Converts a Cork result to its corresponding ASP.NET HTTP context.
  ///
  /// If an error occurred as a result of a thrown exception, the
  /// the exception is rethrown to allow ASP.NET to handle.
  let httpContextOfResult (result: Result): HttpContext =
    let getContext connection =
      connection.Private.Item(connectionPrivateKey) :?> HttpContext

    match result with
    | Ok connection ->
      connection |> getContext
    | Error error ->
      match error.Exception with
      | Some e -> raise e
      | None -> ()

      error.Connection |> getContext

/// ASP.NET middleware for Cork. Allows a Cork stack to be used in conjuction with
/// a project's existing ASP.NET middleware or replace it fully.
type CorkMiddleware (next: RequestDelegate, corks: (ICork * Options) list) =
  let next = next
  let corks = corks

  /// Converts a Cork call stack to an invokable `Task` for the
  /// ASP.NET middleware stack.
  member __.Invoke(context: HttpContext): Task =
    context
    |> connectionOfHttpContext
    |> run corks
    |> httpContextOfResult
    |> next.Invoke

type CorkResponse () =
  static member Finalize = RequestDelegate(fun (context: HttpContext) ->
     context.Response.WriteAsync(""))

[<AutoOpen>]
module CorkMiddlewareExtensions =
  type IApplicationBuilder with
    member this.UseCork (corks: (ICork * Options) list) =
      this.UseMiddleware<CorkMiddleware>(corks)



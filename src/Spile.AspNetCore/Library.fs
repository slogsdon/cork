namespace Spile.AspNetCore

open Microsoft.AspNetCore.Http
open Spile
open Spile.Connection
open System.Threading.Tasks
open Microsoft.AspNetCore.Builder

[<AutoOpen>]
module Middleware =
  /// The dictionary key used to store the ASP.NET HTTP conext in the
  /// `Connection.Private` property.
  ///
  /// Warning: If using this to access the raw `HttpContext" object,
  /// changes may be overwritten once a Spile callstack is invoked.
  let connectionPrivateKey = "aspnetcore_httpcontext"

  /// Converts an ASP.NET HTTP context to a Spile client connection record.
  let connectionOfHttpContext (context: HttpContext): Connection =
    { defaultConnection with
        Private = dict [connectionPrivateKey, context :> obj] }

  /// Converts a Spile result to its corresponding ASP.NET HTTP context.
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

/// ASP.NET middleware for Spile. Allows a Spile stack to be used in conjuction with
/// a project's existing ASP.NET middleware or replace it fully.
type SpileMiddleware (next: RequestDelegate, spiles: (ISpile * Options) list) =
  let next = next
  let spiles = spiles

  /// Converts a Spile call stack to an invokable `Task` for the
  /// ASP.NET middleware stack.
  member __.Invoke(context: HttpContext): Task =
    context
    |> connectionOfHttpContext
    |> run spiles
    |> httpContextOfResult
    |> next.Invoke

type SpileResponse () =
  static member Finalize = RequestDelegate(fun (context: HttpContext) ->
     context.Response.WriteAsync(""))

[<AutoOpen>]
module SpileMiddlewareExtensions =
  type IApplicationBuilder with
    member this.UseSpile (spiles: (ISpile * Options) list) =
      this.UseMiddleware<SpileMiddleware>(spiles)



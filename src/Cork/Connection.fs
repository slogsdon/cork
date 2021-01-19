module Cork.Connection

open System
open System.Collections.Generic

/// State of a connection
type ConnectionState =
  /// Connection has not yet been modified
  | Unset
  /// Connection has been modified
  | Set
  /// Connected client has been sent a response
  | Sent

type Fetchable<'t> =
  | Unfetched
  | Fetched of 't

type Params<'t> = IDictionary<string, 't>
type Headers = (string * string) list

/// The request and response of a client connection
type Connection =
  {
    /// Dictionary for use by application developers
    Assigns: Params<obj>
    BeforeSend: (Connection -> Connection) list
    BodyParams: Fetchable<Params<obj>>
    Cookies: Fetchable<Params<string>>
    /// Server listening host
    Host: string
    /// Request HTTP method
    Method: string
    Params: Fetchable<Params<obj>>
    /// Request path split by path separators
    PathInfo: string list
    PathParams: Params<string>
    /// Server listening port
    Port: int
    /// Dictionary for use by Cork / framework developers
    Private: Params<obj>
    /// Request query string parsed from the raw string
    QueryParams: Fetchable<Params<obj>>
    /// Raw request query string
    QueryString: string
    Peer: string * int
    /// Address for the connected client
    RemoteIP: string
    RequestCookies: Fetchable<Params<string>>
    RequestHeaders: Headers
    /// Raw request path
    RequestPath: string
    ResponseBody: string
    ResponseCookies: Params<string>
    ResponseHeaders: Headers
    /// Request scheme
    Scheme: string
    /// State of the connection
    State: ConnectionState
    Status: int
  }

/// Reasons for connection errors
type ErrorReason =
  /// An exception object was thrown at runtime
  | ThrownException

/// An errored connection and its cause
type ErroredConnection =
  {
    /// Original connection that caused the error
    Connection: Connection
    /// Reason for the error
    Reason: ErrorReason
    /// Exception object, if one was thrown
    Exception: Exception option
  }

[<AutoOpen>]
module rec Helpers =
  let defaultConnection =
    {
      Assigns = Dictionary()
      BeforeSend = []
      BodyParams = Unfetched
      Cookies = Unfetched
      Host = ""
      Method = ""
      Params = Unfetched
      PathInfo = []
      PathParams = Dictionary()
      Port = 0
      Private = Dictionary()
      QueryParams = Unfetched
      QueryString = ""
      Peer = "" , 0
      RemoteIP = ""
      RequestCookies = Unfetched
      RequestHeaders = []
      RequestPath = ""
      ResponseBody = ""
      ResponseCookies = Dictionary()
      ResponseHeaders = []
      Scheme = ""
      State = Unset
      Status = 0
    }

  /// Helper function for creating `ErroredConnection` values
  let error conn reason exc =
    { Connection = conn
      Reason = reason
      Exception = exc }

  let getStatus conn =
    conn.Status

  let putStatus status conn =
    { conn with Status = status }

  let getResponseBody conn =
    conn.ResponseBody

  let resp status body conn =
    { conn with
        Status = status
        ResponseBody = body }

  // params

  let getPrivate key conn =
    conn.Private.Item(key)

  let putPrivate key data conn =
    let priv = conn.Private

    if priv.ContainsKey(key) then
      priv.Remove(key) |> ignore

    priv.Add(key, data)
    { conn with Private = priv }

  let deletePrivate (key: string) conn =
    let priv = conn.Private
    priv.Remove(key) |> ignore
    { conn with Private = priv }

  let getAssigns key conn =
    conn.Assigns.Item(key)

  let putAssigns key data conn =
    let assigns = conn.Assigns
    assigns.Add(key, data)
    { conn with Assigns = assigns }

  let deleteAssigns (key: string) conn =
    let assigns = conn.Assigns
    assigns.Remove(key) |> ignore
    { conn with Assigns = assigns }

  let getPathParam key conn =
    conn.PathParams.Item(key)

  let putPathParam key data conn =
    let pathParams = conn.PathParams
    pathParams.Add(key, data)
    { conn with PathParams = pathParams }

  let deletePathParam (key: string) conn =
    let pathParams = conn.PathParams
    pathParams.Remove(key) |> ignore
    { conn with PathParams = pathParams }

  // fetchable params

  let private getFetchableParam (key: string) (name: string) (fetchable: Fetchable<Params<'a>>) =
    match fetchable with
    | Fetched prms ->
      prms.Item(key)
    | Unfetched ->
      raise <| Exception(name + " have not yet been fetched")

  let fetchBodyParams conn =
    // TODO: get real data
    let bodyParams = dict []
    { conn with BodyParams = Fetched bodyParams }

  let getBodyParam key conn =
    getFetchableParam key "BodyParams" conn.BodyParams

  let fetchCookies conn =
    // TODO: get real data
    let cookies = dict []
    { conn with Cookies = Fetched cookies }

  let getCookie key conn =
    getFetchableParam key "Cookies" conn.Cookies

  let fetchParams conn =
    // TODO: get real data
    let prms = dict []
    { conn with Params = Fetched prms }

  let getParam key conn =
    getFetchableParam key "Params" conn.Params

  let fetchQueryParams conn =
    // TODO: get real data
    let queryParams = dict []
    { conn with QueryParams = Fetched queryParams }

  let getQueryParam key conn =
    getFetchableParam key "QueryParams" conn.QueryParams

  let fetchRequestCookies conn =
    // TODO: get real data
    let requestCookies = dict []
    { conn with RequestCookies = Fetched requestCookies }

  let getRequestCookie key conn =
    getFetchableParam key "RequestCookies" conn.RequestCookies

  let fetchAllParams conn =
    conn
    |> fetchBodyParams
    |> fetchCookies
    |> fetchParams
    |> fetchQueryParams
    |> fetchRequestCookies

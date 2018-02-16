module Spile.Connection

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
    /// Dictionary for use by Spile / framework developers
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
    ResponseCookies: Fetchable<Params<string>>
    ResponseHeaders: Headers
    /// Request scheme
    Scheme: string
    /// State of the connection
    State: ConnectionState
    Status: int
  }

let defaultConnection =
  {
    Assigns = dict []
    BeforeSend = []
    BodyParams = Unfetched
    Cookies = Unfetched
    Host = ""
    Method = ""
    Params = Unfetched
    PathInfo = []
    PathParams = dict []
    Port = 0
    Private = dict []
    QueryParams = Unfetched
    QueryString = ""
    Peer = "" , 0
    RemoteIP = ""
    RequestCookies = Unfetched
    RequestHeaders = []
    RequestPath = ""
    ResponseBody = ""
    ResponseCookies = Unfetched
    ResponseHeaders = []
    Scheme = ""
    State = Unset
    Status = 0
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

/// Helper function for creating `ErroredConnection` values
let error conn reason exc =
  { Connection = conn
    Reason = reason
    Exception = exc }

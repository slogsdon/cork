namespace Cork

open Cork.Connection
open System.Collections.Generic

/// Result type alias
type Result = Result<Connection, ErroredConnection>

/// Dictionary type alias
type Options = IDictionary<string, obj>

/// The Cork contract
type ICork =
  /// Prepares options for a Cork before being passed to `ICork.Call`.
  /// Useful for Cork implementors to modify options passed in by
  /// third-party developers.
  abstract Init: Options -> Options
  /// Accepts a set of options and filters and/or modifies a current
  /// client connection.
  abstract Call: Options -> Connection -> Result

[<AutoOpen>]
module Library =
  /// Helper for building Cork lists, handling the cast to ICork for
  /// compatible sub-types.
  let cork (corkType: unit -> 'T when 'T :> ICork) (opts: Options) =
    corkType() :> ICork, opts

  /// A set of empty Cork options.
  let defaultCorkOptions: Options = dict []
  /// A basic init function for Corks to use in their implementations.
  let defaultCorkInit (options: Options): Options = options
  /// A basic call function for Corks to use in their implementations.
  let defaultCorkCall (_: Options) (conn: Connection): Result = Ok conn

  /// Runs a connection over a list of configured corks.
  let run (corks: (ICork * Options) list) (conn: Connection): Result =
    let asBindable (corkWithOptions: ICork * Options): Connection -> Result =
      let cork = fst corkWithOptions
      let options = snd corkWithOptions

      fun c ->
        c |> cork.Call (cork.Init options)

    // QUESTION: Expose this externally?
    let bind (fn: Connection -> Result) (result: Result): Result =
      match result with
        | Ok conn ->
          // wrap `fn` call in try/with expression in case exception is thrown
          // QUESTION: is this desired?
          try
            fn conn
          with
            | exc -> Error (error conn ThrownException <| Some exc)
        | Error ec -> Error ec

    let flip f a b = f b a

    corks
    |> List.map asBindable
    |> List.fold (flip bind) (Ok conn)

[<AbstractClass>]
type BaseCork() =
  interface ICork with
    member this.Init options = this.Init options
    member this.Call options conn = this.Call options conn

  /// Prepares options for a Cork before being passed to `ICork.Call`.
  /// Useful for Cork implementors to modify options passed in by
  /// third-party developers.
  abstract Init: Options -> Options
  /// Accepts a set of options and filters and/or modifies a current
  /// client connection.
  abstract Call: Options -> Connection -> Result

  default __.Init options = defaultCorkInit options
  default __.Call options conn = defaultCorkCall options conn

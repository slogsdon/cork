namespace Spile

open Spile.Connection
open System.Collections.Generic

/// Result type alias
type Result = Result<Connection, ErroredConnection>

/// Dictionary type alias
type Options = IDictionary<string, obj>

/// The Spile contract
type ISpile =
  /// Prepares options for a Spile before being passed to `ISpile.Call`.
  /// Useful for Spile implementors to modify options passed in by
  /// third-party developers.
  abstract Init: Options -> Options
  /// Accepts a set of options and filters and/or modifies a current
  /// client connection.
  abstract Call: Options -> Connection -> Result

[<AutoOpen>]
module Library =
  /// Helper for building Spile lists, handling the cast to ISpile for
  /// compatible sub-types.
  let spile (spileType: unit -> 'T when 'T :> ISpile) (opts: Options) =
    spileType() :> ISpile, opts

  /// A set of empty Spile options.
  let defaultSpileOptions: Options = dict []
  /// A basic init function for Spiles to use in their implementations.
  let defaultSpileInit (options: Options): Options = options
  /// A basic call function for Spiles to use in their implementations.
  let defaultSpileCall (_: Options) (conn: Connection): Result = Ok conn

  /// Runs a connection over a list of configured spiles.
  let run (spiles: (ISpile * Options) list) (conn: Connection): Result =
    let asBindable (spileWithOptions: ISpile * Options): Connection -> Result =
      let spile = fst spileWithOptions
      let options = snd spileWithOptions

      fun c ->
        c |> spile.Call (spile.Init options)

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

    spiles
    |> List.map asBindable
    |> List.fold (flip bind) (Ok conn)

[<AbstractClass>]
type AbstractSpile() =
  interface ISpile with
    member this.Init options = this.Init options
    member this.Call options conn = this.Call options conn

  /// Prepares options for a Spile before being passed to `ISpile.Call`.
  /// Useful for Spile implementors to modify options passed in by
  /// third-party developers.
  abstract Init: Options -> Options
  /// Accepts a set of options and filters and/or modifies a current
  /// client connection.
  abstract Call: Options -> Connection -> Result

  default __.Init options = defaultSpileInit options
  default __.Call options conn = defaultSpileCall options conn

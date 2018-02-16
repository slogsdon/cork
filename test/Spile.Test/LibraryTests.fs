module Spile.Test.LibraryTests

open Microsoft.VisualStudio.TestTools.UnitTesting

open Spile
open Spile.Connection

let assertOkResult result =
  match result with
  | Ok _ -> Assert.IsTrue(true)
  | Error _ -> Assert.Fail()

// new class type extending abstract
type OkSpile () =
  inherit AbstractSpile()

// new object expression implementing interface
let TestSpile () =
  { new ISpile with
      member __.Init options = options
      member __.Call _ conn = Ok conn }

[<TestClass>]
type TestClass () =

  [<TestMethod>]
  member __.TestMethodPassing () =
    let conn = defaultConnection

    let spiles = [
      spile OkSpile defaultSpileOptions
      spile TestSpile defaultSpileOptions
    ]

    conn
    |> run spiles
    |> assertOkResult

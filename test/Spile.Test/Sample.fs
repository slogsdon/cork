module Spile.Test.LibraryTests

open Expecto
open Spile
open Spile.Connection

let expectOkResult result = Expect.isOk result "Result not ok"

// new class type extending abstract
type OkSpile () =
  inherit AbstractSpile()

// new object expression implementing interface
let TestSpile () =
  { new ISpile with
      member __.Init options = options
      member __.Call _ conn = Ok conn }

[<Tests>]
let classTests =
  testList "SpileAbstract" [
    testCase "can run spiles" <| fun _ ->
      let conn = defaultConnection

      let spiles : (ISpile * Options) list = [
        OkSpile() :> ISpile, defaultSpileOptions
        TestSpile(), defaultSpileOptions
      ]

      conn
      |> run spiles
      |> expectOkResult
  ]

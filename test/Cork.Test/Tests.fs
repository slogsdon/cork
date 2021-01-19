namespace Cork.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting

open Cork
open Cork.Connection

[<AutoOpen>]
module Helpers =

  let assertOkResult result =
    match result with
    | Ok _ -> Assert.IsTrue(true)
    | Error _ -> Assert.Fail()

  // new class type extending abstract
  type OkCork () =
    inherit BaseCork()

  // new object expression implementing interface
  type TestCork () =
    inherit BaseCork()
    override __.Init options = options
    override __.Call _ conn = Ok conn

[<TestClass>]
type TestClass () =

  [<TestMethod>]
  member __.TestMethodPassing () =
    let conn = defaultConnection

    let corks = [
      cork OkCork defaultCorkOptions
      cork TestCork defaultCorkOptions
    ]

    conn
    |> run corks
    |> assertOkResult

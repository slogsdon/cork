module Cork.Test.LibraryTests

open Microsoft.VisualStudio.TestTools.UnitTesting

open Cork
open Cork.Connection

let assertOkResult result =
  match result with
  | Ok _ -> Assert.IsTrue(true)
  | Error _ -> Assert.Fail()

// new class type extending abstract
type OkCork () =
  inherit AbstractCork()

// new object expression implementing interface
let TestCork () =
  { new ICork with
      member __.Init options = options
      member __.Call _ conn = Ok conn }

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

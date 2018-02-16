module Cork.Fable.Http.Test.LibraryTests

open Fable.Core
// open Cork.Fable.Http
open Fable.Core.Testing

[<Global>]
let it (_msg: string) (f: unit->unit): unit = jsNative

it "Adding works" <| fun () ->
    let expected = 3
    let actual = 1 + 2
    Assert.AreEqual(expected, actual)

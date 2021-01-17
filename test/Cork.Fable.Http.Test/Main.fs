module Cork.Fable.Http.Test.Main

open Fable.Core.JsInterop

// This is necessary to make webpack collect all test files
importSideEffects "./LibraryTests.fs"

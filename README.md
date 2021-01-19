# Cork

F# port of [Elixir's Plug library](https://github.com/elixir-lang/plug)

## Projects

- [Cork](src/Cork)
  - Core library
  - Targets `netstandard1.6` to match F# (as of 15 Feb 2018)
- [Cork.AspNetCore](src/Cork.AspNetCore)
  - Library for using Cork in an ASP.NET Core project with included ASP.NET middleware and helpers to aid in project development
  - Targets `netstandard2.0` to match Microsoft.AspNetCore (as of 15 Feb 2018)
- [Cork.Fable.Http](src/Cork.Fable.Http)
  - Library for using Cork in a Node.js project which uses the built-in HTTP server with included helpers to aid in project development
  - Targets `netstandard2.0` to match Fable.Import.Node (as of 16 Feb 2018)

## Installation

Add a Cork package to your project via `dotnet add package`, `paket add`, etc. For example:

```
dotnet add package Cork
```

## Getting Started

```fsharp
open Cork
open Cork.Connection

// new class type extending abstract
type MyCork () =
  inherit BaseCork()

  override __.Call _options conn =
    conn
    |> resp 200 "Hello world!"
    |> Ok

let corks = [
  cork MyCork defaultCorkOptions
  cork TestCork defaultCorkOptions
]

// this should be updated with real connection information
let conn = Connection.defaultConnection

conn |> run corks
```

## Examples

For a full end-to-end demonstration, look at one of the below example projects:

- [Cork.Example.AspNetCore](examples/Cork.Example.AspNetCore) - Minimal ASP.NET Core project using Cork, created with `dotnet new web -lang f#` and modified to use Cork.AspNetCore.
- [Cork.Example.Fable.Http](examples/Cork.Example.Fable.Http) - Minimal Node.js project using Cork, created with `dotnet new fable-library -lang f#` and modified to use Cork.Fable.Http.

## TODO

- Convert Makefile to FAKE script
- Expand ASP.NET middleware to use more than HttpContext API
  - Create Connection helpers
  - Map data between Connection and HttpContext
- Test existing code
- Finish documentation for existing code
- Publish to Nuget?
- Support Suave?
- Enhance C# interop
- Investigate if a computation expression would be beneficial to offer. Potentially:

  ```fsharp
  let corks =
    cork_builder {
      cork CorkName, options
      cork AnotherCork
    }
  ```

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for more details.

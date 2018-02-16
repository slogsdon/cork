# Spile

F# port of [Elixir's Plug library](https://github.com/elixir-lang/plug)

## Projects

- [Spile](src/Spile)
  - Core library
  - Targets `netstandard1.6` to match F# (as of 15 Feb 2018)
- [Spile.AspNetCore](src/Spile.AspNetCore)
  - Library for using Spile in an ASP.NET Core project with included ASP.NET middleware and helpers to aid in project development
  - Targets `netstandard2.0` to match Microsoft.AspNetCore (as of 15 Feb 2018)
- [Spile.Fable.Http](src/Spile.Fable.Http)
  - Library for using Spile in a Node.js project which uses the built-in HTTP server with included helpers to aid in project development
  - Targets `netstandard2.0` to match Fable.Import.Node (as of 16 Feb 2018)

## Installation

Add a Spile package to your project via `dotnet add package`, `paket add`, etc. For example:

```
dotnet add package Spile
```

## Getting Started

```fsharp
open Spile

// new class type extending abstract
type MySpile () =
  inherit AbstractSpile()

  override __.Call _options conn =
    Ok { conn with
          Status = 201
          ResponseBody = "Hello World!"
       }

// new object expression implementing interface
let TestSpile () =
  { new ISpile with
      member __.Init options = options
      member __.Call _ conn = Ok conn }

let spiles = [
  MySpile() :> ISpile, defaultSpileOptions
  TestSpile(), defaultSpileOptions
]

// this should be updated with real connection information
let conn = Connection.defaultConnection

conn |> run spiles
```

## Examples

For a full end-to-end demonstration, look at one of the below example projects:

- [Spile.Example.AspNetCore](examples/Spile.Example.AspNetCore) - Minimal ASP.NET Core project using Spile, created with `dotnet new web -lang f#` and modified to use Spile.AspNetCore.
- [Spile.Example.Fable.Http](examples/Spile.Example.Fable.Http) - Minimal Node.js project using Spile, created with `dotnet new fable-library -lang f#` and modified to use Spile.Fable.Http.

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
  let spiles =
    spile_builder {
      spile SpileName, options
      spile AnotherSpile
    }
  ```

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for more details.

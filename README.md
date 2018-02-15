# Spile

F# port of [Elixir's Plug library](https://github.com/elixir-lang/plug)

## Projects

- [Spile](src/Spile) - Core library
- [Spile.AspNetCore](src/Spile.AspNetCore) - Entry for using Spile in an ASP.NET Core project with included ASP.NET middleware and helpers to aid in project development

## Examples

- [Spile.Example.AspNetCore](examples/Spile.Example.AspNetCore) - Minimal ASP.NET Core project using Spile, created with `dotnet new web -lang f#` and modified to use Spile.AspNetCore.

## TODO

- Use Paket
- Convert Makefile to FAKE script
- Expand ASP.NET middleware to use more than HttpContext API
  - Create Connection helpers
  - Map data between Connection and HttpContext
- Test existing code
- Finish documentation for existing code
- Support Node.js HTTP(S) via Fable
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

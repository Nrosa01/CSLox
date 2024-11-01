#CSLox

.NET 8.0 C# direct port of [jlox](https://github.com/munificent/craftinginterpreters/tree/master/java/com/craftinginterpreters/lox) from [crafting interpreters book](https://craftinginterpreters.com/).
Right now it doesn't have almost anything different from the book version as I wanted to learn the basics. Once I finish with the bytecode interpreter I might return to this one to complete the challenges.

Implementation details

- As C# doesn't support Void in generics like Java, object? is used instead
- C# can't implement an anonymous interface like a lamabda like Java, so for the clock native function, a NativeFunction class has been created. It's currently only used for clock but it could be use for any other inline native function declaration in the interpreter class
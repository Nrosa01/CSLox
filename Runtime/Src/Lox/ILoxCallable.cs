﻿namespace CSLox.Src.Lox
{
    internal interface ILoxCallable
    {
        int Arity { get; }

        object? call(Interpreter interpreter, List<object?> arguments);
    }
}
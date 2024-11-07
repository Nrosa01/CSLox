using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSLox.Src.Lox
{
    internal class LoxFunction(Stmt.Function declaration, Environment closure, bool isInitializer) : ILoxCallable
    {
        public int Arity => declaration.@params.Count;

        public override string ToString() => $"<fn {declaration.name.lexeme}>";

        public object? Call(Interpreter interpreter, List<object?> arguments)
        {
            Environment environment = new Environment(closure);
            for (int i = 0; i < declaration.@params.Count; i++)
                environment.Define(declaration.@params[i].lexeme, arguments[i]);

            try
            {
                interpreter.ExecuteBlock(declaration.body, environment);
            }
            catch (Return returnValue)
            {
                if (isInitializer) return closure.GetAt(0, "this");
                return returnValue.Value;
            }

            if (isInitializer) return closure.GetAt(0, "this");
            return null;
        }

        internal LoxFunction? Bind(LoxInstance instance)
        {
            Environment environment = new Environment(closure);
            environment.Define("this", instance);
            return new LoxFunction(declaration, environment, isInitializer);
        }
    }
}

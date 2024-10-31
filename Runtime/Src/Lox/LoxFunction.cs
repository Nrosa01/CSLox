using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSLox.Src.Lox
{
    internal class LoxFunction(Stmt.Function declaration, Environment closure) : ILoxCallable
    {
        public int Arity => declaration.@params.Count;

        public override string ToString() => $"<fn {declaration.name.lexeme}>";

        public object? call(Interpreter interpreter, List<object?> arguments)
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
                return returnValue.Value;
            }

            return null;
        }
    }
}



namespace CSLox.Src.Lox
{
    internal class NativeFunction : ILoxCallable
    {
        private int arity;
        private readonly Func<object, object, double> func;

        public NativeFunction(int arity, Func<object, object, double> func)
        {
            this.arity = arity;
            this.func = func;
        }

        public int Arity => arity;

        public object? call(Interpreter interpreter, List<object?> arguments) => func.Invoke(interpreter, arguments);

        public override string ToString() => "<native fn>";
    }
}
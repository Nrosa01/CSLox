

namespace CSLox.Src.Lox
{
    internal class LoxClass : ILoxCallable
    {
        internal readonly string name;
        internal readonly Dictionary<string, LoxFunction> methods;

        public LoxClass(string lexeme, Dictionary<string, LoxFunction> methods)
        {
            this.name = lexeme;
            this.methods = methods;
        }

        public int Arity
        {
            get
            {
                LoxFunction? initializer = FindMethod("init");
                return initializer?.Arity ?? 0;
            }
        }

        public object? Call(Interpreter interpreter, List<object?> arguments)
        {
            LoxInstance instance = new(this);
            LoxFunction? initializer = FindMethod("init");
            initializer?.Bind(instance)?.Call(interpreter, arguments);

            return instance;
        }

        public override string ToString() => name;

        internal LoxFunction? FindMethod(string name) => methods.GetValueOrDefault(name);
    }
}
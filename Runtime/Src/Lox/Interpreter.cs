using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSLox.Src.Lox
{
    internal class Interpreter : Expr.IVisitor<object?>, Stmt.IVisitor<object?>
    {
        internal readonly Dictionary<Expr, int> locals = [];
        internal readonly Environment globals = new();
        private Environment environment;

        public Interpreter()
        {
            globals.Define("clock", new NativeFunction(0, (interpreter, arguments) => { 
                return DateTimeOffset.Now.ToUnixTimeMilliseconds() / 1000.0;
            }));

            environment = globals;
        }


        public object? VisitBinaryExpr(Expr.Binary expr)
        {
            object? left = Evaluate(expr.left);
            object? right = Evaluate(expr.right);

            switch (expr.@operator.type)
            {
                case TokenType.MINUS:
                    CheckNumberOperands(expr.@operator, left, right);
                    return (double?)left - (double?)right;
                case TokenType.SLASH:
                    CheckNumberOperands(expr.@operator, left, right);
                    return (double?)left / (double?)right;
                case TokenType.STAR:
                    CheckNumberOperands(expr.@operator, left, right);
                    return (double?)left * (double?)right;
                case TokenType.PLUS:
                    if (left is double && right is double)
                        return (double?)left + (double?)right;
                    if (left is string && right is string)
                        return (string?)left + (string?)right;

                    throw new RuntimeError(expr.@operator, "Operands must be two numbers or two strings."); ;
                case TokenType.GREATER:
                    CheckNumberOperands(expr.@operator, left, right);
                    return (double?)left > (double?)right;
                case TokenType.GREATER_EQUAL:
                    CheckNumberOperands(expr.@operator, left, right);
                    return (double?)left >= (double?)right;
                case TokenType.LESS:
                    CheckNumberOperands(expr.@operator, left, right);
                    return (double?)left < (double?)right;
                case TokenType.LESS_EQUAL:
                    CheckNumberOperands(expr.@operator, left, right);
                    return (double?)left <= (double?)right;
                case TokenType.BANG_EQUAL:
                    return !IsEqual(left, right);
                case TokenType.EQUAL_EQUAL:
                    return IsEqual(left, right);

                default:
                    return null;
            }
        }

        public void Interpret(List<Stmt> statements)
        {
            try
            {
                foreach (Stmt statement in statements)
                    Execute(statement);
            }
            catch (RuntimeError error)
            {
                Lox.RuntimeError(error);
            }
        }

        private void Execute(Stmt stmt)
        {
            stmt.Accept(this);
        }

        private string? Stringify(object? value)
        {
            if (value == null) return "nil";

            if (value is double)
            {
                string? text = value?.ToString();
                if (text?.EndsWith(".0") ?? false)
                    text = text[0..(text.Length - 2)];

                return text;
            }

            return value?.ToString();
        }

        private bool? IsEqual(object? left, object? right)
        {
            if (left == null && right == null) return true;
            if (left == null) return false;

            return left.Equals(right);
        }

        public object? VisitGroupingExpr(Expr.Grouping expr) => Evaluate(expr.expression);

        private object? Evaluate(Expr expression) => expression.Accept(this);

        public object? VisitLiteralExpr(Expr.Literal expr) => expr.value;

        public object? VisitUnaryExpr(Expr.Unary expr)
        {
            object? right = Evaluate(expr.right);

            switch (expr.@operator.type)
            {

                case TokenType.MINUS:
                    {
                        CheckNumberOperand(expr.@operator, right);
                        return -(double?)right;
                    }
                case TokenType.BANG: return !IsTruthy(right);
                default:
                    return null;
            }


        }
        private void CheckNumberOperand(Token @operator, object? operand)
        {
            if (operand is double && operand is not null) return;
            throw new RuntimeError(@operator, "Operand must be a number.");
        }

        private void CheckNumberOperands(Token @operator, object? left, object? right)
        {
            if (left is double && left is not null && right is double && right is not null) return;
            throw new RuntimeError(@operator, "Operand must be a number.");
        }

        private bool IsTruthy(object? right)
        {
            if (right == null) return false;
            if (right is bool) return (bool)right;
            return true;
        }

        public object? VisitExpressionStmt(Stmt.Expression stmt)
        {
            Evaluate(stmt.expression);
            return null;
        }

        public object? VisitPrintStmt(Stmt.Print stmt)
        {
            object? value = Evaluate(stmt.expression);
            Console.WriteLine(Stringify(value));
            return null;
        }

        public object? VisitVariableExpr(Expr.Variable expr) => LookUpVariable(expr.name, expr);

        private object? LookUpVariable(Token name, Expr expr)
        {
            if (locals.TryGetValue(expr, out int distance))
                return environment.GetAt(distance, name.lexeme);
            else
                return globals.Get(name)    ;
        }

        public object? VisitVarStmt(Stmt.Var stmt)
        {
            object? value = null;
            if (stmt.initializer != null)
                value = Evaluate(stmt.initializer);

            environment.Define(stmt.name.lexeme, value);
            return null;
        }

        public object? VisitAssignExpr(Expr.Assign expr)
        {
            object? value = Evaluate(expr.value);

            if (locals.TryGetValue(expr, out int distance))
                environment.AssignAt(distance, expr.name, value);
            else
                globals.Assign(expr.name, value);

            return value;
        }

        public object? VisitBlockStmt(Stmt.Block stmt)
        {
            ExecuteBlock(stmt.statements, new Environment(environment));
            return null;
        }

        internal void ExecuteBlock(List<Stmt> statements, Environment environment)
        {
            Environment previous = this.environment;
            try
            {
                this.environment = environment;

                foreach (Stmt statement in statements)
                    Execute(statement);
            }
            finally
            {
                this.environment = previous;
            }
        }

        public object? VisitIfStmt(Stmt.If stmt)
        {
            if (IsTruthy(Evaluate(stmt.condition)))
                Execute(stmt.thenBranch);
            else if (stmt.elseBranch != null)
                Execute(stmt.elseBranch);
            return null;
        }

        public object? VisitLogicalExpr(Expr.Logical expr)
        {
            object? left = Evaluate(expr.left);

            if (expr.@operator.type == TokenType.OR)
            {
                if (IsTruthy(left)) return left;
            }
            else
            {
                if (!IsTruthy(left)) return left;
            }

            return Evaluate(expr.right);
        }

        public object? VisitWhileStmt(Stmt.While stmt)
        {
            while (IsTruthy(Evaluate(stmt.condition)))
                Execute(stmt.body);

            return null;
        }

        public object? VisitCallExpr(Expr.Call expr)
        {
            object? callee = Evaluate(expr.callee);

            List<object?> arguments = [];
            foreach (Expr argument in expr.arguments)
                arguments.Add(Evaluate(argument));

            if (callee is ILoxCallable func)
            {
                if (arguments.Count != func.Arity)
                    throw new RuntimeError(expr.paren, $"Expected {func.Arity} arguments but got {arguments.Count}.");

                return func.Call(this, arguments);
            }
            else
            {
                throw new RuntimeError(expr.paren, "Can only call functions and classes.");
            }
        }

        public object? VisitFunctionStmt(Stmt.Function stmt)
        {
            LoxFunction function = new LoxFunction(stmt, environment, false);
            environment.Define(stmt.name.lexeme, function);
            return null;
        }

        public object? VisitReturnStmt(Stmt.Return stmt)
        {
            object? value = null;
            if (stmt.value != null) value = Evaluate(stmt.value);

            throw new Return(value);
        }

        internal void Resolve(Expr expr, int depth)
        {
            locals[expr] = depth;
        }

        public object? VisitClassStmt(Stmt.Class stmt)
        {
            environment.Define(stmt.name.lexeme, null);

            Dictionary<string, LoxFunction> methods = [];
            foreach (Stmt.Function method in stmt.methods)
            {
                LoxFunction function = new LoxFunction(method, environment, method.name.lexeme.Equals("init"));
                methods[method.name.lexeme] = function;
            }

            LoxClass klass = new LoxClass(stmt.name.lexeme, methods);
            environment.Assign(stmt.name, klass);
            return null;
        }

        public object? VisitGetExpr(Expr.Get expr)
        {
            object? obj = Evaluate(expr.@object);
            if (obj is LoxInstance)
                return ((LoxInstance)obj).Get(expr.name);

            throw new RuntimeError(expr.name, "Only instances have properties");
        }

        public object? VisitSetExpr(Expr.Set expr)
        {
            object? obj = Evaluate(expr.@object);
            if (obj is not LoxInstance) throw new RuntimeError(expr.name, "Only instances have fields");

            object? value = Evaluate(expr.value);
            ((LoxInstance)obj).Set(expr.name, value);
            return value;
        }

        public object? VisitThisExpr(Expr.This expr)
        {
            return LookUpVariable(expr.keyword, expr);
        }
    }
}

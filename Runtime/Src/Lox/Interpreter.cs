using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSLox.Src.Lox
{
    internal class Interpreter : Expr.IVisitor<object?>
    {
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

        public void Interpret(Expr? expression)
        {
            try
            {
                Object? value = Evaluate(expression);
                Console.WriteLine(Stringify(value));
            }
            catch (RuntimeError error)
            {
                Lox.RuntimeError(error);
            }
        }

        private string? Stringify(object? value)
        {
            if (value == null) return "nil";
                
            if (value is double) {
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
    }
}

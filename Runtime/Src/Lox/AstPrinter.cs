using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSLox.Src.Lox
{
    internal class AstPrinter : Expr.IVisitor<string>
    {
        public string Print(Expr expr) => expr.Accept(this);

        public string VisitBinaryExpr(Expr.Binary expr) => Parenthesize(expr.@operator.lexeme, expr.left, expr.right);

        public string VisitGroupingExpr(Expr.Grouping expr) => Parenthesize("group", expr.expression);

        public string VisitLiteralExpr(Expr.Literal expr)
        {
            return expr.value == null ? "nil" : expr.value.ToString() ?? ""; // ?? "" is a workaround to avoid null possibility warning
        }

        public string VisitUnaryExpr(Expr.Unary expr) => Parenthesize(expr.@operator.lexeme, expr.right);

        public string Parenthesize(string name, params Expr[] exprs)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append('(').Append(name);
            foreach (Expr expr in exprs)
            {
                builder.Append(' ');
                builder.Append(expr.Accept(this));
            }
            builder.Append(')');

            return builder.ToString();
        }
    }
}

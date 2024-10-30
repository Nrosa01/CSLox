namespace CSLox.Src.Lox
{
    using System.Collections.Generic;

    public abstract class Expr
    {
        internal interface IVisitor<T> {
            T VisitAssignExpr(Assign expr);
            T VisitBinaryExpr(Binary expr);
            T VisitGroupingExpr(Grouping expr);
            T VisitLiteralExpr(Literal expr);
            T VisitLogicalExpr(Logical expr);
            T VisitUnaryExpr(Unary expr);
            T VisitVariableExpr(Variable expr);
        }
internal class Assign: Expr {
    internal Assign(Token name, Expr value) {
        this.name = name;
        this.value = value;
    }

    internal override T Accept<T>(IVisitor<T> visitor) {
        return visitor.VisitAssignExpr(this);
    }

    readonly public Token name;
    readonly public Expr value;
}
internal class Binary: Expr {
    internal Binary(Expr left, Token @operator, Expr right) {
        this.left = left;
        this.@operator = @operator;
        this.right = right;
    }

    internal override T Accept<T>(IVisitor<T> visitor) {
        return visitor.VisitBinaryExpr(this);
    }

    readonly public Expr left;
    readonly public Token @operator;
    readonly public Expr right;
}
internal class Grouping: Expr {
    internal Grouping(Expr expression) {
        this.expression = expression;
    }

    internal override T Accept<T>(IVisitor<T> visitor) {
        return visitor.VisitGroupingExpr(this);
    }

    readonly public Expr expression;
}
internal class Literal: Expr {
    internal Literal(object? value) {
        this.value = value;
    }

    internal override T Accept<T>(IVisitor<T> visitor) {
        return visitor.VisitLiteralExpr(this);
    }

    readonly public object? value;
}
internal class Logical: Expr {
    internal Logical(Expr left, Token @operator, Expr right) {
        this.left = left;
        this.@operator = @operator;
        this.right = right;
    }

    internal override T Accept<T>(IVisitor<T> visitor) {
        return visitor.VisitLogicalExpr(this);
    }

    readonly public Expr left;
    readonly public Token @operator;
    readonly public Expr right;
}
internal class Unary: Expr {
    internal Unary(Token @operator, Expr right) {
        this.@operator = @operator;
        this.right = right;
    }

    internal override T Accept<T>(IVisitor<T> visitor) {
        return visitor.VisitUnaryExpr(this);
    }

    readonly public Token @operator;
    readonly public Expr right;
}
internal class Variable: Expr {
    internal Variable(Token name) {
        this.name = name;
    }

    internal override T Accept<T>(IVisitor<T> visitor) {
        return visitor.VisitVariableExpr(this);
    }

    readonly public Token name;
}

        internal abstract T Accept<T>(IVisitor<T> visitor);
    }
}

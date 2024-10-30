namespace CSLox.Src.Lox
{
    using System.Collections.Generic;

    public abstract class Stmt
    {
        internal interface IVisitor<T> {
            T VisitBlockStmt(Block stmt);
            T VisitExpressionStmt(Expression stmt);
            T VisitPrintStmt(Print stmt);
            T VisitVarStmt(Var stmt);
        }
internal class Block: Stmt {
    internal Block(List<Stmt> statements) {
        this.statements = statements;
    }

    internal override T Accept<T>(IVisitor<T> visitor) {
        return visitor.VisitBlockStmt(this);
    }

    readonly public List<Stmt> statements;
}
internal class Expression: Stmt {
    internal Expression(Expr expression) {
        this.expression = expression;
    }

    internal override T Accept<T>(IVisitor<T> visitor) {
        return visitor.VisitExpressionStmt(this);
    }

    readonly public Expr expression;
}
internal class Print: Stmt {
    internal Print(Expr expression) {
        this.expression = expression;
    }

    internal override T Accept<T>(IVisitor<T> visitor) {
        return visitor.VisitPrintStmt(this);
    }

    readonly public Expr expression;
}
internal class Var: Stmt {
    internal Var(Token name, Expr? initializer) {
        this.name = name;
        this.initializer = initializer;
    }

    internal override T Accept<T>(IVisitor<T> visitor) {
        return visitor.VisitVarStmt(this);
    }

    readonly public Token name;
    readonly public Expr? initializer;
}

        internal abstract T Accept<T>(IVisitor<T> visitor);
    }
}

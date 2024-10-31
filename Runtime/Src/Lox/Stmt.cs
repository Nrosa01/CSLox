namespace CSLox.Src.Lox
{
    using System.Collections.Generic;

    public abstract class Stmt
    {
        internal interface IVisitor<T> {
            T VisitBlockStmt(Block stmt);
            T VisitExpressionStmt(Expression stmt);
            T VisitFunctionStmt(Function stmt);
            T VisitIfStmt(If stmt);
            T VisitPrintStmt(Print stmt);
            T VisitReturnStmt(Return stmt);
            T VisitVarStmt(Var stmt);
            T VisitWhileStmt(While stmt);
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
internal class Function: Stmt {
    internal Function(Token name, List<Token> @params, List<Stmt> body) {
        this.name = name;
        this.@params = @params;
        this.body = body;
    }

    internal override T Accept<T>(IVisitor<T> visitor) {
        return visitor.VisitFunctionStmt(this);
    }

    readonly public Token name;
    readonly public List<Token> @params;
    readonly public List<Stmt> body;
}
internal class If: Stmt {
    internal If(Expr condition, Stmt thenBranch, Stmt? elseBranch) {
        this.condition = condition;
        this.thenBranch = thenBranch;
        this.elseBranch = elseBranch;
    }

    internal override T Accept<T>(IVisitor<T> visitor) {
        return visitor.VisitIfStmt(this);
    }

    readonly public Expr condition;
    readonly public Stmt thenBranch;
    readonly public Stmt? elseBranch;
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
internal class Return: Stmt {
    internal Return(Token keyword, Expr? value) {
        this.keyword = keyword;
        this.value = value;
    }

    internal override T Accept<T>(IVisitor<T> visitor) {
        return visitor.VisitReturnStmt(this);
    }

    readonly public Token keyword;
    readonly public Expr? value;
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
internal class While: Stmt {
    internal While(Expr condition, Stmt body) {
        this.condition = condition;
        this.body = body;
    }

    internal override T Accept<T>(IVisitor<T> visitor) {
        return visitor.VisitWhileStmt(this);
    }

    readonly public Expr condition;
    readonly public Stmt body;
}

        internal abstract T Accept<T>(IVisitor<T> visitor);
    }
}

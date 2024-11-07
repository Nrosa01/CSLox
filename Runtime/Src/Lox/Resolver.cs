using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSLox.Src.Lox
{
    internal class Resolver(Interpreter interpreter) : Expr.IVisitor<object?>, Stmt.IVisitor<object?>
    {
        private readonly Stack<Dictionary<string, bool>> scopes = new Stack<Dictionary<string, bool>>();
        private FunctionType currentFunction = FunctionType.NONE;
        private ClassType currentClass = ClassType.NONE;

        private enum FunctionType
        {
            NONE,
            FUNCTION,
            INITIALIZER,
            METHOD
        }

        private enum ClassType
        {
            NONE, CLASS
        }

        public object? VisitAssignExpr(Expr.Assign expr)
        {
            Resolve(expr.value);
            ResolveLocal(expr, expr.name);
            return null;
        }

        public object? VisitBinaryExpr(Expr.Binary expr)
        {
            Resolve(expr.left);
            Resolve(expr.right);
            return null;
        }

        public object? VisitBlockStmt(Stmt.Block stmt)
        {
            BeginScope();
            Resolve(stmt.statements);
            EndScope();
            return null;
        }

        private void EndScope()
        {
            scopes.Pop();
        }

        public void Resolve(List<Stmt> statements)
        {
            foreach (Stmt statement in statements)
                Resolve(statement);
        }

        private void Resolve(Stmt statement) => statement.Accept(this);

        private void Resolve(Expr expression) => expression.Accept(this);

        private void BeginScope()
        {
            scopes.Push([]);
        }

        public object? VisitCallExpr(Expr.Call expr)
        {
            Resolve(expr.callee);

            foreach (Expr argument in expr.arguments)
                Resolve(argument);

            return null;
        }

        public object? VisitExpressionStmt(Stmt.Expression stmt)
        {
            Resolve(stmt.expression);
            return null;
        }

        public object? VisitFunctionStmt(Stmt.Function stmt)
        {
            Declare(stmt.name);
            Define(stmt.name);
            ResolveFunction(stmt, FunctionType.FUNCTION);
            return null;
        }

        private void ResolveFunction(Stmt.Function function, FunctionType type)
        {
            FunctionType enclosingFunction = currentFunction;
            currentFunction = type;
            BeginScope();
            foreach (var param in function.@params)
            {
                Declare(param);
                Define(param);
            }
            Resolve(function.body);
            EndScope();
            currentFunction = enclosingFunction;
        }

        public object? VisitGroupingExpr(Expr.Grouping expr)
        {
            Resolve(expr.expression);
            return null;
        }

        public object? VisitIfStmt(Stmt.If stmt)
        {
            Resolve(stmt.condition);
            Resolve(stmt.thenBranch);
            if (stmt.elseBranch != null) Resolve(stmt.elseBranch);
            return null;
        }

        public object? VisitLiteralExpr(Expr.Literal expr) => null;

        public object? VisitLogicalExpr(Expr.Logical expr)
        {
            Resolve(expr.left);
            Resolve(expr.right);
            return null;
        }

        public object? VisitPrintStmt(Stmt.Print stmt)
        {
            Resolve(stmt.expression);
            return null;
        }

        public object? VisitReturnStmt(Stmt.Return stmt)
        {
            if (currentFunction == FunctionType.NONE)
                Lox.Error(stmt.keyword, "Can't return from top-level code.");

            if (stmt.value != null)
            {
                if(currentFunction == FunctionType.INITIALIZER)
                    Lox.Error(stmt.keyword, "Can't return a value from an initializer");

                Resolve(stmt.value);
            }

            return null;
        }

        public object? VisitUnaryExpr(Expr.Unary expr)
        {
            Resolve(expr.right);
            return null;
        }

        public object? VisitVariableExpr(Expr.Variable expr)
        {
            if (scopes.Count != 0 && scopes.Peek().TryGetValue(expr.name.lexeme, out bool initialized) && !initialized)
                Lox.Error(expr.name, "Can't read local variable in its own initializer.");

            ResolveLocal(expr, expr.name);
            return null;
        }

        private void ResolveLocal(Expr expr, Token name)
        {
            for (int i = 0; i < scopes.Count; i++)
            {
                if (scopes.ElementAt(i).ContainsKey(name.lexeme))
                {
                    interpreter.Resolve(expr, i);
                    return;
                }
            }
        }

        public object? VisitVarStmt(Stmt.Var stmt)
        {
            Declare(stmt.name);
            if (stmt.initializer != null)
                Resolve(stmt.initializer);
            Define(stmt.name);
            return null;
        }

        private void Define(Token name)
        {
            if (scopes.Count == 0) return;
            scopes.Peek()[name.lexeme] = true;
        }

        private void Declare(Token name)
        {
            if (scopes.Count == 0) return;

            Dictionary<string, bool> scope = scopes.Peek();
            if (scope.ContainsKey(name.lexeme))
                Lox.Error(name, "Already a variable with this name in this scope.");

            scope[name.lexeme] = false;
        }

        public object? VisitWhileStmt(Stmt.While stmt)
        {
            Resolve(stmt.condition);
            Resolve(stmt.body);
            return null;
        }

        public object? VisitClassStmt(Stmt.Class stmt)
        {
            ClassType enclosingClass = currentClass;
            currentClass = ClassType.CLASS;

            Declare(stmt.name);
            Define(stmt.name);

            BeginScope();
            scopes.Peek().Add("this", true);

            foreach (Stmt.Function method in stmt.methods)
            {
                FunctionType declaration = FunctionType.METHOD;
                if (method.name.lexeme.Equals("init"))
                    declaration = FunctionType.INITIALIZER;

                ResolveFunction(method, declaration);
            }

            EndScope();
            
            currentClass = enclosingClass;
            return null;
        }

        public object? VisitGetExpr(Expr.Get expr)
        {
            Resolve(expr.@object);
            return null;
        }

        public object? VisitSetExpr(Expr.Set expr)
        {
            Resolve(expr.value);
            Resolve(expr.@object);
            return null;
        }

        public object? VisitThisExpr(Expr.This expr)
        {
            if(currentClass == ClassType.NONE)
            {
                Lox.Error(expr.keyword, "Can't use 'this' outside of a class.");
                return null;
            }

            ResolveLocal(expr, expr.keyword);
            return null;
        }
    }
}

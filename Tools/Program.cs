// See https://aka.ms/new-console-template for more information
using System.IO;
using System.Text;

if (args.Length != 1)
{
    Console.WriteLine("Usage: generate_ast <output directory>");
    Environment.Exit(64);
}
string outputDir = args[0];

DefineAst(outputDir, "Expr",
        [
            "Assign   : Token name, Expr value",
            "Binary   : Expr left, Token @operator, Expr right",
            "Call       : Expr callee, Token paren, List<Expr> arguments",
            "Get       : Expr @object, Token name",
            "Grouping : Expr expression",
            "Literal  : object? value",
            "Logical   : Expr left, Token @operator, Expr right",
            "Set        : Expr @object, Token name, Expr value",
            "This       : Token keyword",
            "Unary    : Token @operator, Expr right",
            "Variable : Token name"
        ]);

DefineAst(outputDir, "Stmt",
        [
            "Block           : List<Stmt> statements",
            "Class           : Token name, List<Stmt.Function> methods",
            "Expression   : Expr expression",
            "Function : Token name, List<Token> @params, List<Stmt> body",
            "If                : Expr condition, Stmt thenBranch, Stmt? elseBranch",
            "Print           : Expr expression",
            "Return     : Token keyword, Expr? value",
            "Var            : Token name, Expr? initializer",
            "While      : Expr condition, Stmt body"
        ]);


static void DefineAst(string outputDir, string baseName, List<string> types)
{
    string path = Path.Combine(outputDir, baseName + ".cs");
    using StreamWriter writer = new StreamWriter(path, false, System.Text.Encoding.UTF8);
    writer.WriteLine("namespace CSLox.Src.Lox");
    writer.WriteLine("{");
    writer.WriteLine("    using System.Collections.Generic;");
    writer.WriteLine();
    writer.WriteLine("    public abstract class " + baseName);
    writer.WriteLine("    {");

    DefineVisitor(writer, baseName, types);

    // The AST classes.
    foreach (string type in types)
    {
        string className = type.Split(":")[0].Trim();
        string fields = type.Split(":")[1].Trim();
        DefineType(writer, baseName, className, fields);
    }

    // The base accept() method.
    writer.WriteLine();
    writer.WriteLine("        internal abstract T Accept<T>(IVisitor<T> visitor);");

    writer.WriteLine("    }");
    writer.WriteLine("}");
}

static void DefineVisitor(StreamWriter writer, string baseName, List<string> types)
{
    writer.WriteLine($"        internal interface IVisitor<T> {{");

    foreach (string type in types)
    {
        string typeName = type.Split(":")[0].Trim();
        writer.WriteLine($"            T Visit{typeName}{baseName}({typeName} {baseName.ToLower()});");
    }

    writer.WriteLine("        }");
}

static void DefineType(StreamWriter writer, string baseName, string className, string fields)
{
    writer.WriteLine($"internal class {className}: {baseName} {{");

    // Constructor
    writer.WriteLine($"    internal {className}({fields}) {{");

    // Store parameters in fields
    string[] fieldList = fields.Split(", ");
    foreach (string field in fieldList)
    {
        string name = field.Split(" ")[1];
        writer.WriteLine($"        this.{name} = {name};");
    }

    writer.WriteLine("    }");

    // Visitor pattern
    writer.WriteLine();
    writer.WriteLine("    internal override T Accept<T>(IVisitor<T> visitor) {");
    writer.WriteLine($"        return visitor.Visit{className}{baseName}(this);");
    writer.WriteLine("    }");

    // Fields
    writer.WriteLine();
    foreach (string field in fieldList)
    {
        writer.WriteLine($"    readonly public {field};");
    }

    writer.WriteLine("}");
}
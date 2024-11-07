using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemEnv =  System.Environment;

namespace CSLox.Src.Lox
{
    internal class Lox
    {
        private static  Interpreter interpreter = new Interpreter();
        static bool hadError = false;
        static bool hadRuntimeError = false;
        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: jlox [script]");
                SystemEnv.Exit(64);
            }
            else if (args.Length == 1)
                RunFile(args[0]);
            else
                RunPrompt();
        }

        static void Run(in string code)
        {
            Scanner scanner = new(code);
            List<Token> tokens = scanner.ScanTokens();

            Parser parser = new(tokens);
            List<Stmt> statements = parser.Parse();

            if (hadError || statements == null) return;

            Resolver resolver = new(interpreter);
            resolver.Resolve(statements);

            if (hadError) return;

            //Console.WriteLine(new AstPrinter().Print(expression ?? new Expr.Literal("null")));
            interpreter.Interpret(statements);
        }

        static void RunFile(in String filename)
        {
            Console.WriteLine($"Running file {filename}");

            string text = "";

            try
            {
                text = File.ReadAllText(filename);
            }
            catch (Exception)
            {
                SystemEnv.Exit(65);
            }

            Run(text);
            if (hadError) SystemEnv.Exit(65);
            if (hadRuntimeError) SystemEnv.Exit(70);
        }

        static void RunPrompt()
        {
            string? line;

            while (true)
            {
                Console.Write("> ");
                line = Console.ReadLine();
                if (line == null || line == "")
                    break;
                else
                {
                    hadError = false;
                    Run(line);
                }
            }
        }

        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        private static void Report(int line, string where,
                                   string message)
        {
            Console.Error.WriteLine($"[line {line}] Error {where}: {message}");
            hadError = true;
        }

        public static void Error(Token token, String message)
        {
            if (token.type == TokenType.EOF)
                Report(token.line, " at end", message);
            else
                Report(token.line, $" at '{token.lexeme}'", message);
        }

        public static void RuntimeError(RuntimeError error)
        {
            Console.WriteLine($"{error.Message} \n[line {error.token.line}]");
            hadRuntimeError = true;
        }
    }
}

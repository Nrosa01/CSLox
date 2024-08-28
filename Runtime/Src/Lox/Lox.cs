using CSLox.Src.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSLox.Src.Lox
{
    internal class Lox
    {
        static bool hadError = false;

        static void Main(string[] args)
        {
            Expr expression = new Expr.Binary(
      new Expr.Unary(
          new Token(TokenType.MINUS, "-", null, 1),
          new Expr.Literal(123)),
      new Token(TokenType.STAR, "*", null, 1),
      new Expr.Grouping(
          new Expr.Literal(45.67)));

            Console.WriteLine(new AstPrinter().Print(expression));

            return;
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: jlox [script]");
                Environment.Exit(64);
            }
            else if (args.Length == 1)
                RunFile(args[0]);
            else
                RunPrompt();
        }

        static void Run(in string code)
        {
            Scanner scanner = new Scanner(code);
            List<Token> tokens = scanner.ScanTokens();

            foreach (Token token in tokens)
                Console.WriteLine(token);
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
                Environment.Exit(65);
            }

            Run(text);
            if (hadError) Environment.Exit(65);
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
    }
}

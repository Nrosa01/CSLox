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
            Run(File.ReadAllText(filename));
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

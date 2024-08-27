using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSLox.Src
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"There are {args.Length} arguments");

            if (args.Length > 1)
            {
                Console.WriteLine("Usage: jlox [script]");
                Environment.Exit(64);
            }
            else if (args.Length == 1)
            {
                RunFile(args[0]);
            }
            else
            {
                RunPrompt();
            }


            Console.ReadLine();
        }

        static void Run(in string code)
        {
            foreach (string word in code.WordList())
            {
                Console.WriteLine(word);
            }
        }

        static void RunFile(in String filename)
        {
            Console.WriteLine($"Running file {filename}");
            Run(File.ReadAllText(filename));
        }

        static void RunPrompt()
        {
            string line;

            while (true)
            {
                Console.Write("> ");
                line = Console.ReadLine();
                if (line.Length == 0)
                    break;
                else
                    Run(line);
            }
        }
    }
}

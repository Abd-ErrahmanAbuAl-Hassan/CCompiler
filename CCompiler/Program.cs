using Scanner;
using Parser;

namespace CCompiler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string filePath = "myprogram.c";
                if (args.Length > 0)
                    filePath = args[0];

                
                Console.WriteLine($"Compiling: {filePath}");
                Console.WriteLine("=".PadRight(60, '='));

                string code = File.ReadAllText(filePath);

                // Phase 1: Lexical Analysis
                Console.WriteLine("\n=== Phase 1: Lexical Analysis ===");
                var scanner = new Scanning(code);
                var tokens = scanner.Scan();

                Console.WriteLine($"Tokens found: {tokens.Count - 1}"); // Exclude EOF
                foreach (var token in tokens)
                {
                    if (token.Type != TokenType.Whitespace)
                    {
                        Console.WriteLine($"  {token}");
                    }
                }

                var scannerErrors = scanner.GetErrors();
                if (scannerErrors.Count > 0)
                {
                    Console.WriteLine("\nScanner errors:");
                    foreach (var error in scannerErrors)
                        Console.WriteLine($"  {error}");
                }

                // Phase 2: Syntax Analysis
                Console.WriteLine("\n=== Phase 2: Syntax Analysis ===");
                var parser = new Parsing(tokens);
                var ast = parser.Parse();

                Console.WriteLine(ast);

                var parserErrors = parser.GetErrors();
                if (parserErrors.Count > 0)
                {
                    Console.WriteLine("\nParser errors:");
                    foreach (var error in parserErrors)
                        Console.WriteLine($"  {error}");
                }
                else
                {
                    Console.WriteLine("Parsing successful!");
                }

                Console.WriteLine("\n" + "=".PadRight(60, '='));
                Console.WriteLine("Compilation completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fatal error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

    }
}
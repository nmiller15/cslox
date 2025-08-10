using System.Text;
using cslox.Models;
using cslox.Services;

namespace Lox;

public class Program
{
    public static int Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: cslox [script]");
            return 64;
        }
        else if (args.Length == 1)
        {
            RunFile(args[0]);
        }
        else
        {
            RunPrompt();
        }
        return 0;
    }

    public static void RunFile(string path)
    {
        var bytes = File.ReadAllBytes(Path.GetFullPath(path));
        Run(Encoding.UTF8.GetString(bytes));
    }

    public static void RunPrompt()
    {
        while (true)
        {
            Console.Write("> ");
            var line = Console.ReadLine();
            if (line == null)
                break;
            Run(line);
        }
    }

    public static void Run(string source)
    {
        var scanner = new Scanner(source);
        List<Token> tokens = scanner.ScanTokens();

        foreach (var token in tokens)
        {
            Console.WriteLine(token.Value);
        }
    }
}

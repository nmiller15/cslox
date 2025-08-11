using System.Text;
using cslox.Models;
using cslox.Services;

namespace Lox;

public class Program
{
    public static bool HasError = false;

    public static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: cslox [script]");
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
        Environment.Exit(0);
    }

    public static void RunFile(string path)
    {
        var bytes = File.ReadAllBytes(Path.GetFullPath(path));
        Run(Encoding.UTF8.GetString(bytes));

        if (HasError)
            Environment.Exit(65);
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
            HasError = false;
        }
    }

    public static void Run(string source)
    {
        var scanner = new Scanner(source);
        List<Token> tokens = scanner.ScanTokens();

        foreach (var token in tokens)
        {
        }
    }

    public static void Error(int line, string message)
    {
        Report(line, "", message);
    }

    public static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
        HasError = true;
    }
}

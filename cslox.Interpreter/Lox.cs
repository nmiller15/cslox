using System.Text;
using cslox.Interpreter.Services;
using cslox.Models;
using cslox.Services;

namespace cslox;

public static class Lox
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

        var parser = new Parser(tokens);
        Expr expression = parser.Parse();

        if (HasError) return;

        Console.WriteLine(new AstPrinter().Print(expression));
    }

    public static void Error(Token token, string message)
    {
        if (token.Type == Token.TokenTypes.EOF)
        {
            Report(token.Line, " at end", "message");
        }
        else
        {
            Report(token.Line, " at '" + token.Lexeme + "'", message);
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


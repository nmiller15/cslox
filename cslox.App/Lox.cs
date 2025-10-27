using System.Text;
using cslox.Services;
using cslox.Models;
using SysEnvironment = System.Environment;

namespace cslox;

public static class Lox
{
    private readonly static Interpreter _interpreter = new Interpreter();

    public static bool HadError = false;
    public static bool HadRuntimeError = false;

    public static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: cslox [script]");
            SysEnvironment.Exit(64);
        }
        else if (args.Length == 1)
        {
            RunFile(args[0]);
        }
        else
        {
            RunPrompt();
        }
        SysEnvironment.Exit(0);
    }

    public static void RunFile(string path)
    {
        var bytes = File.ReadAllBytes(Path.GetFullPath(path));
        Run(Encoding.UTF8.GetString(bytes));

        if (HadError)
            SysEnvironment.Exit(65);
        if (HadRuntimeError)
            SysEnvironment.Exit(70);
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
            HadError = false;
        }
    }

    public static void Run(string source)
    {
        var scanner = new Scanner(source);
        List<Token> tokens = scanner.ScanTokens();

        var parser = new Parser(tokens);
        List<Stmt> statements = parser.Parse();

        if (HadError) return;

        var resolver = new Resolver(_interpreter);
        resolver.Resolve(statements);

        if (HadError) return;

        // Console.WriteLine(new AstPrinter().Print(expression));
        _interpreter.Interpret(statements);
    }

    public static void Error(Token token, string message)
    {
        if (token.Type == Token.TokenTypes.EOF)
        {
            Report(token.Line, " at end", message);
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

    public static void RuntimeError(RuntimeError error)
    {
        Console.Error.WriteLine(error.Message +
                $"{SysEnvironment.NewLine} {error.Token.Line} ]");
        HadRuntimeError = true;
    }

    public static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
        HadError = true;
    }
}


using System.Text;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Usage: GenerateAst <output directory>");
            return;
        }

        string outputDir = args[0];
        DefineAst(outputDir, "Expr", new List<string>()
        {
            "Binary : Expr Left, Token Operator, Expr Right",
            "Grouping : Expr Expression",
            "Literal : object Value",
            "Unary : Token Operator, Expr Right",
            "Ternary : Expr Condition, Token Operator, Expr IfTrue, Token Separator, Expr IfFalse",
            "Variable : Token Name",
            "Assign : Token Name, Expr Value"
        });

        DefineAst(outputDir, "Stmt", new List<string>()
        {
            "Block : List<Stmt> Statements",
            "Expression : Expr eExpression",
            "Print : Expr Expression",
            "Var : Token Name, Expr Initializer"
        });
    }

    static void DefineAst(string outputDir, string baseName, List<string> types)
    {

        var path = Path.Combine(outputDir, $"{baseName}.cs").ToString();
        var writer = new StreamWriter(path, false, Encoding.UTF8);

        writer.WriteLine("using cslox.Models;");
        writer.WriteLine();
        writer.WriteLine($"namespace cslox.Models;");
        writer.WriteLine();
        writer.WriteLine($"public abstract class {baseName}");
        writer.WriteLine("{");
        writer.WriteLine("    public abstract T Accept<T>(IVisitor<T> visitor);");
        writer.WriteLine();
        DefineVisitor(writer, baseName, types);
        writer.WriteLine();
        foreach (var type in types)
        {
            var className = type.Split(':')[0].Trim();
            var fields = type.Split(':')[1].Trim();
            writer.WriteLine($"    public class {className} : {baseName}");
            writer.WriteLine("    {");
            DefineClass(writer, className, baseName, fields);
            writer.WriteLine("    }");
            writer.WriteLine();
        }
        writer.WriteLine();
        writer.WriteLine("}");

        writer.Close();
    }

    static void DefineClass(StreamWriter writer, string className, string baseName, string fieldList)
    {
        List<string> fields = fieldList.Split(", ").ToList();
        foreach (var field in fields)
        {
            var type = field.Split(' ')[0];
            var name = field.Split(' ')[1];

            writer.WriteLine($"        public {type} {name} {{ get; }}");
        }
        writer.WriteLine();

        // Constructor Declaration
        writer.Write($"        public {className}(");
        var first = true;
        foreach (var field in fields)
        {
            var type = field.Split(' ')[0];
            var name = field.Split(' ')[1].ToLower();
            if (name.Equals("operator")) name = "oper";
            if (first) first = false;
            else writer.Write(", ");

            writer.Write($"{type} {name}");
        }
        writer.WriteLine(")");

        // Constructor Body
        writer.WriteLine("        {");
        foreach (var field in fields)
        {
            var name = field.Split(' ')[1];
            var argument = name.Equals("operator", StringComparison.OrdinalIgnoreCase)
                ? "oper"
                : name.ToLower();

            writer.WriteLine($"            {name} = {argument};");

        }

        writer.WriteLine("        }");
        writer.WriteLine();
        writer.WriteLine("        public override T Accept<T>(IVisitor<T> visitor)");
        writer.WriteLine("        {");
        writer.WriteLine($"            return visitor.visit{className}{baseName}(this);");
        writer.WriteLine("        }");
    }

    static void DefineVisitor(StreamWriter writer, string baseName, List<string> types)
    {
        writer.WriteLine($"    public interface IVisitor<T>");
        writer.WriteLine("    {");
        foreach (var type in types)
        {
            var typeName = type.Split(':')[0].Trim();
            writer.WriteLine($"        T visit{typeName}{baseName}({typeName} {baseName.ToLower()});");
        }
        writer.WriteLine("    }");
    }
}

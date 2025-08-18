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
            "Unary : Token Operator, Expr Right"
        });

    }

    static void DefineAst(string outputDir, string baseName, List<string> types)
    {
        foreach (var type in types)
        {
            var className = type.Split(':')[0].Trim();
            var props = type.Split(':')[1].Trim().Split(',');
            var path = Path.Combine(outputDir, $"{className}.cs").ToString();
            var writer = new StreamWriter(path, false, Encoding.UTF8);

            writer.WriteLine("using cslox.Models;");
            writer.WriteLine();
            writer.WriteLine($"namespace cslox.Models;");
            writer.WriteLine();
            writer.WriteLine($"public class {className} : {baseName}");
            writer.WriteLine("{");
            foreach (var prop in props)
            {
                var parts = prop.Split(' ');
                var declare = parts[0].Trim();
                var name = parts[1].Trim();
                writer.WriteLine($"    public {declare} {name} {{ get; }}");
            }
            writer.WriteLine();
            writer.Write($"    public {className}(");
            var first = true;
            foreach (var prop in props)
            {
                if (first) { first = false; }
                else { writer.Write(", "); }
                writer.Write($"{prop.Split(' ')[0].Trim()} {prop.Split(' ')[1].Trim().ToLower()}");
            }
            writer.WriteLine(")");
            writer.WriteLine("    {");
            foreach (var prop in props)
            {
                writer.WriteLine($"        {prop.Split(' ')[1].Trim()} = {prop.Split(' ')[1].Trim().ToLower()};");
            }
            writer.WriteLine("    }");
            writer.WriteLine("}");

            writer.Close();
        }
    }
}

using cslox.Models;

namespace cslox.Services;

public class Scanner
{
    public string Source { get; set; }

    public Scanner(string source)
    {
        Source = source;
    }

    public List<Token> ScanTokens()
    {
        return new List<Token>() { new Token()
            {
                Value = Source,
            }
        };
    }
}


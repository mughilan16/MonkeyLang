using System.Linq.Expressions;
using System.Text;

namespace Monkey;

public interface INode
{
    string? TokenLiteral();
    string? String();
}

public interface IStatement : INode
{
    public new string? TokenLiteral();
}

public interface IExpression : INode
{
    public new string? TokenLiteral();
}

public class IntegerLiteral : IExpression
{
    public Token? Token { get; set; }
    public long Value { get; set; } // long = int64

    public string? TokenLiteral()
    {
        return Token?.Literal;
    }

    public string? String()
    {
        return Token?.Literal;
    }
}

public class Boolean : IExpression
{
    public Token? Token { get; init; }
    public bool Value { get; set; }

    public string? TokenLiteral()
    {
        return Token?.Literal;
    }

    public string? String()
    {
        return Token?.Literal;
    }
}

public class Identifier : IExpression
{
    public Token? Token;
    public string? Value;

    public string? TokenLiteral()
    {
        return Token?.Literal;
    }

    public string? String()
    {
        return Value;
    }
}

public class LetStatement : IStatement
{
    public Token? Token { get; init; }
    public Identifier? Name { get; set; }
    public IExpression? Value { get; set; }

    public string? TokenLiteral()
    {
        return Token?.Literal;
    }

    public string String()
    {
        MemoryStream buffer = new();
        StreamWriter writer = new(buffer, Encoding.Unicode);

        writer.Write(TokenLiteral() + " ");
        writer.Write(Name?.String());
        writer.Write(" = ");
        if (Value != null)
        {
            writer.Write(Value.String());
        }

        writer.Write(";");
        writer.Flush();


        buffer.Position = 0;
        StreamReader reader = new(buffer, Encoding.Unicode, false);
        return reader.ReadToEnd();
    }
}

public class ReturnStatement : IStatement
{
    public Token? Token;
    public IExpression? ReturnValue;

    public string? TokenLiteral()
    {
        return Token?.Literal;
    }

    public string String()
    {
        MemoryStream buffer = new();
        StreamWriter writer = new(buffer, Encoding.Unicode);

        writer.Write(TokenLiteral() + " ");
        if (ReturnValue != null)
        {
            writer.Write(ReturnValue.String());
        }

        writer.Write(";");
        writer.Flush();


        buffer.Position = 0;
        StreamReader reader = new(buffer, Encoding.Unicode, false);
        return reader.ReadToEnd();
    }
}

public class ExpressionStatement : IStatement
{
    public Token? Token;
    public IExpression? Expression;

    public string? TokenLiteral()
    {
        return Token?.Literal;
    }

    public string? String()
    {
        return Expression != null ? Expression.String() : "";
    }
}

public class PrefixExpression : IExpression
{
    public Token? Token;
    public string? Operator;
    public IExpression? Right;

    public string? TokenLiteral()
    {
        return Token?.Literal;
    }

    public string String()
    {
        MemoryStream buffer = new();
        StreamWriter writer = new(buffer, Encoding.Unicode);

        writer.Write("(");
        writer.Write(Operator);
        writer.Write(Right?.String());
        writer.Write(")");
        writer.Flush();


        buffer.Position = 0;
        StreamReader reader = new(buffer, Encoding.Unicode, false);
        return reader.ReadToEnd();
    }
}

public class InfixExpression : IExpression
{
    public Token? Token;
    public IExpression? Left;
    public string? Operator;
    public IExpression? Right;

    public string? TokenLiteral()
    {
        return Token?.Literal;
    }

    public string String()
    {
        MemoryStream buffer = new();
        StreamWriter writer = new(buffer, Encoding.Unicode);

        writer.Write("(");
        writer.Write(Left?.String());
        writer.Write(" ");
        writer.Write(Operator);
        writer.Write(" ");
        writer.Write(Right?.String());
        writer.Write(")");
        writer.Flush();


        buffer.Position = 0;
        StreamReader reader = new(buffer, Encoding.Unicode, false);
        return reader.ReadToEnd();
    }
}

public class BlockStatement : IStatement
{
    public Token? Token { get; set; }
    public List<IStatement>? Statements { get; set; }

    public string String()
    {
        MemoryStream buffer = new();
        StreamWriter writer = new(buffer, Encoding.Unicode);

        if (Statements != null)
            foreach (var s in Statements)
            {
                writer.Write(s.String());
            }

        writer.Flush();


        buffer.Position = 0;
        StreamReader reader = new(buffer, Encoding.Unicode, false);
        return reader.ReadToEnd();
    }

    public string? TokenLiteral()
    {
        return Token?.Literal;
    }
}

public class FunctionLiteral : IExpression
{
    public Token Token;
    public List<Identifier> Parameters;
    public BlockStatement Body;

    public string TokenLiteral()
    {
        return Token.Literal;
    }

    public string String()
    {
        MemoryStream buffer = new();
        StreamWriter writer = new(buffer, Encoding.Unicode);
        
        List<string> _params = [] ;
        _params.AddRange(Parameters.Select(parameter => parameter.String()));

        writer.Write(TokenLiteral());
        writer.Write("(");
        writer.Write(string.Join(", ", _params.ToArray()));
        writer.Write(") ");
        writer.Write(Body.String());
        writer.Flush();


        buffer.Position = 0;
        StreamReader reader = new(buffer, Encoding.Unicode, false);
        return reader.ReadToEnd();
    }
}

public class IfExpression : IExpression
{
    public Token Token { get; set; }
    public IExpression? Condition { get; set; }
    public BlockStatement Consequence { get; set; }
    public BlockStatement? Alternative { get; set; }

    public string String()
    {
        MemoryStream buffer = new();
        StreamWriter writer = new(buffer, Encoding.Unicode);

        writer.Write("if");
        writer.Write(Condition.String());
        writer.Write(" ");
        writer.Write(Consequence.String());
        if (Alternative != null)
        {
            writer.Write("else ");
            writer.Write(Alternative.String());
        }
        writer.Flush();


        buffer.Position = 0;
        StreamReader reader = new(buffer, Encoding.Unicode, false);
        return reader.ReadToEnd();
    }

    public string? TokenLiteral()
    {
        return Token.Literal;
    }
}

public class Program : INode
{
    public readonly List<IStatement> Statements = [];

    public string? TokenLiteral()
    {
        return Statements.Count > 0 ? Statements[0].TokenLiteral() : "";
    }

    public string String()
    {
        MemoryStream buffer = new();
        StreamWriter writer = new(buffer, Encoding.Unicode);

        foreach (var s in Statements)
        {
            writer.Write(s.String());
        }

        writer.Flush();


        buffer.Position = 0;
        StreamReader reader = new(buffer, Encoding.Unicode, false);
        return reader.ReadToEnd();
    }
}
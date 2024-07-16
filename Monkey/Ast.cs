using System.Text;

namespace Monkey;

public interface INode
{
    string? TokenLiteral();
    string? String();
}

public interface IStatement: INode
{
    protected void StatementNode();
    public new string? TokenLiteral();
}

public interface IExpression: INode
{
    protected void ExpressionNode();
    public new string? TokenLiteral();
}

public class IntegerLiteral: IExpression
{
    public Token? Token;
    public long Value; // long = int64
    public void ExpressionNode() {}
    public string? TokenLiteral()
    {
        return Token?.Literal;
    }
    public string? String()
    {
        return Token?.Literal;
    }
}

public class Identifier: IExpression
{
    public Token? Token;
    public string? Value;
    public string? TokenLiteral()
    {
        return Token?.Literal;
    }

    public void ExpressionNode() {}
    public string? String()
    {
        return Value;
    }
}

public class LetStatement: IStatement
{
    public Token? Token { get; set; }
    public Identifier? Name { get; set; }
    public IExpression? Value { get; set; }

    public void StatementNode() { }

    public string? TokenLiteral()
    {
        return Token?.Literal;
    }
    public string? String()
    {
        MemoryStream buffer = new();
        StreamWriter writer = new(buffer, Encoding.Unicode);

        try
        {
            writer.Write(TokenLiteral() + " ");
            writer.Write(Name?.String());
            writer.Write(" = ");
            if (Value != null)
            {
                writer.Write(Value.String());
            }
            writer.Write(";");
        }
        finally
        {
            writer.Dispose();
        }

        return buffer.ToString();
    }
}

public class ReturnStatement : IStatement
{
    public Token? Token;
    public IExpression? ReturnValue;
    public void StatementNode() {}
    
    public string? TokenLiteral()
    {
        return Token?.Literal;
    }
    public string? String()
    {
        MemoryStream buffer = new();
        StreamWriter writer = new(buffer, Encoding.Unicode);

        try
        {
            writer.Write(TokenLiteral() + " ");
            if (ReturnValue != null)
            {
                writer.Write(ReturnValue.String());
            }
            writer.Write(";");
        }
        finally
        {
            writer.Dispose();
        }

        return buffer.ToString();
    }
}

public class ExpressionStatement : IStatement
{
    public Token? Token;
    public IExpression? Expression;
    public void StatementNode() {}
    
    public string? TokenLiteral()
    {
        return Token?.Literal;
    }

    public string? String()
    {
        return Token?.Literal;
    }
}

public class PrefixExpression : IExpression
{
    public Token? Token;
    public string? Operator;
    public IExpression? Right;
    
    public void ExpressionNode() {}

    public string? TokenLiteral()
    {
        return Token?.Literal;
    }

    public string? String()
    {
        MemoryStream buffer = new();
        StreamWriter writer = new(buffer, Encoding.Unicode);

        try
        {
            writer.Write("(");
            writer.Write(Operator);
            writer.Write(Right?.String());
            writer.Write(")");
        }
        finally
        {
            writer.Dispose();
        }

        return buffer.ToString();
    }
}

public class InfixExpression: IExpression
{
    public Token? Token;
    public IExpression? Left;
    public string? Operator;
    public IExpression? Right;
    
    public void ExpressionNode() {}

    public string? TokenLiteral()
    {
        return Token?.Literal;
    }

    public string? String()
    {
        MemoryStream buffer = new();
        StreamWriter writer = new(buffer, Encoding.Unicode);

        try
        {
            writer.Write("(");
            writer.Write(Left?.String());
            writer.Write(Operator);
            writer.Write(Right?.String());
            writer.Write(")");
        }
        finally
        {
            writer.Dispose();
        }

        return buffer.ToString();
    }
}

public class Program: INode
{
    public readonly List<IStatement> Statements = [];

    public string? TokenLiteral()
    {
        return Statements.Count > 0 ? Statements[0].TokenLiteral() : "";
    }

    public string? String()
    {
        MemoryStream buffer = new();
        StreamWriter writer = new(buffer, Encoding.Unicode);

        try
        {
            foreach (var s in Statements)
            {
                writer.Write(s.String());
            }
        }
        finally
        {
            writer.Dispose();
        }

        return buffer.ToString();
    }
}
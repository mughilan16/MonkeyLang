using static Monkey.TokenType;

namespace Monkey;

public enum Precedence
{
    Lowest = 1,
    Equals = 2,
    LessGreater = 3,
    Sum = 4,
    Product = 5,
    Prefix = 6,
    Call = 7,
}

public class Parser
{
    private readonly Lexer _lexer;
    private Token _curToken;
    private Token _peekToken;
    private readonly List<string> _errors;
    
    private readonly Dictionary<TokenType, Func<IExpression?>> _prefixParseFns = new();
    private readonly Dictionary<TokenType, Func<IExpression?>> _infixParseFns = new();

    public Parser(Lexer lexer)
    {
        _lexer = lexer;
        _errors = [];
        _curToken = _lexer.NextToken();
        _peekToken = _lexer.NextToken();
        RegisterPrefix(TokenType.Ident, ParseIdentifier);
        RegisterPrefix(TokenType.Int, ParseIntegerLiteral);
    }

    private void RegisterPrefix(TokenType tokenType, Func<IExpression> fn)
    {
        _prefixParseFns[tokenType] = fn;
    }

    private void RegisterInfix(TokenType tokenType, Func<IExpression> fn)
    {
        _infixParseFns[tokenType] = fn;
    }

    private void NextToken()
    {
        _curToken = _peekToken;
        _peekToken = _lexer.NextToken();
    }

    private void PeekError(TokenType t)
    {
        var msg = $"expected next token to be {t}, got {_peekToken.Type} instead";
        _errors.Add(msg);
    }

    private bool ExceptPeek(TokenType t)
    {
        if (_peekToken.Type == t)
        {
            NextToken();
            return true;
        }
        else
        {
            PeekError(t);
            return false;
        }
    }

    private IExpression? ParseIdentifier()
    {
        return new Identifier
        {
            Token = _curToken, 
            Value = _curToken.Literal
        };
    }

    private IExpression? ParseIntegerLiteral()
    {
        var literal = new IntegerLiteral
        {
            Token = _curToken
        };
        long value;
        try
        {
            value = long.Parse(_curToken.Literal);
        }
        catch (FormatException)
        {
            _errors.Add($"could not parse {_curToken.Literal} as a integer");
            return null;
        }

        literal.Value = value;
        return literal;
    }
    
    private IExpression? ParseExpression(Precedence precedence)
    {
        if (!_prefixParseFns.TryGetValue(_curToken.Type, out var prefix))
        {
            return null;
        }
        var leftExp = prefix();
        return leftExp;
    }

    private LetStatement? ParseLetStatement()
    {
        var statement = new LetStatement { Token = _curToken };
        if (!ExceptPeek(Ident))
        {
            return null;
        }

        statement.Name = new Identifier
        {
            Token = _curToken,
            Value = _curToken.Literal
        };

        if (!ExceptPeek(Assign))
        {
            return null;
        }

        while (_curToken.Type != (Semicolon))
        {
            NextToken();
        }

        return statement;
    }

    private ReturnStatement ParseReturnStatement()
    {
        var statement = new ReturnStatement
        {
            Token = _curToken
        };
        NextToken();
        while (_curToken.Type != Semicolon)
        {
            NextToken();
        }

        return statement;
    }

    private ExpressionStatement ParseExpressionStatement()
    {
        var statement = new ExpressionStatement
        {
            Token = _curToken,
            Expression = ParseExpression(Precedence.Lowest)
        };

        if (_peekToken.Type == Semicolon)
        {
            NextToken();
        }

        return statement;
    }

    private IStatement? ParseStatement()
    {
        return _curToken.Type switch
        {
            Let => ParseLetStatement(),
            Return => ParseReturnStatement(),
            _ => ParseExpressionStatement()
        };
    }

    public List<string> Errors()
    {
        return _errors;
    }
    
    public Program ParseProgram()
    {
        Program program = new();
        while (_curToken.Type != Eof)
        {
            var statement = ParseStatement();
            if (statement != null)
            {
                program.Statements.Add(statement);
            }
            NextToken();
        }
        return program;
    }
}
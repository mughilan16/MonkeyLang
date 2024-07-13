namespace Monkey;

public class Lexer
{
    private readonly string _input;
    private int _position;
    private int _readPosition;
    private byte _ch;

    public Lexer(string input)
    {
        _input = input;
        ReadChar();
    }

    private void ReadChar()
    {
        if (_readPosition >= _input.Length)
        {
            _ch = 0;
        }
        else
        {
            _ch = (byte) _input[_readPosition];
        }

        _position = _readPosition;
        _readPosition += 1;
    }

    private static bool IsLetter(byte ch)
    {
        return 'a' <= ch && ch <= 'z' || 'A' <= ch && ch <= 'Z' || ch == '_';
    }
    
    private static bool IsDigit(byte ch)
    {
        return '0' <= ch && ch <= '9';
    }

    private string ReadIdentifier()
    {
        var position = _position;
        while (IsLetter(_ch))
        {
            ReadChar();
        }

        return _input.Substring(position, _position-position);
    }

    private string ReadNumber()
    {
        var position = _position;
        while (IsDigit(_ch))
        {
            ReadChar();
        }

        return _input.Substring(position, _position-position);
    }

    private void SkipWhiteSpace()
    {
        while (_ch == ' ' || _ch == '\t' || _ch == '\n' || _ch == '\r')
        {
            ReadChar();
        }
    }

    
    public Token NextToken()
    {
        Token token;
        
        SkipWhiteSpace();
        
        switch (_ch)
        {
            case (byte) '=':
                token = new Token(TokenType.Assign, _ch);
                break;
            case (byte) '+':
                token = new Token(TokenType.Plus, _ch);
                break;
            case (byte) '-':
                token = new Token(TokenType.Minus, _ch);
                break;
            case (byte) '*':
                token = new Token(TokenType.Asterisk, _ch);
                break;
            case (byte) '/':
                token = new Token(TokenType.Slash, _ch);
                break;
            case (byte) '<':
                token = new Token(TokenType.Lt, _ch);
                break;
            case (byte) '>':
                token = new Token(TokenType.Gt, _ch);
                break;
            case (byte) '!':
                token = new Token(TokenType.Bang, _ch);
                break;
            case (byte) ';':
                token = new Token(TokenType.Semicolon, _ch);
                break;
            case (byte) ',':
                token = new Token(TokenType.Comma, _ch);
                break;
            case (byte) '(':
                token = new Token(TokenType.Lparan, _ch);
                break;
            case (byte) ')':
                token = new Token(TokenType.Rparan, _ch);
                break;
            case (byte) '{':
                token = new Token(TokenType.Lbrace, _ch);
                break;
            case (byte) '}':
                token = new Token(TokenType.Rbrace, _ch);
                break;
            case 0:
                token = new Token(TokenType.Eof, "");
                break;
            default:
                if (IsLetter(_ch))
                {
                    var literal = ReadIdentifier();
                    token = new Token
                    {
                        Literal = literal,
                        Type = Token.LookupIdent(literal)
                    };
                    // early return to prevent calling ReadChar()
                    // it already execute inside of ReadIdentifier()
                    return token;
                }
                else if (IsDigit(_ch))
                {
                    token = new Token
                    {
                        Literal = ReadNumber(),
                        Type = TokenType.Int
                    };
                    // early return to prevent calling ReadChar()
                    // it already execute inside of ReadIdentifier()
                    return token;
                }
                else
                {
                    token = new Token(TokenType.Illegal, _ch);
                }
                break;
        }
        ReadChar();
        return token;
    }
}
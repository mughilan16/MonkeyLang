namespace Monkey
{

    public record Token
    {
        public TokenType Type { get; set; }
        public string? Literal { get; set; }

        private static readonly Dictionary<string, TokenType> Keywords = new()
        {
            {"fn", TokenType.Function},
            {"let", TokenType.Let},
            {"true", TokenType.True},
            {"false", TokenType.False},
            {"if", TokenType.If},
            {"else", TokenType.Else},
            {"return", TokenType.Return},
        };

        public Token()
        {
            Literal = "";
        }

        public Token(TokenType type, string? literal)
        {
            Type = type;
            Literal = literal;
        }

        public Token(TokenType type, byte literal) 
        {
            var character = (char)literal;
            Type = type;
            Literal = character.ToString();
        }
        
        public static TokenType LookupIdent(string ident)
        {
            return Keywords.GetValueOrDefault(ident, TokenType.Ident);
        }
    }

    public enum TokenType
    {
        Illegal,
        Eof,

        Ident,
        Int,
        
        Eq,
        NotEq,

        Assign = '=',
        Plus = '+',
        Minus = '-',
        Bang = '!',
        Asterisk = '*',
        Slash = '/',
        
        Lt = '<',
        Gt = '>',

        Comma = ',',
        Semicolon = ';',

        Lparan = '(',
        Rparan = ')',
        Lbrace = '{',
        Rbrace = '}',

        Function,
        Let,
        True,
        False,
        If,
        Else,
        Return
        
    }
}
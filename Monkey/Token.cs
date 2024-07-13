namespace Monkey
{

    public record Token
    {
        public TokenType Type { get; set; }
        public string? Literal { get; set; }

        private static readonly Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>
        {
            {"fn", TokenType.Function},
            {"let", TokenType.Let}
        };

        public Token() { }

        public Token(TokenType type, string literal)
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

        Assign,
        Plus,

        Comma = ',',
        Semicolon = ';',

        Lparan = '(',
        Rparan = ')',
        Lbrace = '{',
        Rbrace = '}',

        Function,
        Let
    }
}
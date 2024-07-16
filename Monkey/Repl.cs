namespace Monkey;

public class Repl(string prompt = ">> ")
{
    private readonly string _prompt = prompt;
    public void Start()
    {
        while (true)
        {
            Console.Write(_prompt);
            var input = Console.ReadLine();
            if (input == null)
            {
                return;
            }

            Lexer lexer = new(input);
            for (var token = lexer.NextToken(); token.Type != TokenType.Eof; token = lexer.NextToken())
            {
                Console.WriteLine(token);
            }
        }
    }
}
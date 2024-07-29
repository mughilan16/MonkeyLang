namespace Monkey;

public class Repl(string prompt = ">> ")
{
    public void Start()
    {
        while (true)
        {
            Console.Write(prompt);
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
namespace MyCompiler.LexicalAnalyzer
{
    public class LexicalError
    {
        public int Line { get; set; }
        public int Column { get; set; }
        public string Message { get; set; }
        public string Symbol { get; set; }

        public LexicalError(int line, int column, string message, string symbol = "")
        {
            Line = line;
            Column = column;
            Message = message;
            Symbol = symbol;
        }
    }
}
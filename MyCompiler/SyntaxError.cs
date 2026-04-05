namespace MyCompiler.SyntaxAnalyzer
{
    public class SyntaxError
    {
        public int ErrorNumber { get; set; }
        public string Message { get; set; }
        public string Expected { get; set; }
        public string Found { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }

        public SyntaxError()
        {
            ErrorNumber = 0;
            Message = string.Empty;
            Expected = string.Empty;
            Found = string.Empty;
            Line = 1;
            Column = 1;
        }

        public override string ToString()
        {
            return $"[{Line}:{Column}] {Message} (ожидалось: {Expected}, найдено: {Found})";
        }
    }
}
namespace MyCompiler.SearchModule
{
    public class SearchResult
    {
        public string Match { get; set; }
        public int Line { get; set; }
        public int Position { get; set; }
        public int Length { get; set; }
        public int AbsoluteIndex { get; set; }

        public override string ToString()
        {
            return $"{Match} | Строка {Line}, позиция {Position} | Длина {Length}";
        }
    }
}
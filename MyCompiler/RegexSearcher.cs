using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MyCompiler.SearchModule
{
    public class RegexSearcher
    {

        private const string PatternYears = @"(200[0-9]|2010)";

        private const string PatternMirCard = @"220[0-4]\d{12}";

        private const string PatternIPv6WithPrefix =
            @"([0-9a-fA-F]{1,4}:){7}[0-9a-fA-F]{1,4}/\d{1,3}";

        public enum SearchType
        {
            Years2000to2010,
            MirCard,
            IPv6WithPrefix
        }

        public List<SearchResult> FindMatches(string text, SearchType searchType)
        {
            string pattern = GetPattern(searchType);
            return FindMatchesWithPattern(text, pattern);
        }

        private string GetPattern(SearchType searchType)
        {
            switch (searchType)
            {
                case SearchType.Years2000to2010:
                    return PatternYears;
                case SearchType.MirCard:
                    return PatternMirCard;
                case SearchType.IPv6WithPrefix:
                    return PatternIPv6WithPrefix;
                default:
                    throw new ArgumentException("Неизвестный тип поиска");
            }
        }

        private List<SearchResult> FindMatchesWithPattern(string text, string pattern)
        {
            var results = new List<SearchResult>();

            if (string.IsNullOrEmpty(text))
                return results;

            try
            {
                Regex regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                MatchCollection matches = regex.Matches(text);

                string[] lines = text.Split('\n');

                foreach (Match match in matches)
                {
                    int absoluteIndex = match.Index;
                    int lineNumber = GetLineNumber(text, absoluteIndex);
                    int positionInLine = GetPositionInLine(text, absoluteIndex);

                    results.Add(new SearchResult
                    {
                        Match = match.Value,
                        Line = lineNumber,
                        Position = positionInLine,
                        Length = match.Length,
                        AbsoluteIndex = absoluteIndex
                    });
                }
            }
            catch (RegexParseException ex)
            {
                throw new ArgumentException($"Ошибка в регулярном выражении: {ex.Message}");
            }

            return results;
        }

        private int GetLineNumber(string text, int index)
        {
            int lineNumber = 1;
            for (int i = 0; i < index && i < text.Length; i++)
            {
                if (text[i] == '\n')
                    lineNumber++;
            }
            return lineNumber;
        }
        private int GetPositionInLine(string text, int index)
        {
            int position = 1;
            for (int i = index - 1; i >= 0 && i < text.Length; i--)
            {
                if (text[i] == '\n')
                    break;
                position++;
            }
            return position;
        }
    }
}
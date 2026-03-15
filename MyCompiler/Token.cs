using System;

namespace MyCompiler.LexicalAnalyzer
{
    public class Token
    {
        public TokenType Type { get; set; }
        public string Value { get; set; }
        public int Line { get; set; }
        public int StartColumn { get; set; }
        public int EndColumn { get; set; }
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }

        public Token()
        {
            Type = TokenType.Unknown;
            Value = string.Empty;
            Line = 1;
            StartColumn = 1;
            EndColumn = 1;
            IsError = false;
            ErrorMessage = string.Empty;
        }

        public Token(TokenType type, string value, int line, int startCol, int endCol)
        {
            Type = type;
            Value = value;
            Line = line;
            StartColumn = startCol;
            EndColumn = endCol;
            IsError = false;
            ErrorMessage = string.Empty;
        }

        public static Token CreateError(string message, int line, int column)
        {
            return new Token
            {
                Type = TokenType.Error,
                Value = message,
                Line = line,
                StartColumn = column,
                EndColumn = column,
                IsError = true,
                ErrorMessage = message
            };
        }
    }
}
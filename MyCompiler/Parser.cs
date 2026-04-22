using System;
using System.Collections.Generic;
using MyCompiler.LexicalAnalyzer;

namespace MyCompiler.SyntaxAnalyzer
{
    public class Parser
    {
        private List<Token> tokens;
        private int currentPos;
        private List<SyntaxError> errors;
        private Token currentToken;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens ?? new List<Token>();
            currentPos = 0;
            errors = new List<SyntaxError>();
            SkipWhitespaceAndNewLines();
        }

        private bool IsWhitespaceOrNewLine(Token token) =>
            token != null && (token.Type == TokenType.OperatorSpace ||
                              token.Type == TokenType.OperatorNewLine ||
                              token.Type == TokenType.OperatorTab);

        private void SkipWhitespaceAndNewLines()
        {
            while (currentPos < tokens.Count && IsWhitespaceOrNewLine(tokens[currentPos]))
                currentPos++;
            currentToken = currentPos < tokens.Count ? tokens[currentPos] : null;
        }

        private void NextToken()
        {
            currentPos++;
            SkipWhitespaceAndNewLines();
        }

        private void AddError(string message, string found, string expected)
        {
            int line = currentToken?.Line ?? 1;
            int col = currentToken?.StartColumn ?? 1;
            errors.Add(new SyntaxError
            {
                ErrorNumber = errors.Count + 1,
                Message = message,
                Expected = expected,
                Found = found,
                Line = line,
                Column = col
            });
        }

        private bool CheckToken(TokenType type) => currentToken != null && currentToken.Type == type;

        private void Expect(TokenType expectedType, string expectedLiteral)
        {
            while (currentToken != null && IsWhitespaceOrNewLine(currentToken))
                NextToken();

            if (currentToken == null)
            {
                AddError("Неожиданный конец файла", "EOF", expectedLiteral);
                return;
            }

            if (currentToken.Type == expectedType)
            {
                NextToken();
                return;
            }

            AddError($"Ожидалась лексема '{expectedLiteral}'",
                     currentToken.Value ?? "EOF", expectedLiteral);
            NextToken();
        }

        private bool IsComparisonOperator(Token token) =>
            token != null && (token.Type == TokenType.OperatorLess ||
                              token.Type == TokenType.OperatorGreater ||
                              token.Type == TokenType.OperatorLessEqual ||
                              token.Type == TokenType.OperatorGreaterEqual ||
                              token.Type == TokenType.OperatorEqual ||
                              token.Type == TokenType.OperatorNotEqual);

        public List<SyntaxError> Parse()
        {
            if (tokens.Count < 2 || currentToken == null)
            {
                AddError("Пустой входной файл", "EOF", "while ( условие ) { ... } ;");
                return errors;
            }

            if (!CheckToken(TokenType.KeywordWhile))
            {
                AddError("Ожидалось ключевое слово 'while'",
                         currentToken.Value ?? "EOF", "while");
            }

            NextToken();

            Expect(TokenType.DelimiterLParen, "(");
            ParseCondition();
            Expect(TokenType.DelimiterRParen, ")");
            Expect(TokenType.DelimiterLBrace, "{");
            ParseStatementBlock();
            Expect(TokenType.DelimiterRBrace, "}");
            Expect(TokenType.DelimiterSemicolon, ";");

            if (currentToken != null && currentToken.Type != TokenType.EndOfFile)
                AddError($"Неожиданная лексема после завершения программы: '{currentToken.Value}'",
                         currentToken.Value, "конец программы");

            return errors;
        }

        private void ParseCondition() => ParseLogicalOr();

        private void ParseLogicalOr()
        {
            ParseLogicalAnd();
            while (CheckToken(TokenType.OperatorLogicalOr))
            {
                NextToken();
                ParseLogicalAnd();
            }
        }

        private void ParseLogicalAnd()
        {
            ParseComparison();
            while (CheckToken(TokenType.OperatorLogicalAnd))
            {
                NextToken();
                ParseComparison();
            }
        }

        private void ParseComparison()
        {
            if (CheckToken(TokenType.DelimiterLParen))
            {
                NextToken();
                ParseCondition();
                Expect(TokenType.DelimiterRParen, ")");
                return;
            }

            ParseExpression();

            if (IsComparisonOperator(currentToken))
            {
                NextToken();
                ParseExpression();
            }
            else if (currentToken != null)
            {
                AddError("Ожидался оператор сравнения",
                         currentToken.Value ?? "EOF",
                         "<, >, <=, >=, ==, !=");
                NextToken();
                if (CheckToken(TokenType.Identifier) || CheckToken(TokenType.IntegerNumber))
                    ParseExpression();
            }
        }

        private void ParseExpression()
        {
            if (CheckToken(TokenType.Identifier) || CheckToken(TokenType.IntegerNumber))
                NextToken();
            else if (currentToken != null)
            {
                AddError("Ожидалась переменная или число",
                         currentToken.Value ?? "EOF", "идентификатор или число");
                NextToken();
                if (CheckToken(TokenType.Identifier) || CheckToken(TokenType.IntegerNumber))
                    NextToken();
            }
        }

        private void ParseStatementBlock() => ParseStatementList();

        private void ParseStatementList()
        {
            while (currentToken != null &&
                   currentToken.Type != TokenType.DelimiterRBrace &&
                   currentToken.Type != TokenType.EndOfFile)
            {
                if (IsValidStatement())
                    ParseStatement();
                else
                {
                    AddError("Неожиданная лексема в блоке операторов",
                             currentToken.Value ?? "EOF",
                             "оператор вида 'i++;' или 'j--;'");
                    NextToken();
                }
            }
        }

        private bool IsValidStatement()
        {
            if (currentToken?.Type != TokenType.Identifier) return false;
            int nextPos = currentPos + 1;
            while (nextPos < tokens.Count && IsWhitespaceOrNewLine(tokens[nextPos]))
                nextPos++;
            if (nextPos >= tokens.Count) return false;
            Token next = tokens[nextPos];
            return next.Type == TokenType.OperatorIncrement ||
                   next.Type == TokenType.OperatorDecrement;
        }

        private void ParseStatement()
        {
            ParseIncrementStatement();
            Expect(TokenType.DelimiterSemicolon, ";");
        }

        private void ParseIncrementStatement()
        {
            if (CheckToken(TokenType.Identifier))
            {
                string varName = currentToken.Value;
                NextToken();

                while (currentToken != null && IsWhitespaceOrNewLine(currentToken))
                    NextToken();

                if (CheckToken(TokenType.OperatorIncrement) || CheckToken(TokenType.OperatorDecrement))
                    NextToken();
                else if (currentToken != null)
                {
                    AddError($"Ожидался оператор '++' или '--' после переменной '{varName}'",
                             currentToken.Value ?? "EOF", "++ или --");
                    NextToken();
                }
                else
                    AddError($"Ожидался оператор '++' или '--' после переменной '{varName}'",
                             "EOF", "++ или --");
            }
            else if (currentToken != null)
            {
                AddError("Ожидалось имя переменной для инкремента/декремента",
                         currentToken.Value ?? "EOF", "идентификатор");
                NextToken();
            }
        }
    }
}
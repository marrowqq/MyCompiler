using MyCompiler.LexicalAnalyzer;
using System;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace MyCompiler.SyntaxAnalyzer
{
    public class Parser
    {
        private List<Token> tokens;
        private List<Token> _tokens;
        private int currentPos;
        private List<SyntaxError> errors;
        private Token currentToken;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens ?? new List<Token>();
            currentPos = 0;
            errors = new List<SyntaxError>();
            combinetokens();
            SetCurrentToken();
            
        }
        private void combinetokens()
        {
            if (this.tokens == null || this.tokens.Count == 0) return;

            List<Token> combined = new List<Token>();
            int i = 0;

            while (i < this.tokens.Count)
            {
                Token current = this.tokens[i];

                Token combinedToken = new Token(
                    current.Type,
                    current.Value,
                    current.Line,
                    current.StartColumn,
                    current.EndColumn
                );

                int j = 1;
                while (i + j < this.tokens.Count && this.tokens[i + j].Type == combinedToken.Type)
                {
                    if (current.Type == TokenType.Error && this.tokens[i + j].Type == TokenType.Error)
                    {
                        string nextVal = this.tokens[i + j].Value;


                        combinedToken.Value += nextVal;
                        combinedToken.EndColumn = this.tokens[i + j].EndColumn;
                        j++;
                    }

                    

                }

                combined.Add(combinedToken);
                i += j;
            }

            this.tokens = combined;
        }

        private void SetCurrentToken()

        {

            while (currentPos < tokens.Count)

            {

                var t = tokens[currentPos];



                if (IsWhitespaceOrNewLine(t))

                {

                    currentPos++;

                    continue;

                }



                if (t.Type == TokenType.Error)

                {

                    AddError($"Недопустимый символ в коде: '{t.Value}'", t.Value, "валидная лексема");

                    currentPos++;

                    continue;

                }



                break;

            }

            currentToken = currentPos < tokens.Count ? tokens[currentPos] : null;

        }

        private bool IsWhitespaceOrNewLine(Token token) =>
            token != null && (token.Type == TokenType.OperatorSpace ||
                              token.Type == TokenType.OperatorNewLine ||
                              token.Type == TokenType.OperatorTab);

        private void NextToken()
        {
            if (currentPos < tokens.Count) currentPos++;
            SetCurrentToken();
        }

        private void AddError(string message, string found, string expected)
        {
            errors.Add(new SyntaxError
            {
                ErrorNumber = errors.Count + 1,
                Message = message,
                Expected = expected,
                Found = found,
                Line = currentToken?.Line ?? (tokens.Count > 0 ? tokens[^1].Line : 1),
                Column = currentToken?.StartColumn ?? (tokens.Count > 0 ? tokens[^1].StartColumn : 1)
            });
        }

        public List<SyntaxError> Parse()
        {
            if (currentToken == null) return errors;

            if (CheckToken(TokenType.KeywordWhile))
            {
                NextToken();
            }
            else
            {
                if (currentPos == 0)
                {
                    AddError("Ожидалось 'while' в начале программы", currentToken.Value, "while");
                    if (!CheckToken(TokenType.DelimiterLParen)) NextToken();
                }
                
            }

            if (!CheckToken(TokenType.DelimiterLParen))
            {
                bool foundLParen = false;
                for (int i = currentPos; i < Math.Min(currentPos + 10, tokens.Count); i++)
                {
                    if (tokens[i].Type == TokenType.DelimiterLParen) { foundLParen = true; break; }
                }

                if (foundLParen)
                {
                    while (currentToken != null && !CheckToken(TokenType.DelimiterLParen)) NextToken();
                }
                else
                {
                    AddError("Пропущена открывающая скобка '('", currentToken?.Value, "(");
                }
            }

            if (CheckToken(TokenType.DelimiterLParen))
            {
                NextToken();
                ParseCondition();
                Expect(TokenType.DelimiterRParen, ")");
            }

            bool hasLBrace = CheckToken(TokenType.DelimiterLBrace);
            if (hasLBrace) NextToken();
            else AddError("Пропущена открывающая скобка '{'", currentToken?.Value, "{");

            ParseStatementList();

            if (hasLBrace) Expect(TokenType.DelimiterRBrace, "}");
            else if (CheckToken(TokenType.DelimiterRBrace)) NextToken();

            Expect(TokenType.DelimiterSemicolon, ";");

            while (currentToken != null && currentToken.Type != TokenType.EndOfFile)
            {
                AddError("Лишняя лексема после программы", currentToken.Value, "конец файла");
                NextToken();
            }

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
            }
            else
            {
                ParsePrimary();
                if (IsComparisonOp(currentToken?.Type) && (currentToken.Value == "<=" ||currentToken.Value == "<" 
                    || currentToken.Value == "==" || currentToken.Value == ">=" || currentToken.Value == ">"
                    || currentToken.Value == "!="))
                {
                    NextToken();
                    ParsePrimary();
                }
                else
                {
                    AddError("Ожидался оператор сравнения", currentToken?.Value, "< <= > >= != ==");
                    NextToken();
                    ParsePrimary();
                }
            }
        }

        private void ParsePrimary()
        {
            if (CheckToken(TokenType.Identifier) || CheckToken(TokenType.IntegerNumber))
                NextToken();
            else
                AddError("Ожидалось число или переменная", currentToken?.Value, "ID/NUM");
        }

        private void ParseStatementList()
        {
            while (currentToken != null &&
                   !CheckToken(TokenType.DelimiterRBrace) &&
                   !CheckToken(TokenType.DelimiterSemicolon) &&
                   !CheckToken(TokenType.EndOfFile))
            {
                ParseStatement();
            }
        }

        private void ParseStatement()
        {
            if (CheckToken(TokenType.Identifier))
            {
                string id = currentToken.Value;
                NextToken();
                if (CheckToken(TokenType.OperatorIncrement) || CheckToken(TokenType.OperatorDecrement))
                {
                    NextToken();
                }
                else
                {
                    AddError($"Некорректная операция для '{id}'", currentToken?.Value, "++ или --");
                    while (currentToken != null && !CheckToken(TokenType.DelimiterSemicolon) && !CheckToken(TokenType.DelimiterRBrace)) NextToken();
                }
                Expect(TokenType.DelimiterSemicolon, ";");
            }
            else
            {
                AddError("Неизвестный оператор", currentToken?.Value, "инкремент (i++)");
                NextToken(); 
            }
        }

        private bool CheckToken(TokenType type) => currentToken != null && currentToken.Type == type;

        private void Expect(TokenType type, string literal)
        {
            if (CheckToken(type)) NextToken();
            else AddError($"Ожидалось '{literal}'", currentToken?.Value ?? "EOF", literal);
        }

        private bool IsComparisonOp(TokenType? type) =>
            type == TokenType.OperatorLess || type == TokenType.OperatorGreater ||
            type == TokenType.OperatorEqual || type == TokenType.OperatorNotEqual ||
            type == TokenType.OperatorLessEqual || type == TokenType.OperatorGreaterEqual;
    }
}
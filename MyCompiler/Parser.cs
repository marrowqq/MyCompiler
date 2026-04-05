using System;
using System.Collections.Generic;
using System.Linq;
using MyCompiler.LexicalAnalyzer;

namespace MyCompiler.SyntaxAnalyzer
{
    public class Parser
    {
        private List<Token> tokens;
        private int currentPos;
        private List<SyntaxError> errors;
        private Token currentToken;
        private Stack<ParseTreeNode> parseStack;
        private HashSet<TokenType> expectedSymbols;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens ?? new List<Token>();
            currentPos = 0;
            errors = new List<SyntaxError>();
            parseStack = new Stack<ParseTreeNode>();
            expectedSymbols = new HashSet<TokenType>();

            SkipWhitespaceAndNewLines();
        }

        private bool IsWhitespaceOrNewLine(Token token)
        {
            if (token == null) return false;
            return token.Type == TokenType.OperatorSpace ||
                   token.Type == TokenType.OperatorNewLine ||
                   token.Type == TokenType.OperatorTab;
        }

        private void SkipWhitespaceAndNewLines()
        {
            while (currentPos < tokens.Count && IsWhitespaceOrNewLine(tokens[currentPos]))
            {
                currentPos++;
            }

            if (currentPos < tokens.Count)
                currentToken = tokens[currentPos];
            else
                currentToken = null;
        }

        private void NextToken()
        {
            currentPos++;
            SkipWhitespaceAndNewLines();
        }
        private void SkipErrorSymbol()
        {
            if (currentToken != null)
            {
                int line = currentToken.Line;
                int column = currentToken.StartColumn;

                errors.Add(new SyntaxError
                {
                    ErrorNumber = errors.Count + 1,
                    Message = $"Неверное место для лексемы '{currentToken.Value}'",
                    Expected = "что-то",
                    Found = currentToken.Value,
                    Line = line,
                    Column = column
                });
                NextToken();
            }
        }
        private bool CanRecoverFromError()
        {
            if (currentToken == null) return false;
            switch (currentToken.Type)
            {
                case TokenType.KeywordWhile:
                case TokenType.DelimiterRParen:
                case TokenType.DelimiterRBrace:
                case TokenType.DelimiterSemicolon:
                    return true;
                default:
                    return false;
            }
        }

        private void BuildExpectedSet()
        {
            expectedSymbols.Clear();

            foreach (var node in parseStack)
            {
                var firstSet = GetFirstSet(node.NonTerminal);
                foreach (var symbol in firstSet)
                {
                    expectedSymbols.Add(symbol);
                }
            }
        }

        private HashSet<TokenType> GetFirstSet(string nonTerminal)
        {
            var result = new HashSet<TokenType>();

            switch (nonTerminal)
            {
                case "Start":
                    result.Add(TokenType.KeywordWhile);
                    break;
                case "Condition":
                case "LogicalOr":
                case "LogicalAnd":
                case "Comparison":
                    result.Add(TokenType.Identifier);
                    result.Add(TokenType.IntegerNumber);
                    result.Add(TokenType.DelimiterLParen);
                    break;
                case "Expression":
                    result.Add(TokenType.Identifier);
                    result.Add(TokenType.IntegerNumber);
                    break;
                case "StatementBlock":
                case "StatementList":
                case "Statement":
                    result.Add(TokenType.Identifier);
                    break;
                case "IncrementStatement":
                    result.Add(TokenType.Identifier);
                    break;
            }

            return result;
        }

        private bool CanCurrentTokenBeDerived()
        {
            if (currentToken == null) return false;

            foreach (var expected in expectedSymbols)
            {
                if (currentToken.Type == expected)
                    return true;
            }
            return false;
        }
        private void AddError(string message, string found, string expected)
        {
            int line = currentToken?.Line ?? 1;
            int column = currentToken?.StartColumn ?? 1;

            errors.Add(new SyntaxError
            {
                ErrorNumber = errors.Count + 1,
                Message = message,
                Expected = expected,
                Found = found,
                Line = line,
                Column = column
            });
        }

        private bool IsComparisonOperator(Token token)
        {
            if (token == null) return false;
            return token.Type == TokenType.OperatorLess ||
                   token.Type == TokenType.OperatorGreater ||
                   token.Type == TokenType.OperatorLessEqual ||
                   token.Type == TokenType.OperatorGreaterEqual ||
                   token.Type == TokenType.OperatorEqual ||
                   token.Type == TokenType.OperatorNotEqual;
        }

        private bool CheckToken(TokenType expectedType)
        {
            if (currentToken == null) return false;
            return currentToken.Type == expectedType;
        }

        private void Expect(TokenType expectedType, string expectedDescription)
        {
            while (currentToken != null && IsWhitespaceOrNewLine(currentToken))
            {
                NextToken();
            }

            if (currentToken == null)
            {
                AddError($"Неожиданный конец файла", "EOF", expectedDescription);
                return;
            }

            if (currentToken.Type == expectedType)
            {
                NextToken();
                return;
            }
            AddError($"Ожидался {expectedDescription}",
                    currentToken.Value ?? "EOF",
                    expectedDescription);
            while (currentToken != null &&
                   currentToken.Type != expectedType &&
                   currentToken.Type != TokenType.DelimiterSemicolon &&
                   currentToken.Type != TokenType.DelimiterRBrace &&
                   currentToken.Type != TokenType.KeywordWhile &&
                   currentToken.Type != TokenType.DelimiterRParen)
            {
                AddError($"Удален ошибочный символ '{currentToken.Value}'",
                        currentToken.Value,
                        expectedDescription);

                NextToken();

                while (currentToken != null && IsWhitespaceOrNewLine(currentToken))
                {
                    NextToken();
                }
            }
            if (currentToken != null && currentToken.Type == expectedType)
            {
                NextToken();
            }
        }

        public List<SyntaxError> Parse()
        {
            try
            {
                if (tokens.Count == 0 || currentToken == null)
                {
                    AddError("Пустой входной файл", "EOF", "конструкция while");
                    return errors;
                }

                ParseStart();

                if (currentToken != null && currentToken.Type != TokenType.EndOfFile)
                {
                    AddError($"Неожиданная лексема после завершения программы: '{currentToken.Value}'",
                             currentToken.Value, "конец программы");
                }
            }
            catch (Exception ex)
            {
                AddError($"Критическая ошибка разбора: {ex.Message}", "", "");
            }

            return errors;
        }

        private void ParseStart()
        {
            if (CheckToken(TokenType.KeywordWhile))
            {
                NextToken();

                Expect(TokenType.DelimiterLParen, "'('");
                ParseCondition();
                Expect(TokenType.DelimiterRParen, "')'");
                Expect(TokenType.DelimiterLBrace, "'{'");
                ParseStatementBlock();
                Expect(TokenType.DelimiterRBrace, "'}'");
                Expect(TokenType.DelimiterSemicolon, "';'");
            }
            else
            {
                AddError($"Ожидалось ключевое слово 'while' в начале программы",
                        currentToken?.Value ?? "EOF", "while");

                while (currentToken != null && currentToken.Type != TokenType.KeywordWhile)
                {
                    AddError($"Удален ошибочный символ '{currentToken.Value}'",
                            currentToken.Value,
                            "while");
                    NextToken();
                }

                if (currentToken != null && currentToken.Type == TokenType.KeywordWhile)
                {
                    NextToken();
                    Expect(TokenType.DelimiterLParen, "'('");
                    ParseCondition();
                    Expect(TokenType.DelimiterRParen, "')'");
                    Expect(TokenType.DelimiterLBrace, "'{'");
                    ParseStatementBlock();
                    Expect(TokenType.DelimiterRBrace, "'}'");
                    Expect(TokenType.DelimiterSemicolon, "';'");
                }
            }
        }

        private void ParseCondition()
        {
            parseStack.Push(new ParseTreeNode("Condition"));
            ParseLogicalOr();
            parseStack.Pop();
        }

        private void ParseLogicalOr()
        {
            parseStack.Push(new ParseTreeNode("LogicalOr"));
            ParseLogicalAnd();

            while (CheckToken(TokenType.OperatorLogicalOr))
            {
                NextToken();
                ParseLogicalAnd();
            }
            parseStack.Pop();
        }

        private void ParseLogicalAnd()
        {
            parseStack.Push(new ParseTreeNode("LogicalAnd"));
            ParseComparison();

            while (CheckToken(TokenType.OperatorLogicalAnd))
            {
                NextToken();
                ParseComparison();
            }
            parseStack.Pop();
        }

        private void ParseComparison()
        {
            parseStack.Push(new ParseTreeNode("Comparison"));

            if (CheckToken(TokenType.DelimiterLParen))
            {
                NextToken();
                ParseCondition();
                Expect(TokenType.DelimiterRParen, "')'");
            }
            else
            {
                ParseExpression();

                if (IsComparisonOperator(currentToken))
                {
                    NextToken();
                    ParseExpression();
                }
                else if (currentToken != null)
                {
                    AddError($"Ожидался оператор сравнения",
                            currentToken.Value ?? "EOF",
                            "<, >, <=, >=, ==, !=");

                    while (currentToken != null && !IsComparisonOperator(currentToken))
                    {
                        SkipErrorSymbol();
                    }

                    if (IsComparisonOperator(currentToken))
                    {
                        NextToken();
                        ParseExpression();
                    }
                }
            }

            parseStack.Pop();
        }

        private void ParseExpression()
        {
            parseStack.Push(new ParseTreeNode("Expression"));

            if (CheckToken(TokenType.Identifier))
            {
                NextToken();
            }
            else if (CheckToken(TokenType.IntegerNumber))
            {
                NextToken();
            }
            else if (currentToken != null)
            {
                AddError($"Ожидалась переменная или число",
                        currentToken.Value ?? "EOF",
                        "идентификатор или число");

                while (currentToken != null &&
                       currentToken.Type != TokenType.Identifier &&
                       currentToken.Type != TokenType.IntegerNumber)
                {
                    SkipErrorSymbol();
                }

                if (CheckToken(TokenType.Identifier) || CheckToken(TokenType.IntegerNumber))
                {
                    NextToken();
                }
            }

            parseStack.Pop();
        }

        private void ParseStatementBlock()
        {
            parseStack.Push(new ParseTreeNode("StatementBlock"));
            ParseStatementList();
            parseStack.Pop();
        }

        private void ParseStatementList()
        {
            parseStack.Push(new ParseTreeNode("StatementList"));

            while (currentToken != null &&
                   currentToken.Type != TokenType.DelimiterRBrace &&
                   currentToken.Type != TokenType.EndOfFile)
            {
                if (IsValidStatement())
                {
                    ParseStatement();
                }
                else
                {
                    AddError($"Неожиданная лексема в блоке операторов",
                            currentToken.Value ?? "EOF",
                            "оператор инкремента/декремента (например, i++; или j--;)");

                    while (currentToken != null && !IsValidStatement() &&
                           currentToken.Type != TokenType.DelimiterRBrace)
                    {
                        SkipErrorSymbol();
                    }
                }
            }

            parseStack.Pop();
        }

        private bool IsValidStatement()
        {
            if (currentToken == null) return false;

            if (currentToken.Type == TokenType.Identifier)
            {
                int nextPos = currentPos + 1;
                while (nextPos < tokens.Count && IsWhitespaceOrNewLine(tokens[nextPos]))
                {
                    nextPos++;
                }
                if (nextPos < tokens.Count)
                {
                    Token nextToken = tokens[nextPos];
                    return nextToken.Type == TokenType.OperatorIncrement ||
                           nextToken.Type == TokenType.OperatorDecrement;
                }
            }
            return false;
        }

        private void ParseStatement()
        {
            ParseIncrementStatement();
            Expect(TokenType.DelimiterSemicolon, "';'");
        }

        private void ParseIncrementStatement()
        {
            parseStack.Push(new ParseTreeNode("IncrementStatement"));

            if (CheckToken(TokenType.Identifier))
            {
                string varName = currentToken.Value;
                NextToken();

                while (currentToken != null && IsWhitespaceOrNewLine(currentToken))
                {
                    NextToken();
                }

                if (CheckToken(TokenType.OperatorIncrement) || CheckToken(TokenType.OperatorDecrement))
                {
                    NextToken();
                }
                else if (currentToken != null)
                {
                    AddError($"Ожидался оператор '++' или '--' после переменной '{varName}'",
                            currentToken.Value ?? "EOF",
                            "++ или --");

                    while (currentToken != null &&
                           !CheckToken(TokenType.OperatorIncrement) &&
                           !CheckToken(TokenType.OperatorDecrement))
                    {
                        SkipErrorSymbol();
                    }

                    if (CheckToken(TokenType.OperatorIncrement) || CheckToken(TokenType.OperatorDecrement))
                    {
                        NextToken();
                    }
                }
                else
                {
                    AddError($"Ожидался оператор '++' или '--' после переменной '{varName}'",
                            "EOF",
                            "++ или --");
                }
            }
            else if (currentToken != null)
            {
                AddError($"Ожидалось имя переменной для инкремента/декремента",
                        currentToken.Value ?? "EOF",
                        "идентификатор");

                while (currentToken != null && currentToken.Type != TokenType.Identifier)
                {
                    SkipErrorSymbol();
                }

                if (CheckToken(TokenType.Identifier))
                {
                    NextToken();
                    while (currentToken != null && IsWhitespaceOrNewLine(currentToken))
                    {
                        NextToken();
                    }
                    if (CheckToken(TokenType.OperatorIncrement) || CheckToken(TokenType.OperatorDecrement))
                    {
                        NextToken();
                    }
                }
            }

            parseStack.Pop();
        }
    }
    public class ParseTreeNode
    {
        public string NonTerminal { get; set; }
        public List<ParseTreeNode> Children { get; set; }

        public ParseTreeNode(string nonTerminal)
        {
            NonTerminal = nonTerminal;
            Children = new List<ParseTreeNode>();
        }
    }
}
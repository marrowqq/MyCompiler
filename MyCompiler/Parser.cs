using System;
using System.Collections.Generic;
using System.Linq;
using MyCompiler.LexicalAnalyzer;

namespace MyCompiler.SyntaxAnalyzer
{
    public class Parser
    {
        private readonly List<Token> _tokens;
        private int _currentPos;
        private Token _currentToken;
        public List<SyntaxError> Errors { get; }

        public Parser(List<Token> tokens)
        {
            _tokens = tokens?.Where(t => !IsIgnorable(t)).ToList() ?? new List<Token>();
            _currentPos = 0;
            Errors = new List<SyntaxError>();
            UpdateCurrentToken();
        }

        private bool IsIgnorable(Token t) =>
            t.Type == TokenType.OperatorSpace ||
            t.Type == TokenType.OperatorNewLine ||
            t.Type == TokenType.OperatorTab;

        private void UpdateCurrentToken()
        {
            _currentToken = _currentPos < _tokens.Count ? _tokens[_currentPos] : null;
        }

        private void NextToken()
        {
            if (_currentPos < _tokens.Count) _currentPos++;
            UpdateCurrentToken();
        }

        private bool Check(TokenType type) => _currentToken != null && _currentToken.Type == type;

        private void Consume(TokenType type, string message)
        {
            if (Check(type))
            {
                NextToken();
                return;
            }

            AddError(message, type.ToString());

            // Если токен не тот, что ждали, мы его "съедаем", только если это не важный разделитель.
            // Это позволяет парсеру перейти к следующему шагу и не выдавать 10 ошибок на одну опечатку.
            if (_currentToken != null && !IsSyncToken(_currentToken.Type) && _currentToken.Type != TokenType.EndOfFile)
            {
                NextToken();
            }
        }

        private bool IsSyncToken(TokenType type)
        {
            return type == TokenType.DelimiterRParen ||
                   type == TokenType.DelimiterLBrace ||
                   type == TokenType.DelimiterRBrace ||
                   type == TokenType.DelimiterSemicolon;
        }

        private void AddError(string message, string expected)
        {
            Errors.Add(new SyntaxError
            {
                ErrorNumber = Errors.Count + 1,
                Message = message,
                Expected = expected,
                Found = _currentToken?.Value ?? "EOF",
                Line = _currentToken?.Line ?? 0,
                Column = _currentToken?.StartColumn ?? 0
            });
        }

        private void Synchronize(params TokenType[] syncTokens)
        {
            var set = new HashSet<TokenType>(syncTokens);
            while (_currentToken != null && !set.Contains(_currentToken.Type) && _currentToken.Type != TokenType.EndOfFile)
            {
                NextToken();
            }
        }

        public List<SyntaxError> Parse()
        {
            // 1. Поиск ключевого слова while
            if (Check(TokenType.KeywordWhile))
            {
                NextToken();
            }
            else
            {
                AddError("Цикл должен начинаться с 'while'", "while");
                Synchronize(TokenType.DelimiterLParen);
            }

            // --- ФИКС ДЛЯ "while;" ---
            if (Check(TokenType.DelimiterSemicolon))
            {
                AddError("Лишняя ';' после while. Условие должно идти сразу в скобках.", "(");
                NextToken();
            }

            // 2. Парсинг условия в скобках
            Consume(TokenType.DelimiterLParen, "Ожидалась '(' после 'while'");
            ParseCondition();
            Consume(TokenType.DelimiterRParen, "Ожидалась ')' после условия");

            // 3. СТРОГИЙ ПАРСИНГ ТЕЛА (только в {})
            Consume(TokenType.DelimiterLBrace, "Ожидалась '{' перед телом цикла");

            // Внутри блока может быть пусто, или могут быть инструкции. 
            // ParseStatementBlock() сам будет крутиться, пока не встретит '}'
            ParseStatementBlock();

            Consume(TokenType.DelimiterRBrace, "Ожидалась '}' в конце блока");

            // 4. СТРОГАЯ ТОЧКА С ЗАПЯТОЙ В КОНЦЕ
            Consume(TokenType.DelimiterSemicolon, "Ожидалась ';' в конце конструкции цикла");

            // 5. ПРОВЕРКА НА МУСОР (Конец файла)
            if (_currentToken != null && _currentToken.Type != TokenType.EndOfFile)
            {
                AddError($"Неожиданный символ после завершения цикла: '{_currentToken.Value}'", "Конец файла");

                // Пропускаем мусор
                while (_currentToken != null && _currentToken.Type != TokenType.EndOfFile)
                {
                    NextToken();
                }
            }

            return Errors;
        }

        private void ParseCondition() => ParseLogicalOr();

        private void ParseLogicalOr()
        {
            ParseLogicalAnd();
            while (Check(TokenType.OperatorLogicalOr))
            {
                NextToken();
                ParseLogicalAnd();
            }
        }

        private void ParseLogicalAnd()
        {
            ParseComparison();
            while (Check(TokenType.OperatorLogicalAnd))
            {
                NextToken();
                ParseComparison();
            }
        }

        private void ParseComparison()
        {
            ParsePrimary();
            if (IsComparisonOperator(_currentToken))
            {
                NextToken();
                ParsePrimary();
            }
            // Если видим ID или число сразу после другого операнда — забыт знак сравнения
            else if (_currentToken != null && (_currentToken.Type == TokenType.Identifier || _currentToken.Type == TokenType.IntegerNumber))
            {
                AddError("Ожидался оператор сравнения (==, !=, <, >, <=, >=)", "Comparison Operator");
                NextToken(); // Пропускаем лишний операнд, чтобы не ломать скобки
            }
        }

        private void ParseStatementBlock()
        {
            while (_currentToken != null && !Check(TokenType.DelimiterRBrace) && !Check(TokenType.EndOfFile))
            {
                ParseStatement();
            }
        }

        private void ParsePrimary()
        {
            if (Check(TokenType.DelimiterLParen))
            {
                NextToken(); // Потребляем '('
                ParseCondition(); // Рекурсивный запуск парсинга условий
                Consume(TokenType.DelimiterRParen, "Ожидалась закрывающая ')'");
            }
            else if (Check(TokenType.Identifier) || Check(TokenType.IntegerNumber))
            {
                NextToken();
            }
            else
            {
                AddError("Ожидалось имя переменной, число или '('", "Выражение");
                // Синхронизация: не даем парсеру зависнуть на мусорном токене
                if (_currentToken != null && !IsSyncToken(_currentToken.Type))
                {
                    NextToken();
                }
            }
        }

        private void ParseStatement()
        {
            if (Check(TokenType.Identifier))
            {
                NextToken(); // Потребляем имя переменной (например, 'i')

                if (Check(TokenType.OperatorIncrement) || Check(TokenType.OperatorDecrement))
                {
                    NextToken(); // Потребляем '++' или '--'
                }
                else
                {
                    AddError("После переменной ожидался инкремент или декремент", "++ / --");
                    // Если там что-то другое (например, '+'), скипаем это до точки с запятой
                    if (!Check(TokenType.DelimiterSemicolon)) NextToken();
                }

                Consume(TokenType.DelimiterSemicolon, "Инструкция должна заканчиваться ';'");
            }
            else if (!Check(TokenType.DelimiterRBrace)) // Если это не конец блока, значит инструкция просто битая
            {
                AddError("Некорректная инструкция. Ожидалось выражение типа 'i++;'", "Statement");
                Synchronize(TokenType.DelimiterSemicolon, TokenType.DelimiterRBrace);
                if (Check(TokenType.DelimiterSemicolon)) NextToken();
            }
        }

        private bool IsComparisonOperator(Token t)
        {
            if (t == null) return false;
            return t.Type == TokenType.OperatorLess ||
                   t.Type == TokenType.OperatorGreater ||
                   t.Type == TokenType.OperatorLessEqual ||
                   t.Type == TokenType.OperatorGreaterEqual ||
                   t.Type == TokenType.OperatorEqual ||
                   t.Type == TokenType.OperatorNotEqual;
        }

    }
}
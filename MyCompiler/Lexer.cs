using System;
using System.Collections.Generic;
using System.Text;

namespace MyCompiler.LexicalAnalyzer
{
    public class Lexer
    {
        private string _input;
        private int _position;
        private int _line;
        private int _column;
        private List<Token> _tokens;
        private List<LexicalError> _errors;

        private enum State
        {
            Start = 0,
            InLetter = 1,
            InDigit = 2,
            InLParen = 3,
            InRParen = 4,
            InSemicolon = 5,
            InLBrace = 6,
            InRBrace = 7,
            InPlus = 8,
            InMinus = 9,
            InSpace = 10,
            InLess = 11,
            InGreater = 12,
            InError = 13,
            InIncrement = 14,
            InDecrement = 15,
            InLessEqual = 16,
            InGreaterEqual = 17,
            InNewLine = 20,
            InTab = 21,
            InEqual,
            InNot,
            InAmpersand,
            InPipe
        }

        public Lexer()
        {
            _tokens = new List<Token>();
            _errors = new List<LexicalError>();
        }

        public List<Token> Analyze(string input)
        {
            _input = input;
            _position = 0;
            _line = 1;
            _column = 1;
            _tokens.Clear();
            _errors.Clear();

            while (_position < _input.Length)
            {
                Token token = GetNextToken();

                if (token != null)
                {
                    _tokens.Add(token);

                    if (token.IsError)
                    {
                        _errors.Add(new LexicalError(
                            token.Line,
                            token.StartColumn,
                            token.ErrorMessage,
                            token.Value));
                    }
                }
            }

            return _tokens;
        }

        private Token GetNextToken()
        {
            if (_position >= _input.Length) return null;

            State state = State.Start;
            StringBuilder buffer = new StringBuilder();
            int startLine = _line;
            int startColumn = _column;
            char currentChar;

            while (_position < _input.Length)
            {
                currentChar = _input[_position];

                switch (state)
                {
                    case State.Start:
                        buffer.Clear();
                        startLine = _line;
                        startColumn = _column;

                        if (char.IsLetter(currentChar))
                        {
                            state = State.InLetter;
                            buffer.Append(currentChar);
                            _position++;
                            _column++;
                        }
                        else if (char.IsDigit(currentChar))
                        {
                            state = State.InDigit;
                            buffer.Append(currentChar);
                            _position++;
                            _column++;
                        }
                        else if (currentChar == '(')
                        {
                            _position++;
                            _column++;
                            return new Token(TokenType.DelimiterLParen, "(",
                                startLine, startColumn, _column - 1);
                        }
                        else if (currentChar == ')')
                        {
                            _position++;
                            _column++;
                            return new Token(TokenType.DelimiterRParen, ")",
                                startLine, startColumn, _column - 1);
                        }
                        else if (currentChar == ';')
                        {
                            _position++;
                            _column++;
                            return new Token(TokenType.DelimiterSemicolon, ";",
                                startLine, startColumn, _column - 1);
                        }
                        else if (currentChar == '{')
                        {
                            _position++;
                            _column++;
                            return new Token(TokenType.DelimiterLBrace, "{",
                                startLine, startColumn, _column - 1);
                        }
                        else if (currentChar == '}')
                        {
                            _position++;
                            _column++;
                            return new Token(TokenType.DelimiterRBrace, "}",
                                startLine, startColumn, _column - 1);
                        }
                        else if (currentChar == ' ')
                        {
                            _position++;
                            _column++;
                            return new Token(TokenType.OperatorSpace, " ",
                                startLine, startColumn, _column - 1);
                        }
                        else if (currentChar == '\t')
                        {
                            _position++;
                            _column++;
                            return new Token(TokenType.OperatorTab, "\t",
                                startLine, startColumn, _column - 1);
                        }
                        else if (currentChar == '\n')
                        {
                            _position++;
                            int endCol = _column;
                            _line++;
                            _column = 1;
                            return new Token(TokenType.OperatorNewLine, "\\n",
                                startLine, startColumn, endCol);
                        }
                        else if (currentChar == '\r')
                        {
                            _position++;
                            continue;
                        }
                        else if (currentChar == '+')
                        {
                            state = State.InPlus;
                            buffer.Append(currentChar);
                            _position++;
                            _column++;
                        }
                        else if (currentChar == '-')
                        {
                            state = State.InMinus;
                            buffer.Append(currentChar);
                            _position++;
                            _column++;
                        }
                        else if (currentChar == '<')
                        {
                            state = State.InLess;
                            buffer.Append(currentChar);
                            _position++;
                            _column++;
                        }
                        else if (currentChar == '>')
                        {
                            state = State.InGreater;
                            buffer.Append(currentChar);
                            _position++;
                            _column++;
                        }
                        else if (currentChar == '=')
                        {
                            state = State.InEqual;
                            buffer.Append(currentChar);
                            _position++;
                            _column++;
                        }
                        else if (currentChar == '!')
                        {
                            state = State.InNot;
                            buffer.Append(currentChar);
                            _position++;
                            _column++;
                        }
                        else if (currentChar == '&')
                        {
                            state = State.InAmpersand;
                            buffer.Append(currentChar);
                            _position++;
                            _column++;
                        }
                        else if (currentChar == '|')
                        {
                            state = State.InPipe;
                            buffer.Append(currentChar);
                            _position++;
                            _column++;
                        }
                        else
                        {
                            // Недопустимый символ: создаем ошибку, сдвигаем позицию на 1 и возвращаем
                            Token err = CreateErrorToken($"Недопустимый символ '{currentChar}'",
                                startLine, startColumn);
                            _position++;
                            _column++;
                            return err;
                        }
                        break;

                    case State.InLetter:
                        if (char.IsLetterOrDigit(currentChar))
                        {
                            buffer.Append(currentChar);
                            _position++;
                            _column++;
                        }
                        else
                        {
                            return CreateIdentifierToken(buffer.ToString(),
                                startLine, startColumn, _column - 1);
                        }
                        break;

                    case State.InDigit:
                        if (char.IsDigit(currentChar))
                        {
                            buffer.Append(currentChar);
                            _position++;
                            _column++;
                        }
                        else
                        {
                            return new Token(TokenType.IntegerNumber, buffer.ToString(),
                                startLine, startColumn, _column - 1);
                        }
                        break;

                    case State.InPlus:
                        if (currentChar == '+')
                        {
                            _position++;
                            _column++;
                            return new Token(TokenType.OperatorIncrement, "++",
                                startLine, startColumn, _column - 1);
                        }
                        else
                        {
                            return new Token(TokenType.OperatorPlus, "+",
                                startLine, startColumn, _column - 1);
                        }

                    case State.InMinus:
                        if (currentChar == '-')
                        {
                            _position++;
                            _column++;
                            return new Token(TokenType.OperatorDecrement, "--",
                                startLine, startColumn, _column - 1);
                        }
                        else
                        {
                            return new Token(TokenType.OperatorMinus, "-",
                                startLine, startColumn, _column - 1);
                        }

                    case State.InLess:
                        if (currentChar == '=')
                        {
                            _position++;
                            _column++;
                            return new Token(TokenType.OperatorLessEqual, "<=",
                                startLine, startColumn, _column - 1);
                        }
                        else
                        {
                            return new Token(TokenType.OperatorLess, "<",
                                startLine, startColumn, _column - 1);
                        }

                    case State.InGreater:
                        if (currentChar == '=')
                        {
                            _position++;
                            _column++;
                            return new Token(TokenType.OperatorGreaterEqual, ">=",
                                startLine, startColumn, _column - 1);
                        }
                        else
                        {
                            return new Token(TokenType.OperatorGreater, ">",
                                startLine, startColumn, _column - 1);
                        }

                    case State.InEqual:
                        // Уже считали '=', проверяем, не '==' ли
                        if (currentChar == '=')
                        {
                            _position++;
                            _column++;
                            return new Token(TokenType.OperatorEqual, "==",
                                startLine, startColumn, _column - 1);
                        }
                        else
                        {
                            // Одиночный '=' - ошибка, но символ '=' уже съеден, возвращаем ошибку
                            // Позицию не сдвигаем, т.к. мы уже её сдвинули при чтении '='
                            Token err = CreateErrorToken($"Одиночный '=' недопустим, ожидалось '=='",
                                startLine, startColumn);
                            return err;
                        }

                    case State.InNot:
                        if (currentChar == '=')
                        {
                            _position++;
                            _column++;
                            return new Token(TokenType.OperatorNotEqual, "!=",
                                startLine, startColumn, _column - 1);
                        }
                        else
                        {
                            Token err = CreateErrorToken($"Одиночный '!' недопустим, ожидалось '!='",
                                startLine, startColumn);
                            return err;
                        }

                    case State.InAmpersand:
                        if (currentChar == '&')
                        {
                            _position++;
                            _column++;
                            return new Token(TokenType.OperatorLogicalAnd, "&&",
                                startLine, startColumn, _column - 1);
                        }
                        else
                        {
                            Token err = CreateErrorToken($"Одиночный '&' недопустим, ожидалось '&&'",
                                startLine, startColumn);
                            return err;
                        }

                    case State.InPipe:
                        if (currentChar == '|')
                        {
                            _position++;
                            _column++;
                            return new Token(TokenType.OperatorLogicalOr, "||",
                                startLine, startColumn, _column - 1);
                        }
                        else
                        {
                            Token err = CreateErrorToken($"Одиночный '|' недопустим, ожидалось '||'",
                                startLine, startColumn);
                            return err;
                        }
                }
            }

            // Конец файла
            if (buffer.Length > 0)
            {
                switch (state)
                {
                    case State.InLetter:
                        return CreateIdentifierToken(buffer.ToString(),
                            startLine, startColumn, _column - 1);
                    case State.InDigit:
                        return new Token(TokenType.IntegerNumber, buffer.ToString(),
                            startLine, startColumn, _column - 1);
                    case State.InPlus:
                        return new Token(TokenType.OperatorPlus, "+",
                            startLine, startColumn, _column - 1);
                    case State.InMinus:
                        return new Token(TokenType.OperatorMinus, "-",
                            startLine, startColumn, _column - 1);
                    case State.InLess:
                        return new Token(TokenType.OperatorLess, "<",
                            startLine, startColumn, _column - 1);
                    case State.InGreater:
                        return new Token(TokenType.OperatorGreater, ">",
                            startLine, startColumn, _column - 1);
                    case State.InEqual:
                        return CreateErrorToken($"Незавершенный оператор '='", startLine, startColumn);
                    case State.InNot:
                        return CreateErrorToken($"Незавершенный оператор '!'", startLine, startColumn);
                    case State.InAmpersand:
                        return CreateErrorToken($"Незавершенный оператор '&'", startLine, startColumn);
                    case State.InPipe:
                        return CreateErrorToken($"Незавершенный оператор '|'", startLine, startColumn);
                }
            }

            return null;
        }

        private Token CreateIdentifierToken(string value, int line, int startCol, int endCol)
        {
            if (value == "while")
            {
                return new Token(TokenType.KeywordWhile, value, line, startCol, endCol);
            }

            return new Token(TokenType.Identifier, value, line, startCol, endCol);
        }

        private Token CreateErrorToken(string message, int line, int col)
        {
            Token error = Token.CreateError(message, line, col);

            if (_position < _input.Length)
            {
                error.Value = _input[_position].ToString();
                _position++;
                _column++;
            }

            return error;
        }

        public List<Token> Tokens => _tokens;
        public List<LexicalError> Errors => _errors;
        public bool HasErrors => _errors.Count > 0;
    }
}
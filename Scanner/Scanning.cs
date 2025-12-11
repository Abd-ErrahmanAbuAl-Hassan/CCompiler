using System.Linq;
using System.Text;

namespace Scanner
{
    public class Scanning
    {
        private readonly string _source;
        private int _index = 0;
        private int _line = 1;
        private int _column = 1;
        private readonly List<Token> _tokens = new();
        private readonly List<string> _errors = new();

        public Scanning(string source)
        {
            _source = source;
        }

        public List<Token> Scan()
        {
            while (!IsAtEnd())
            {
                char current = Peek();
                _column = _index - GetLineStart(_index) + 1;

                // Handle whitespace
                if (char.IsWhiteSpace(current))
                {
                    HandleWhitespace();
                    continue;
                }

                // Handle comments
                if (current == '/')
                {
                    if ($"{current}{PeekAhead()}" == "//")
                    {
                        _tokens.Add(HandleLineComment());
                        continue;
                    }
                    else if ($"{current}{PeekAhead()}" == "/*")
                    {
                        _tokens.Add(HandleMultiLineComment());
                        continue;
                    }
                }

                // Handle identifiers and keywords
                if (char.IsLetter(current) || current == '_')
                {
                    _tokens.Add(HandleIdentifierOrKeyword());
                    continue;
                }

                // Handle numbers
                if (char.IsDigit(current) || (current == '.' && char.IsDigit(PeekAhead())))
                {
                    _tokens.Add(HandleNumber());
                    continue;
                }

                // Handle string literals
                if (current == '"')
                {
                    _tokens.Add(HandleStringLiteral());
                    continue;
                }

                // Handle character literals
                if (current == '\'')
                {
                    _tokens.Add(HandleCharacterLiteral());
                    continue;
                }

                // Handle operators (multi-character first)
                string op = $"{current}{PeekAhead()}";
                if (CDefinitions.MultiCharOperators.Contains($"{current}{PeekAhead()}"))
                {
                    _tokens.Add(HandleOperator(op.Length));
                    continue;

                }

                // Handle single-character operators
                if (CDefinitions.SingleCharOperators.Contains(current))
                {
                    _tokens.Add(HandleOperator(1));
                    continue;
                }

                // Handle delimiters
                if (CDefinitions.Delimiters.Contains(current))
                {
                    _tokens.Add(HandleDelimiter());
                    continue;
                }

                // Unknown character
                _errors.Add($"Error at {_line}:{_column}: Unexpected character '{current}'");
                _tokens.Add(new Token(TokenType.Unknown, current.ToString(), _line, _column));
                Advance();
            }

            // Add EOF token
            _tokens.Add(new Token(TokenType.EOF, "", _line, _column));

            return _tokens;
        }

        private Token HandleLineComment()
        {
            int start = _index;
            int col = _column;

            // Already matched "//"
            Advance(2);

            while (!IsAtEnd() && Peek() != '\n')
                Advance();

            string comment = _source.Substring(start, _index - start);
            return new Token(TokenType.Comment, comment.TrimEnd('\r', '\n'), _line, col);
        }

        private Token HandleMultiLineComment()
        {
            int start = _index;
            int col = _column;
            int startLine = _line;

            // Already matched "/*"
            Advance(2);

            while (!IsAtEnd())
            {
                if (Peek() == '*' && PeekAhead() == '/')
                {
                    Advance(2);
                    break;
                }

                if (Peek() == '\n')
                {
                    _line++;
                }
                Advance();
            }

            if (IsAtEnd())
            {
                _errors.Add($"Error at {startLine}:{col}: Unterminated multi-line comment");
            }

            string comment = _source.Substring(start, _index - start);
            return new Token(TokenType.Comment, comment.TrimEnd('\r', '\n'), startLine, col);
        }

        private Token HandleIdentifierOrKeyword()
        {
            int start = _index;
            int startColumn = _column;

            while (!IsAtEnd() && (char.IsLetterOrDigit(Peek()) || Peek() == '_'))
                Advance();

            string text = _source.Substring(start, _index - start);
            var type = CDefinitions.Keywords.Contains(text) ? TokenType.Keyword : TokenType.Identifier;

            return new Token(type, text, _line, startColumn);
        }

        private Token HandleNumber()
        {
            int start = _index;
            int startColumn = _column;
            bool hasDecimal = false;
            bool hasExponent = false;

            // Integer part
            while (!IsAtEnd() && char.IsDigit(Peek()))
                Advance();

            // Decimal point
            if (!IsAtEnd() && Peek() == '.')
            {
                hasDecimal = true;
                Advance();

                // Fractional part (must have at least one digit)
                if (!IsAtEnd() && char.IsDigit(Peek()))
                {
                    while (!IsAtEnd() && char.IsDigit(Peek()))
                        Advance();
                }
                else if (!hasExponent) // Allow .e2 format
                {
                    _errors.Add($"Error at {_line}:{startColumn}: Invalid number format");
                }
            }

            // Exponent part
            if (!IsAtEnd() && (Peek() == 'e' || Peek() == 'E'))
            {
                hasExponent = true;
                Advance();

                // Optional sign
                if (!IsAtEnd() && (Peek() == '+' || Peek() == '-'))
                    Advance();

                // Must have at least one digit
                if (!IsAtEnd() && char.IsDigit(Peek()))
                {
                    while (!IsAtEnd() && char.IsDigit(Peek()))
                        Advance();
                }
                else
                {
                    _errors.Add($"Error at {_line}:{startColumn}: Invalid exponent in number");
                }
            }

            string number = _source.Substring(start, _index - start);
            return new Token(TokenType.Number, number, _line, startColumn);
        }

        private Token HandleStringLiteral()
        {
            int startColumn = _column;
            Advance(); // Skip opening quote

            StringBuilder sb = new StringBuilder();
            bool escaped = false;

            while (!IsAtEnd())
            {
                char c = Peek();

                if (escaped)
                {
                    sb.Append(ParseEscapeSequence(c));
                    escaped = false;
                }
                else if (c == '\\')
                {
                    escaped = true;
                }
                else if (c == '"')
                {
                    Advance(); // Skip closing quote
                    return new Token(TokenType.StringLiteral, sb.ToString().TrimEnd('\r', '\n'), _line, startColumn);
                }
                else if (c == '\n')
                {
                    _errors.Add($"Error at {_line}:{startColumn}: Unterminated string literal");
                    return new Token(TokenType.StringLiteral, sb.ToString().TrimEnd('\r', '\n'), _line, startColumn);
                }
                else
                {
                    sb.Append(c);
                }
                Advance();
            }

            _errors.Add($"Error at {_line}:{startColumn}: Unterminated string literal");
            return new Token(TokenType.StringLiteral, sb.ToString(), _line, startColumn);
        }

        private Token HandleCharacterLiteral()
        {
            int startColumn = _column;
            Advance(); // Skip opening quote

            char value = '\0';
            bool escaped = false;

            if (!IsAtEnd())
            {
                if (Peek() == '\\')
                {
                    escaped = true;
                    Advance();
                    if (!IsAtEnd())
                    {
                        value = ParseEscapeSequence(Peek());
                        Advance();
                    }
                }
                else
                {
                    value = Peek();
                    Advance();
                }

                if (!IsAtEnd() && Peek() == '\'')
                {
                    Advance(); // Skip closing quote
                    return new Token(TokenType.CharacterLiteral, value.ToString(), _line, startColumn);
                }
            }

            _errors.Add($"Error at {_line}:{startColumn}: Invalid character literal");
            return new Token(TokenType.CharacterLiteral, value.ToString(), _line, startColumn);
        }

        private Token HandleDelimiter()
        {
            int startColumn = _column;
            char delimiter = Peek();
            Advance();
            return new Token(TokenType.Delimiter, delimiter.ToString(), _line, startColumn);
        }

        private Token HandleOperator(int length)
        {
            int startColumn = _column;
            string op = _source.Substring(_index, length);
            Advance(length);
            return new Token(TokenType.Operator, op, _line, startColumn);
        }

        private void HandleWhitespace()
        {
            char c = Peek();
            if (c == '\n')
            {
                _line++;
            }
            Advance();
        }

        private char ParseEscapeSequence(char c)
        {
            return c switch
            {
                'n' => '\n',
                't' => '\t',
                'r' => '\r',
                '0' => '\0',
                '\\' => '\\',
                '\'' => '\'',
                '"' => '"',
                'a' => '\a',
                'b' => '\b',
                'f' => '\f',
                'v' => '\v',
                _ => c // Default to the character itself
            };
        }

        // Helper methods
        private bool IsAtEnd() => _index >= _source.Length;
        private char Peek() => _index < _source.Length ? _source[_index] : '\0';
        private char PeekAhead(int offset = 1) => _index + offset < _source.Length ? _source[_index + offset] : '\0';
        private void Advance(int count = 1)
        {
            _index += count;
        }
        private int GetLineStart(int position)
        {
            int start = 0;
            for (int i = position - 1; i >= 0; i--)
            {
                if (_source[i] == '\n')
                {
                    start = i + 1;
                    break;
                }
            }
            return start;
        }

        public List<string> GetErrors() => _errors;
    }
}
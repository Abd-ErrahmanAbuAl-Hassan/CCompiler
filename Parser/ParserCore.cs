using Scanner;

namespace Parser
{
    public class ParserCore
    {
        protected readonly List<Token> tokens;
        protected int index = 0;
        protected readonly List<string> errors = new List<string>();

        protected static readonly HashSet<string> PrimitiveTypes = new()
        {
            "int", "float", "double", "char", "short", "long",
            "signed", "unsigned", "void"
        };

        public List<string> GetErrors() => errors;

        protected void Error(string message, Token token = null)
        {
            token = token ?? Lookahead();
            errors.Add($"Syntax error at {token.Line}:{token.Column} — {message}");
        }

        public ParserCore(List<Token> _tokens)
        {
            tokens = _tokens.Where(t => t.Type != TokenType.Comment && t.Type != TokenType.Whitespace).ToList();
        }

        protected Token Lookahead()
        {
            int i = index;
            if (i >= tokens.Count)
                return new Token(TokenType.EOF, "", -1, -1);
            return tokens[i];
        }

        protected bool IsAtEnd => index >= tokens.Count || Lookahead().Type == TokenType.EOF;

        protected Token Consume()
        {
            if (IsAtEnd) return null;
            return tokens[index++];
        }

        protected void Synchronize()
        {
            while (!IsAtEnd)
            {
                if (Lookahead().Type == TokenType.Delimiter &&
                    (Lookahead().Value == ";" || Lookahead().Value == "}"))
                {
                    Consume();
                    break;
                }
                Consume();
            }
        }

        protected Token ExpectToken(TokenType type, string value = null)
        {
            if (IsAtEnd)
            {
                Error($"Expected {type}, got end of input");
                throw new ParseException("Unexpected end of input");
            }

            var token = Lookahead();
            if (token.Type != type || (value != null && token.Value != value))
            {
                Error($"Expected {type} '{value ?? "any"}', got {token.Type} '{token.Value}'", token);
                throw new ParseException("Token mismatch");
            }

            return Consume();
        }

        protected Token ExpectDelimiter(char delimiter)
        {
            if (IsAtEnd)
            {
                Error($"Expected delimiter '{delimiter}', got end of input");
                throw new ParseException("Unexpected end of input");
            }

            var token = Lookahead();

            if ((token.Type == TokenType.Delimiter && token.Value == delimiter.ToString()) ||
                (delimiter == ',' && token.Type == TokenType.Operator && token.Value == ","))
            {
                return Consume();
            }

            Error($"Expected delimiter '{delimiter}', got {token.Type} '{token.Value}'", token);
            throw new ParseException("Delimiter mismatch");
        }

        protected Token ExpectKeyword(string keyword) => ExpectToken(TokenType.Keyword, keyword);
        protected Token ExpectIdentifier() => ExpectToken(TokenType.Identifier);

        protected bool CheckKeyword(string keyword) => !IsAtEnd && Lookahead().Type == TokenType.Keyword && Lookahead().Value == keyword;
        protected bool CheckDelimiter(char delimiter) => !IsAtEnd && Lookahead().Type == TokenType.Delimiter && Lookahead().Value == delimiter.ToString();
        protected bool CheckOperator(string op) => !IsAtEnd && Lookahead().Type == TokenType.Operator && Lookahead().Value == op;

        protected bool IsAssignmentOperator()
        {
            if (Lookahead().Type != TokenType.Operator) return false;
            var op = Lookahead().Value;
            return op == "=" || op.EndsWith("=");
        }

        protected bool IsPrefixOperator()
        {
            if (Lookahead().Type != TokenType.Operator) return false;
            var op = Lookahead().Value;
            return op == "+" || op == "-" || op == "!" || op == "~" ||
                   op == "++" || op == "--" || op == "&" || op == "*";
        }

        protected bool IsDeclarationStart()
        {
            if (IsAtEnd) return false;
            if (Lookahead().Type == TokenType.Keyword)
            {
                var value = Lookahead().Value;
                return PrimitiveTypes.Contains(value) ||
                       value == "struct" || value == "union" || value == "enum";
            }
            return false;
        }

        protected bool IsMainAhead()
        {
            int savedIndex = index;
            try
            {
                if (!IsDeclarationStart()) return false;
                var typeToken = tokens[savedIndex];
                if (savedIndex + 1 >= tokens.Count) return false;
                var secondToken = tokens[savedIndex + 1];
                if (secondToken.Type != TokenType.Keyword || secondToken.Value != "main") return false;
                if (savedIndex + 2 >= tokens.Count) return false;
                var thirdToken = tokens[savedIndex + 2];
                if (thirdToken.Type != TokenType.Delimiter || thirdToken.Value != "(") return false;
                return true;
            }
            finally
            {
                index = savedIndex;
            }
        }

        protected string ParseTypeSpecifier()
        {
            if (Lookahead().Type == TokenType.Keyword && PrimitiveTypes.Contains(Lookahead().Value))
            {
                return Consume().Value;
            }
            Error("Expected type specifier");
            return "int";
        }

        protected int ParseConstantExpression()
        {
            if (Lookahead().Type == TokenType.Number)
            {
                var token = Consume();
                if (int.TryParse(token.Value, out int result))
                    return result;
            }
            Error("Expected constant expression");
            return 0;
        }
    }
}
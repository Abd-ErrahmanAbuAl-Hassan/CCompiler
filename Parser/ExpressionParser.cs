using Parser.Nodes;
using Scanner;

namespace Parser
{
    public class ExpressionParser : ParserCore
    {
        public ExpressionParser(List<Token> tokens) : base(tokens) { }

        public ExpressionNode ParseExpression() => ParseAssignment();

        public ExpressionNode ParseCondition()
        {
            return ParseLogicalOr();  
        }

        private ExpressionNode ParseAssignment()
        {
            var left = ParseLogicalOr();  

            if (IsAssignmentOperator())
            {
                var opToken = Consume();
                var right = ParseAssignment();

                return new AssignExpressionNode
                {
                    Left = left,
                    Right = right,
                    Op = opToken.Value,
                    Line = opToken.Line,
                    Column = opToken.Column
                };
            }

            return left;
        }

        private ExpressionNode ParseLogicalOr() =>
            ParseLeftAssociativeBinary(ParseLogicalAnd, "||");

        private ExpressionNode ParseLogicalAnd() =>
            ParseLeftAssociativeBinary(ParseBitwiseOr, "&&");

        private ExpressionNode ParseBitwiseOr() =>
            ParseLeftAssociativeBinary(ParseBitwiseXor, "|");

        private ExpressionNode ParseBitwiseXor() =>
            ParseLeftAssociativeBinary(ParseBitwiseAnd, "^");

        private ExpressionNode ParseBitwiseAnd() =>
            ParseLeftAssociativeBinary(ParseEquality, "&");

        private ExpressionNode ParseEquality() =>
            ParseLeftAssociativeBinary(ParseRelational, "==", "!=");

        private ExpressionNode ParseRelational() =>
            ParseLeftAssociativeBinary(ParseShift, "<", ">", "<=", ">=");

        private ExpressionNode ParseShift() =>
            ParseLeftAssociativeBinary(ParseAdditive, "<<", ">>");

        private ExpressionNode ParseAdditive() =>
            ParseLeftAssociativeBinary(ParseMultiplicative, "+", "-");

        private ExpressionNode ParseMultiplicative() =>
            ParseLeftAssociativeBinary(ParseUnary, "*", "/", "%");

        private ExpressionNode ParseUnary()
        {
            if (IsPrefixOperator())
            {
                var opToken = Consume();
                var operand = ParseUnary();

                return new UnaryExpressionNode
                {
                    Op = opToken.Value,
                    Operand = operand,
                    IsPrefix = true,
                    Line = opToken.Line,
                    Column = opToken.Column
                };
            }

            return ParsePostfix();
        }

        private ExpressionNode ParsePostfix()
        {
            var expr = ParsePrimary();

            while (true)
            {
                if (CheckDelimiter('['))
                {
                    expr = ParseArrayIndex(expr);
                }
                else if (CheckDelimiter('('))
                {
                    expr = ParseFunctionCall(expr);
                }
                else if (CheckOperator("++") || CheckOperator("--"))
                {
                    expr = ParsePostfixUnary(expr);
                }
                else
                {
                    break;
                }
            }

            return expr;
        }

        private ExpressionNode ParsePrimary()
        {
            if (IsAtEnd)
            {
                Error("Unexpected end of input");
                return null;
            }

            var token = Lookahead();

            switch (token.Type)
            {
                case TokenType.Number:
                    return ParseNumberLiteral();
                case TokenType.StringLiteral:
                    return ParseStringLiteral();
                case TokenType.CharacterLiteral:
                    return ParseCharacterLiteral();
                case TokenType.Identifier:
                    return ParseIdentifier();
                default:
                    if (CheckDelimiter('('))
                        return ParseParenthesizedExpression();

                    Error($"Expected primary expression, got {token.Type} '{token.Value}'", token);
                    return null;
            }
        }

        private ExpressionNode ParseNumberLiteral()
        {
            var token = Consume();
            return new NumberExpressionNode
            {
                Value = token.Value,
                Line = token.Line,
                Column = token.Column
            };
        }

        private ExpressionNode ParseStringLiteral()
        {
            var token = Consume();
            return new StringExpressionNode
            {
                Value = token.Value,
                Line = token.Line,
                Column = token.Column
            };
        }

        private ExpressionNode ParseCharacterLiteral()
        {
            var token = Consume();
            char value = token.Value.Length > 0 ? token.Value[0] : '\0';
            return new CharacterExpressionNode
            {
                Value = value,
                Line = token.Line,
                Column = token.Column
            };
        }

        private ExpressionNode ParseIdentifier()
        {
            var token = Consume();
            return new IdentifierExpressionNode
            {
                Name = token.Value,
                Line = token.Line,
                Column = token.Column
            };
        }

        private ExpressionNode ParseParenthesizedExpression()
        {
            Consume(); // '('
            var expr = ParseExpression();
            ExpectDelimiter(')');
            return expr;
        }

        private ExpressionNode ParseArrayIndex(ExpressionNode target)
        {
            Consume(); // '['
            var index = ParseExpression();
            ExpectDelimiter(']');

            return new IndexExpressionNode
            {
                Target = target,
                Index = index
            };
        }

        private ExpressionNode ParseFunctionCall(ExpressionNode callee)
        {
            Consume(); // '('
            var call = new CallExpressionNode { Callee = callee };

            if (!CheckDelimiter(')'))
            {
                var args = ParseArgumentList();
                foreach (var arg in args)
                    call.Args.Add(arg);
            }

            ExpectDelimiter(')');
            return call;
        }

        private ExpressionNode ParsePostfixUnary(ExpressionNode operand)
        {
            var opToken = Consume();
            return new UnaryExpressionNode
            {
                Op = opToken.Value,
                Operand = operand,
                IsPrefix = false,
                Line = opToken.Line,
                Column = opToken.Column
            };
        }
        private ExpressionNode ParseLeftAssociativeBinary(Func<ExpressionNode> parseNext, params string[] operators)
        {
            var left = parseNext();

            while (!IsAtEnd && Lookahead().Type == TokenType.Operator && operators.Contains(Lookahead().Value))
            {
                var opToken = Consume();
                var right = parseNext();

                left = new BinaryExpressionNode
                {
                    Left = left,
                    Right = right,
                    Op = opToken.Value,
                    Line = opToken.Line,
                    Column = opToken.Column
                };
            }

            return left;
        }

        public List<ExpressionNode> ParseArgumentList()
        {
            var args = new List<ExpressionNode>();

            do
            {
                args.Add(ParseExpression());

                if (CheckOperator(","))
                {
                    Consume();
                    continue;
                }

                break;
            } while (true);

            return args;
        }
    }
}
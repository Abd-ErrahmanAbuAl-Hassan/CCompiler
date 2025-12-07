using Parser.Nodes;
using Scanner;

namespace Parser
{
    public class ExpressionParser : ParserCore
    {
        public ExpressionParser(List<Token> tokens) : base(tokens) { }

        public ExpressionNode ParseExpression()
        {
            return ParseAssignment();
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

        private ExpressionNode ParseLogicalOr()
        {
            return ParseLeftAssociativeBinary(ParseLogicalAnd, "||");
        }

        private ExpressionNode ParseLogicalAnd()
        {
            return ParseLeftAssociativeBinary(ParseBitwiseOr, "&&");
        }

        private ExpressionNode ParseBitwiseOr()
        {
            return ParseLeftAssociativeBinary(ParseBitwiseXor, "|");
        }

        private ExpressionNode ParseBitwiseXor()
        {
            return ParseLeftAssociativeBinary(ParseBitwiseAnd, "^");
        }

        private ExpressionNode ParseBitwiseAnd()
        {
            return ParseLeftAssociativeBinary(ParseEquality, "&");
        }

        private ExpressionNode ParseEquality()
        {
            return ParseLeftAssociativeBinary(ParseRelational, "==", "!=");
        }

        private ExpressionNode ParseRelational()
        {
            return ParseLeftAssociativeBinary(ParseShift, "<", ">", "<=", ">=");
        }

        private ExpressionNode ParseShift()
        {
            return ParseLeftAssociativeBinary(ParseAdditive, "<<", ">>");
        }

        private ExpressionNode ParseAdditive()
        {
            return ParseLeftAssociativeBinary(ParseMultiplicative, "+", "-");
        }

        private ExpressionNode ParseMultiplicative()
        {
            return ParseLeftAssociativeBinary(ParseUnary, "*", "/", "%");
        }

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
                    Consume();
                    var index = ParseExpression();
                    ExpectDelimiter(']');

                    expr = new IndexExpressionNode
                    {
                        Target = expr,
                        Index = index
                    };
                }
                else if (CheckDelimiter('('))
                {
                    Consume();
                    var call = new CallExpressionNode { Callee = expr };

                    if (!CheckDelimiter(')'))
                    {
                        var args = ParseArgumentList();
                        foreach (var arg in args)
                            call.Args.Add(arg);
                    }

                    ExpectDelimiter(')');
                    expr = call;
                }
                else if (CheckOperator("++") || CheckOperator("--"))
                {
                    var opToken = Consume();

                    expr = new UnaryExpressionNode
                    {
                        Op = opToken.Value,
                        Operand = expr,
                        IsPrefix = false,
                        Line = opToken.Line,
                        Column = opToken.Column
                    };
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

            if (token.Type == TokenType.Number)
            {
                Consume();
                return new NumberExpressionNode
                {
                    Value = token.Value,
                    Line = token.Line,
                    Column = token.Column
                };
            }

            if (token.Type == TokenType.StringLiteral)
            {
                Consume();
                return new StringExpressionNode
                {
                    Value = token.Value,
                    Line = token.Line,
                    Column = token.Column
                };
            }

            if (token.Type == TokenType.CharacterLiteral)
            {
                Consume();
                return new CharacterExpressionNode
                {
                    Value = token.Value.Length > 0 ? token.Value[0] : '\0',
                    Line = token.Line,
                    Column = token.Column
                };
            }

            if (token.Type == TokenType.Identifier)
            {
                Consume();
                return new IdentifierExpressionNode
                {
                    Name = token.Value,
                    Line = token.Line,
                    Column = token.Column
                };
            }

            if (CheckDelimiter('('))
            {
                Consume();
                var expr = ParseExpression();
                ExpectDelimiter(')');
                return expr;
            }

            Error($"Expected primary expression, got {token.Type} '{token.Value}'", token);
            return null;
        }

        private ExpressionNode ParseLeftAssociativeBinary(Func<ExpressionNode> parseNext, params string[] operators)
        {
            var left = parseNext();

            while (!IsAtEnd && operators.Contains(Lookahead().Value))
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

                if (CheckDelimiter(','))
                {
                    Consume();
                    continue;
                }

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
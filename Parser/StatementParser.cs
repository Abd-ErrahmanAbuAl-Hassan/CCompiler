using Parser.Nodes;
using Scanner;

namespace Parser
{
    public class StatementParser : ExpressionParser
    {
        public StatementParser(List<Token> tokens) : base(tokens) { }

        public virtual StatementNode ParseStatement()
        {
            if (IsAtEnd) return null;

            try
            {
                if (CheckDelimiter(';'))
                {
                    Consume();
                    return new ExprStmtNode { Expression = null };
                }

                if (CheckDelimiter('{'))
                {
                    return ParseBlock();
                }

                if (Lookahead().Type == TokenType.Keyword)
                {
                    var keyword = Lookahead().Value;
                    if (keyword == "if" || keyword == "while" || keyword == "do" ||
                        keyword == "for" || keyword == "return" || keyword == "break" ||
                        keyword == "continue")
                    {
                        return ParseControlFlowStatement();
                    }
                }

                return ParseExpressionStatement();
            }
            catch (ParseException)
            {
                Synchronize();
                return null;
            }
        }
        private StatementNode ParseControlFlowStatement()
        {
            var keyword = Lookahead().Value;

            switch (keyword)
            {
                case "if":
                    return ParseIfStatement();
                case "while":
                    return ParseWhileStatement();
                case "do":
                    return ParseDoWhileStatement();
                case "for":
                    return ParseForStatement();
                case "return":
                    return ParseReturnStatement();
                case "break":
                    return ParseBreakStatement();
                case "continue":
                    return ParseContinueStatement();
                default:
                    return ParseExpressionStatement();
            }
        }
        private StatementNode ParseExpressionStatement()
        {
            var expr = ParseExpression();  
            ExpectDelimiter(';');
            return new ExprStmtNode { Expression = expr };
        }
        public BlockStmtNode ParseBlock()
        {
            ExpectDelimiter('{');

            var block = new BlockStmtNode();
            while (!CheckDelimiter('}') && !IsAtEnd)
            {
                var stmt = ParseStatement();
                if (stmt != null)
                {
                    block.Statements.Add(stmt);
                }
                
            }

            ExpectDelimiter('}');
            return block;
        }
        private IfStmtNode ParseIfStatement()
        {
            ExpectKeyword("if");
            ExpectDelimiter('(');
            var condition = ParseCondition();  
            ExpectDelimiter(')');

            var thenBranch = ParseStatement();
            StatementNode elseBranch = null;

            if (CheckKeyword("else"))
            {
                Consume();
                elseBranch = ParseStatement();
            }

            return new IfStmtNode
            {
                Condition = condition,
                ThenBranch = thenBranch,
                ElseBranch = elseBranch
            };
        }
        private WhileStmtNode ParseWhileStatement()
        {
            ExpectKeyword("while");
            ExpectDelimiter('(');
            var condition = ParseCondition();  
            ExpectDelimiter(')');

            ExpectDelimiter('{');
            var body = ParseStatement();
            ExpectDelimiter('}');


            return new WhileStmtNode
            {
                Condition = condition,
                Body = body
            };
        }
        private DoWhileStmtNode ParseDoWhileStatement()
        {
            ExpectKeyword("do");
            ExpectDelimiter('{');
            var body = ParseStatement();
            ExpectDelimiter('}');

            ExpectKeyword("while");
            ExpectDelimiter('(');
            var condition = ParseCondition();  
            ExpectDelimiter(')');
            ExpectDelimiter(';');

            return new DoWhileStmtNode
            {
                Body = body,
                Condition = condition
            };
        }
        private ForStmtNode ParseForStatement()
        {
            ExpectKeyword("for");
            ExpectDelimiter('(');

            StatementNode init = null;
            if (!CheckDelimiter(';'))
            {
                if (PrimitiveTypes.Contains(Lookahead().Value)) Consume();
                var expr = ParseExpression();  
                ExpectDelimiter(';');
                init = new ExprStmtNode { Expression = expr };
            }
            else
            {
                Consume(); // ';'
                init = new ExprStmtNode { Expression = null };
            }

            // Parse condition
            ExpressionNode condition = null;
            if (!CheckDelimiter(';'))
            {
                condition = ParseCondition();  
            }
            ExpectDelimiter(';');

            // Parse increment
            ExpressionNode increment = null;
            if (!CheckDelimiter(')'))
            {
                increment = ParseExpression();  
            }
            ExpectDelimiter(')');

            ExpectDelimiter('{');
            var body = ParseStatement();
            ExpectDelimiter('}');


            return new ForStmtNode
            {
                Init = init,
                Condition = condition,
                Increment = increment,
                Body = body
            };
        }
        private ReturnStmtNode ParseReturnStatement()
        {
            var returnToken = ExpectKeyword("return");
            var node = new ReturnStmtNode
            {
                Line = returnToken.Line,
                Column = returnToken.Column
            };

            if (!CheckDelimiter(';'))
                node.Expression = ParseExpression();  

            ExpectDelimiter(';');
            return node;
        }
        private BreakStmtNode ParseBreakStatement()
        {
            var breakToken = ExpectKeyword("break");
            ExpectDelimiter(';');
            return new BreakStmtNode
            {
                Line = breakToken.Line,
                Column = breakToken.Column
            };
        }
        private ContinueStmtNode ParseContinueStatement()
        {
            var continueToken = ExpectKeyword("continue");
            ExpectDelimiter(';');
            return new ContinueStmtNode
            {
                Line = continueToken.Line,
                Column = continueToken.Column
            };
        }
    }
}
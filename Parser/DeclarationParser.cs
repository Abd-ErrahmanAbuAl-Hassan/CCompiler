using Parser.Nodes;
using Scanner;

namespace Parser
{
    public class DeclarationParser : ExpressionParser
    {
        public DeclarationParser(List<Token> tokens) : base(tokens) { }

        public ProgramNode Parse()
        {
            var node = new ProgramNode();

            try
            {
                while (!IsAtEnd && IsDeclarationStart())
                {
                    if (IsMainAhead())
                    {
                        node.MainFunction = ParseMainDeclaration();
                        break;
                    }
                    else
                    {
                        var decl = ParseTopLevelDeclaration();
                        if (decl != null)
                            node.PreMainDecls.Add(decl);
                    }
                }

                if (node.MainFunction == null)
                {
                    Error("Program must contain a main function");
                    return node;
                }

                while (!IsAtEnd && IsDeclarationStart())
                {
                    var decl = ParseTopLevelDeclaration();
                    if (decl != null)
                        node.PostMainDecls.Add(decl);
                }

                if (!IsAtEnd && Lookahead().Type != TokenType.EOF)
                {
                    Error("Unexpected tokens after program");
                }
            }
            catch (ParseException)
            {
                // Errors already logged
            }

            return node;
        }

        private DeclarationNode ParseTopLevelDeclaration()
        {
            try
            {
                string typeName = ParseTypeSpecifier();

                if (Lookahead().Type == TokenType.Identifier || Lookahead().Value == "main")
                {
                    var nameToken = Consume();

                    if (Lookahead().Type == TokenType.Delimiter && Lookahead().Value == "(")
                    {
                        return ParseFunctionDeclaration(typeName, nameToken.Value);
                    }
                    else
                    {
                        return ParseVariableDeclaration(typeName, nameToken.Value, true);
                    }
                }
                else
                {
                    Error($"Expected identifier after type '{typeName}'");
                    return null;
                }
            }
            catch (ParseException)
            {
                Synchronize();
                return null;
            }
        }

        private FuncDeclNode ParseMainDeclaration()
        {
            var returnType = ParseTypeSpecifier();
            ExpectKeyword("main");

            var func = new FuncDeclNode
            {
                ReturnType = returnType,
                Name = "main",
                Line = Lookahead().Line,
                Column = Lookahead().Column
            };

            ExpectDelimiter('(');
            ExpectDelimiter(')');
            func.Body = ParseBlock();

            return func;
        }

        private FuncDeclNode ParseFunctionDeclaration(string returnType, string name)
        {
            var func = new FuncDeclNode
            {
                ReturnType = returnType,
                Name = name,
                Line = Lookahead().Line,
                Column = Lookahead().Column
            };

            ExpectDelimiter('(');

            if (!CheckDelimiter(')'))
            {
                var parameters = ParseParameterList();
                foreach (var param in parameters)
                    func.Parameters.Add(param);
            }

            ExpectDelimiter(')');

            if (CheckDelimiter(';'))
            {
                Consume();
            }
            else if (CheckDelimiter('{'))
            {
                func.Body = ParseBlock();
            }
            else
            {
                Error("Expected ';' or '{' after function declaration");
            }

            return func;
        }

        private List<ParamNode> ParseParameterList()
        {
            var parameters = new List<ParamNode>();

            do
            {
                var paramType = ParseTypeSpecifier();
                string paramName = "";

                if (Lookahead().Type == TokenType.Identifier)
                    paramName = ExpectIdentifier().Value;

                parameters.Add(new ParamNode
                {
                    TypeName = paramType,
                    Name = paramName
                });

                if (CheckOperator(","))
                {
                    Consume();
                    continue;
                }

                break;
            } while (true);

            return parameters;
        }

        private VarDeclNode ParseVariableDeclaration(string typeName, string name, bool isGlobal = false)
        {
            var varDecl = new VarDeclNode
            {
                TypeName = typeName,
                Name = name,
                Line = Lookahead().Line,
                Column = Lookahead().Column
            };

            if (CheckDelimiter('['))
            {
                Consume();
                varDecl.ArraySize = ParseConstantExpression();
                ExpectDelimiter(']');
            }

            if (CheckOperator("="))
            {
                Consume();
                varDecl.Initializer = ParseExpression();
            }

            ExpectDelimiter(';');

            return varDecl;
        }

        public StatementNode ParseStatement()
        {
            if (IsAtEnd) return null;

            try
            {
                if (IsDeclarationStart())
                {
                    var typeName = ParseTypeSpecifier();
                    if (Lookahead().Type == TokenType.Identifier)
                    {
                        var name = Consume().Value;
                        return new VarDeclStmtNode
                        {
                            Declaration = ParseVariableDeclaration(typeName, name, false)
                        };
                    }
                }

                if (CheckKeyword("if")) return ParseIfStatement();
                if (CheckKeyword("while")) return ParseWhileStatement();
                if (CheckKeyword("do")) return ParseDoWhileStatement();
                if (CheckKeyword("for")) return ParseForStatement();
                if (CheckKeyword("break")) return ParseBreakStatement();
                if (CheckKeyword("continue")) return ParseContinueStatement();
                if (CheckKeyword("return")) return ParseReturnStatement();

                if (CheckDelimiter('{'))
                    return ParseBlock();

                if (CheckDelimiter(';'))
                {
                    Consume();
                    return new ExprStmtNode();
                }

                var expr = ParseExpression();
                ExpectDelimiter(';');
                return new ExprStmtNode { Expression = expr };
            }
            catch (ParseException)
            {
                Synchronize();
                return null;
            }
        }

        private BlockStmtNode ParseBlock()
        {
            ExpectDelimiter('{');

            var block = new BlockStmtNode();
            while (!CheckDelimiter('}') && !IsAtEnd)
            {
                var stmt = ParseStatement();
                if (stmt != null)
                    block.Statements.Add(stmt);
            }

            ExpectDelimiter('}');
            return block;
        }

        private IfStmtNode ParseIfStatement()
        {
            ExpectKeyword("if");
            ExpectDelimiter('(');
            var condition = ParseExpression();
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
            var condition = ParseExpression();
            ExpectDelimiter(')');
            ExpectDelimiter('{');
            var body = ParseStatement();
            ExpectDelimiter('{');

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
            var condition = ParseExpression();
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
                if (IsDeclarationStart())
                {
                    var typeName = ParseTypeSpecifier();
                    if (Lookahead().Type == TokenType.Identifier)
                    {
                        var name = Consume().Value;
                        init = new VarDeclStmtNode
                        {
                            Declaration = ParseVariableDeclaration(typeName, name, false)
                        };
                    }
                }
                else
                {
                    var expr = ParseExpression();
                    ExpectDelimiter(';');
                    init = new ExprStmtNode { Expression = expr };
                }
            }
            else
            {
                Consume();
                init = new ExprStmtNode();
            }

            ExpressionNode condition = null;
            if (!CheckDelimiter(';'))
                condition = ParseExpression();
            ExpectDelimiter(';');

            ExpressionNode increment = null;
            if (!CheckDelimiter(')'))
                increment = ParseExpression();
            ExpectDelimiter(')');

            var body = ParseStatement();

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
            ExpectKeyword("return");
            var node = new ReturnStmtNode();
            if (!CheckDelimiter(';'))
                node.Expression = ParseExpression();
            ExpectDelimiter(';');
            return node;
        }

        private BreakStmtNode ParseBreakStatement()
        {
            ExpectKeyword("break");
            ExpectDelimiter(';');
            return new BreakStmtNode();
        }

        private ContinueStmtNode ParseContinueStatement()
        {
            ExpectKeyword("continue");
            ExpectDelimiter(';');
            return new ContinueStmtNode();
        }
    }
}
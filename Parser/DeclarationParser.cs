using Parser.Nodes;
using Scanner;

namespace Parser
{
    public class DeclarationParser : StatementParser
    {
        public DeclarationParser(List<Token> tokens) : base(tokens) { }

        public List<DeclarationNode> ParseTopLevelDeclaration()
        {
            try
            {
                string typeName = ParseTypeSpecifier();

                if (IsFunctionDeclarationAhead())
                {
                    return new List<DeclarationNode> { ParseFunctionDeclaration(typeName) };
                }

                return ParseVariableDeclarationList(typeName, true);
            }
            catch (ParseException)
            {
                Synchronize();
                return null;
            }
        }

        public override StatementNode ParseStatement()
        {
            if (IsAtEnd) return null;

            try
            {
                if (IsDeclarationStart())
                {
                    return ParseDeclarationStatement();
                }

                return base.ParseStatement();
            }
            catch (ParseException)
            {
                Synchronize();
                return null;
            }
        }

        private StatementNode ParseDeclarationStatement()
        {
            string typeName = ParseTypeSpecifier();

            if (IsFunctionDeclarationAhead())
            {
                var funcDecl = ParseFunctionDeclaration(typeName);
                ExpectDelimiter(';'); 
                return new ExprStmtNode { Expression = null }; 
            }

            var varDecls = ParseVariableDeclarationList(typeName, false);

            if (varDecls.Count == 1)
            {
                return new VarDeclStmtNode { Declaration = (VarDeclNode)varDecls[0] };
            }

            return CreateDeclarationBlock(varDecls);
        }

        public List<DeclarationNode> ParseVariableDeclarationList(string typeName, bool isGlobal = false)
        {
            var declarations = new List<DeclarationNode>();

            do
            {
                var varDecl = ParseSingleDeclarator(typeName);
                declarations.Add(varDecl);

                if (!CheckOperator(",")) break;
                Consume(); // Consume ','
            } while (true);

            ExpectDelimiter(';');
            return declarations;
        }

        private VarDeclNode ParseSingleDeclarator(string typeName)
        {
            Token nameToken = ExpectIdentifier();

            var varDecl = new VarDeclNode
            {
                TypeName = typeName,
                Name = nameToken.Value,
                Line = nameToken.Line,
                Column = nameToken.Column
            };

            if (CheckDelimiter('['))
            {
                Consume(); // '['
                varDecl.ArraySize = ParseConstantExpression();
                ExpectDelimiter(']');
            }

            if (CheckOperator("="))
            {
                Consume(); // '='
                varDecl.Initializer = ParseExpression();
            }

            return varDecl;
        }

        private BlockStmtNode CreateDeclarationBlock(List<DeclarationNode> declarations)
        {
            var block = new BlockStmtNode();
            foreach (VarDeclNode varDecl in declarations)
            {
                block.Statements.Add(new VarDeclStmtNode { Declaration = varDecl });
            }
            return block;
        }

        private bool IsFunctionDeclarationAhead()
        {
            int savedIndex = index;
            try
            {
                var typeToken = Lookahead();
                if (!PrimitiveTypes.Contains(typeToken.Value)) return false;

                index++;

                if (Lookahead().Type != TokenType.Identifier) return false;
                index++; // Skip identifier

                return CheckDelimiter('(');
            }
            finally
            {
                index = savedIndex;
            }
        }

        private FuncDeclNode ParseFunctionDeclaration(string returnType)
        {
            var nameToken = ExpectIdentifier();

            var func = new FuncDeclNode
            {
                ReturnType = returnType,
                Name = nameToken.Value,
                Line = nameToken.Line,
                Column = nameToken.Column
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
                throw new ParseException("Invalid function declaration");
            }

            return func;
        }

        public FuncDeclNode ParseMainDeclaration()
        {
            var returnType = ParseTypeSpecifier();
            var mainToken = ExpectKeyword("main");

            var func = new FuncDeclNode
            {
                ReturnType = returnType,
                Name = "main",
                Line = mainToken.Line,
                Column = mainToken.Column
            };

            ExpectDelimiter('(');
            ExpectDelimiter(')');
            func.Body = ParseBlock();

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
    }
}
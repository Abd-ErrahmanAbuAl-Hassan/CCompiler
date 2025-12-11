using Parser.Nodes;
using Scanner;

namespace Parser
{
    public class Parsing : DeclarationParser
    {
        public Parsing(List<Token> tokens) : base(tokens) { }

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

                    var decls = ParseTopLevelDeclaration();
                    if (decls != null)
                        node.PreMainDecls.AddRange(decls);
                }

                if (node.MainFunction == null)
                {
                    Error("Program must contain a main function");
                }

                while (!IsAtEnd && IsDeclarationStart())
                {
                    var decls = ParseTopLevelDeclaration();
                    if (decls != null)
                        node.PostMainDecls.AddRange(decls);
                }

                if (!IsAtEnd && Lookahead().Type != TokenType.EOF)
                {
                    Error("Unexpected tokens after program");
                }
            }
            catch (ParseException)
            {
                
            }

            return node;
        }
    }
}
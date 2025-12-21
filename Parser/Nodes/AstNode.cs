using System.Text;

namespace Parser.Nodes
{
    public abstract class AstNode
    {
        public int Line { get; set; }
        public int Column { get; set; }
    }

    public class ProgramNode : AstNode
    {
        public List<DeclarationNode> PreMainDecls { get; } = new();
        public FuncDeclNode MainFunction { get; set; }
        public List<DeclarationNode> PostMainDecls { get; } = new();

    }

}
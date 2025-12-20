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

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Pre-main declarations:");
            foreach (var decl in PreMainDecls)
                sb.AppendLine($"  {decl}");

            sb.AppendLine("\nMain function:");
            sb.AppendLine($"  {MainFunction}");

            sb.AppendLine("\nPost-main declarations:");
            foreach (var decl in PostMainDecls)
                sb.AppendLine($"  {decl}");

            return sb.ToString();
        }
    }

}
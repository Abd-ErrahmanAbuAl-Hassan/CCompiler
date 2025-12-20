namespace Parser.Nodes
{
    public abstract class DeclarationNode : AstNode { }

    public class VarDeclNode : DeclarationNode
    {
        public string TypeName { get; set; }
        public string Name { get; set; }
        public ExpressionNode Initializer { get; set; }
        public int? ArraySize { get; set; }
    }

    public class FuncDeclNode : DeclarationNode
    {
        public string ReturnType { get; set; }
        public string Name { get; set; }
        public List<ParamNode> Parameters { get; } = new();
        public BlockStmtNode Body { get; set; }
    }

    public class ParamNode : AstNode
    {
        public string TypeName { get; set; }
        public string Name { get; set; }
    }
}

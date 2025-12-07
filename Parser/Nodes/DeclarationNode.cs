namespace Parser.Nodes
{
    public abstract class DeclarationNode : AstNode { }

    public class VarDeclNode : DeclarationNode
    {
        public string TypeName { get; set; }
        public string Name { get; set; }
        public ExpressionNode Initializer { get; set; }
        public int? ArraySize { get; set; }
        public bool IsArray => ArraySize.HasValue;

        public override string ToString()
        {
            if (IsArray)
                return $"VarDecl[{TypeName} {Name}[{ArraySize}] = {Initializer}]";
            return $"VarDecl[{TypeName} {Name} = {Initializer}]";
        }
    }

    public class FuncDeclNode : DeclarationNode
    {
        public string ReturnType { get; set; }
        public string Name { get; set; }
        public List<ParamNode> Parameters { get; } = new();
        public BlockStmtNode Body { get; set; }
        public bool IsPrototype => Body == null;

        public override string ToString()
        {
            var paramsStr = string.Join(", ", Parameters);
            if (IsPrototype)
                return $"FuncProto[{ReturnType} {Name}({paramsStr})]";
            return $"FuncDecl[{ReturnType} {Name}({paramsStr}) {{ ... }}]";
        }
    }

    public class ParamNode : AstNode
    {
        public string TypeName { get; set; }
        public string Name { get; set; }

        public override string ToString() => $"{TypeName} {Name}";
    }
}

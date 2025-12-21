namespace Parser.Nodes
{
    public abstract class ExpressionNode : AstNode { } 

    public class NumberExpressionNode : ExpressionNode
    {
        public string Value { get; set; }
    }

    public class StringExpressionNode : ExpressionNode
    {
        public string Value { get; set; }
    }

    public class CharacterExpressionNode : ExpressionNode
    {
        public char Value { get; set; }
    }

    public class IdentifierExpressionNode : ExpressionNode
    {
        public string Name { get; set; }
    }

    public class BinaryExpressionNode : ExpressionNode
    {
        public string Op { get; set; }
        public ExpressionNode Left { get; set; }
        public ExpressionNode Right { get; set; }

    }

    public class UnaryExpressionNode : ExpressionNode
    {
        public string Op { get; set; }
        public ExpressionNode Operand { get; set; }
        public bool IsPrefix { get; set; } = true;

    }

    public class AssignExpressionNode : ExpressionNode
    {
        public ExpressionNode Left { get; set; }
        public ExpressionNode Right { get; set; }
        public string Op { get; set; } = "="; // For compound assignments

    }

    public class CallExpressionNode : ExpressionNode
    {
        public ExpressionNode Callee { get; set; }
        public List<ExpressionNode> Args { get; } = new();

    }

    public class IndexExpressionNode : ExpressionNode
    {
        public ExpressionNode Target { get; set; }
        public ExpressionNode Index { get; set; }

    }
   
}

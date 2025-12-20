namespace Parser.Nodes
{
    public abstract class StatementNode : AstNode { }

    public class BlockStmtNode : StatementNode
    {
        public List<StatementNode> Statements { get; } = new();

    }

    public class ExprStmtNode : StatementNode
    {
        public ExpressionNode Expression { get; set; }

    }

    public class VarDeclStmtNode : StatementNode
    {
        public VarDeclNode Declaration { get; set; }

    }

    public class ReturnStmtNode : StatementNode
    {
        public ExpressionNode Expression { get; set; }
    }

    public class IfStmtNode : StatementNode
    {
        public ExpressionNode Condition { get; set; }
        public StatementNode ThenBranch { get; set; }
        public StatementNode ElseBranch { get; set; }

    }

    public class WhileStmtNode : StatementNode
    {
        public ExpressionNode Condition { get; set; }
        public StatementNode Body { get; set; }

    }

    public class DoWhileStmtNode : StatementNode
    {
        public StatementNode Body { get; set; }
        public ExpressionNode Condition { get; set; }

    }

    public class ForStmtNode : StatementNode
    {
        public StatementNode Init { get; set; }
        public ExpressionNode Condition { get; set; }
        public ExpressionNode Increment { get; set; }
        public StatementNode Body { get; set; }

    }

    public class BreakStmtNode : StatementNode
    {
    }

    public class ContinueStmtNode : StatementNode
    {
    }

}

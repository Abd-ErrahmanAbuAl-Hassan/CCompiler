using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.Nodes
{
    public abstract class StatementNode : AstNode { }

    public class BlockStmtNode : StatementNode
    {
        public List<StatementNode> Statements { get; } = new();

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Block {");
            foreach (var stmt in Statements)
                sb.AppendLine($"  {stmt}");
            sb.Append("}");
            return sb.ToString();
        }
    }

    public class ExprStmtNode : StatementNode
    {
        public ExpressionNode Expression { get; set; }

        public override string ToString()
        {
            if (Expression == null) return "EmptyStmt[;]";
            return $"ExprStmt[{Expression}]";
        }
    }

    public class VarDeclStmtNode : StatementNode
    {
        public VarDeclNode Declaration { get; set; }

        public override string ToString() => $"VarDeclStmt[{Declaration}]";
    }

    public class ReturnStmtNode : StatementNode
    {
        public ExpressionNode Expression { get; set; }

        public override string ToString()
        {
            if (Expression == null) return "Return[]";
            return $"Return[{Expression}]";
        }
    }

    public class IfStmtNode : StatementNode
    {
        public ExpressionNode Condition { get; set; }
        public StatementNode ThenBranch { get; set; }
        public StatementNode ElseBranch { get; set; }

        public override string ToString()
        {
            if (ElseBranch == null)
                return $"If[{Condition}] then {ThenBranch}";
            return $"If[{Condition}] then {ThenBranch} else {ElseBranch}";
        }
    }

    public class WhileStmtNode : StatementNode
    {
        public ExpressionNode Condition { get; set; }
        public StatementNode Body { get; set; }

        public override string ToString() => $"While[{Condition}] {Body}";
    }

    public class DoWhileStmtNode : StatementNode
    {
        public StatementNode Body { get; set; }
        public ExpressionNode Condition { get; set; }

        public override string ToString() => $"DoWhile[{Body} while {Condition}]";
    }

    public class ForStmtNode : StatementNode
    {
        public StatementNode Init { get; set; }
        public ExpressionNode Condition { get; set; }
        public ExpressionNode Increment { get; set; }
        public StatementNode Body { get; set; }

        public override string ToString()
        {
            return $"For[init: {Init}, cond: {Condition}, inc: {Increment}] {Body}";
        }
    }

    public class BreakStmtNode : StatementNode
    {
        public override string ToString() => "Break[]";
    }

    public class ContinueStmtNode : StatementNode
    {
        public override string ToString() => "Continue[]";
    }

}

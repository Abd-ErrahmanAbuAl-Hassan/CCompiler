using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.Nodes
{
    public abstract class ExpressionNode : AstNode { }

    public class NumberExpressionNode : ExpressionNode
    {
        public string Value { get; set; }
        public override string ToString() => $"Num[{Value}]";
    }

    public class StringExpressionNode : ExpressionNode
    {
        public string Value { get; set; }
        public override string ToString() => $"Str[\"{Value}\"]";
    }

    public class CharacterExpressionNode : ExpressionNode
    {
        public char Value { get; set; }
        public override string ToString() => $"Char['{Value}']";
    }

    public class IdentifierExpressionNode : ExpressionNode
    {
        public string Name { get; set; }
        public override string ToString() => $"Id[{Name}]";
    }

    public class BinaryExpressionNode : ExpressionNode
    {
        public string Op { get; set; }
        public ExpressionNode Left { get; set; }
        public ExpressionNode Right { get; set; }

        public override string ToString() => $"BinOp[{Left} {Op} {Right}]";
    }

    public class UnaryExpressionNode : ExpressionNode
    {
        public string Op { get; set; }
        public ExpressionNode Operand { get; set; }
        public bool IsPrefix { get; set; } = true;

        public override string ToString()
        {
            if (IsPrefix)
                return $"Unary[{Op}{Operand}]";
            return $"Unary[{Operand}{Op}]";
        }
    }

    public class AssignExpressionNode : ExpressionNode
    {
        public ExpressionNode Left { get; set; }
        public ExpressionNode Right { get; set; }
        public string Op { get; set; } = "="; // For compound assignments

        public override string ToString() => $"Assign[{Left} {Op} {Right}]";
    }

    public class CallExpressionNode : ExpressionNode
    {
        public ExpressionNode Callee { get; set; }
        public List<ExpressionNode> Args { get; } = new();

        public override string ToString()
        {
            var argsStr = string.Join(", ", Args);
            return $"Call[{Callee}({argsStr})]";
        }
    }

    public class IndexExpressionNode : ExpressionNode
    {
        public ExpressionNode Target { get; set; }
        public ExpressionNode Index { get; set; }

        public override string ToString() => $"Index[{Target}[{Index}]]";
    }

    public class MemberAccessExpressionNode : ExpressionNode
    {
        public ExpressionNode Target { get; set; }
        public string MemberName { get; set; }
        public bool IsArrow { get; set; } // true for ->, false for .

        public override string ToString()
        {
            var op = IsArrow ? "->" : ".";
            return $"Member[{Target}{op}{MemberName}]";
        }
    }

    public class TernaryExpressionNode : ExpressionNode
    {
        public ExpressionNode Condition { get; set; }
        public ExpressionNode TrueExpr { get; set; }
        public ExpressionNode FalseExpr { get; set; }

        public override string ToString() => $"Ternary[{Condition} ? {TrueExpr} : {FalseExpr}]";
    }
}

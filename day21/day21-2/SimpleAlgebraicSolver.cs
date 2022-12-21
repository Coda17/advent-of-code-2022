using System.Linq.Expressions;

namespace day21_2;

public class SimpleAlgebraicSolver : ExpressionVisitor
{
    protected override Expression VisitBinary(BinaryExpression node)
    {
        // Reduce the expression first to make it simpler.
        var reducer = new BinaryExpressionReducer();
        if (node.NodeType == ExpressionType.Equal)
        {
            node = (BinaryExpression) reducer.Visit(node);
        }

        var lhs = node.Left;
        var rhs = node.Right;

        // Assume the lhs always has the parameter because I'm lazy.
        while (lhs is BinaryExpression be)
        {
            var lhsIsConstant = be.Left.NodeType == ExpressionType.Constant;
            var constant = lhsIsConstant ? be.Left : be.Right;
            var nodeType = lhs.NodeType;

            if (lhs is ConstantExpression && rhs is ParameterExpression ||
                lhs is ParameterExpression && rhs is ConstantExpression)
            {
                break;
            }

            rhs = nodeType switch
            {
                ExpressionType.Add => Expression.Subtract(rhs, constant),
                ExpressionType.Subtract when !lhsIsConstant => Expression.Add(rhs, constant),
                ExpressionType.Subtract when lhsIsConstant => Expression.Subtract(constant, rhs),
                ExpressionType.Multiply => Expression.Divide(rhs, constant),
                ExpressionType.Divide => Expression.Multiply(rhs, constant),
                _ => throw new Exception("Invalid node type.")
            };

            lhs = lhsIsConstant ? be.Right : be.Left;

            lhs = reducer.Visit(lhs);
            rhs = reducer.Visit(rhs);
        }

        return Expression.Equal(lhs, rhs);
    }
}
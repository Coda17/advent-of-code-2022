using System.Linq.Expressions;

namespace day21_2;

public class BinaryExpressionReducer : ExpressionVisitor
{
    protected override Expression VisitBinary(BinaryExpression node)
    { 
        var left = node.Left is ParameterExpression or ConstantExpression ? node.Left : Visit(node.Left);
        var right = node.Right is ParameterExpression or ConstantExpression ? node.Right : Visit(node.Right);

        if (left is ConstantExpression ceLeft && right is ConstantExpression ceRight)
        {
            return node.NodeType switch
            {
                ExpressionType.Add => Expression.Constant((long)ceLeft.Value! + (long)ceRight.Value!, typeof(long)),
                ExpressionType.Subtract => Expression.Constant((long)ceLeft.Value! - (long)ceRight.Value!, typeof(long)),
                ExpressionType.Multiply => Expression.Constant((long)ceLeft.Value! * (long)ceRight.Value!, typeof(long)),
                ExpressionType.Divide => Expression.Constant((long)ceLeft.Value! / (long)ceRight.Value!, typeof(long)),
                _ => base.VisitBinary(node)
            };
        }

        return node.NodeType switch
        {
            ExpressionType.Add => Expression.Add(left, right),
            ExpressionType.Subtract => Expression.Subtract(left, right),
            ExpressionType.Multiply => Expression.Multiply(left, right),
            ExpressionType.Divide => Expression.Divide(left, right),
            _ => base.VisitBinary(node)
        };
    }
}
/*
created by zou jian
*/

using System.Linq.Expressions;

namespace Fissoft
{
    public static class ExpressionExtensions
    {
        public static Expression RemoveUnary(this Expression body)
        {
            return body is UnaryExpression uniary ? uniary.Operand : body;
        }
    }
}
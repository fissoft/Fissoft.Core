/*
created by zou jian
*/
using System.Linq.Expressions;

namespace Fissoft.Framework.Systems
{
    public static class ExpressionExtensions
    {
        public static Expression RemoveUnary(this Expression body)
        {
            var uniary = body as UnaryExpression;
            return uniary != null ? uniary.Operand : body;
        }
    }
}
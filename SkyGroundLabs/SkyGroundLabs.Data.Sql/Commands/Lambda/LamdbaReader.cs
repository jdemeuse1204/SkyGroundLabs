using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SkyGroundLabs.Data.Sql.Commands.Lambda
{
    public class LamdbaReader
    {
        public string ResolveToSql<T>(Expression<Func<T, bool>> expression) where T : class
        {
            
        }

        #region Expression Conversion
        private BinaryExpression _toBinaryExpression(Expression expression)
        {
            var result = expression as BinaryExpression;

            if (result == null)
            {
                throw new ArgumentException("Expected a Binary Expression!");
            }

            return result;
        }

        private MemberExpression _toMemberExpression(Expression expression)
        {
            var result = expression as MemberExpression;

            if (result == null)
            {
                throw new ArgumentException("Expected a Member Expression!");
            }

            return result;
        }

        #endregion

        private void _evaluateExpressionTree(Expression expression, SqlQueryBuilder builder, string tableName, IEnumerable<PropertyInfo> properties)
        {
            if (_hasLeft(expression))
            {
                var result = _evaluate((expression as BinaryExpression).Right);
                var dbColumnName = _findDbColumnName(properties, result.PropertyName);

                _addWhereToBuilder(builder, result, dbColumnName, tableName);
                _evaluateExpressionTree((expression as BinaryExpression).Left, builder, tableName, properties);
            }
            else
            {
                var result = _evaluate(expression as BinaryExpression);
                var dbColumnName = _findDbColumnName(properties, result.PropertyName);

                _addWhereToBuilder(builder, result, dbColumnName, tableName);
            }
        }

        private void _addWhereToBuilder(SqlQueryBuilder builder, dynamic expressionData, string dbColumnName, string tableName)
        {
            builder.AddWhere(tableName, dbColumnName, expressionData.Comparison, expressionData.Value);
        }

        private bool _hasLeft(Expression expression)
        {
            return expression.NodeType == ExpressionType.And
                || expression.NodeType == ExpressionType.AndAlso
                || expression.NodeType == ExpressionType.Or
                || expression.NodeType == ExpressionType.OrElse;
        }

        private object _getRightSideValue(dynamic rightSide)
        {
            if (rightSide.NodeType == ExpressionType.Constant)
            {
                return rightSide.Value;
            }
            else
            {
                // Need to evaluate the expression to get the result
                // member access
                Type t = rightSide.Expression.Value.GetType();
                var result = t.InvokeMember(
                    rightSide.Member.Name,
                    BindingFlags.GetField,
                    null,
                    rightSide.Expression.Value,
                    null);

                return result;
            }
        }

        private dynamic _evaluate(Expression expression)
        {
            dynamic result = new ExpandoObject();

            // left and right side are internals so set to dynamics
            var leftSide = (expression as BinaryExpression).Left as dynamic;
            var rightSide = (expression as BinaryExpression).Right as dynamic;

            // check for conversions like enums
            if (leftSide.NodeType == ExpressionType.Convert)
            {
                result.PropertyName = leftSide.Operand.Member.Name;
                result.Value = _getRightSideValue(rightSide);
                result.Comparison = ComparisonType.Equals;
            }
            else
            {
                result.PropertyName = leftSide.Member.Name;
                result.Value = _getRightSideValue(rightSide);
                result.Comparison = ComparisonType.Equals;
            }

            // make sure value is not null
            if (result.Value == null)
            {
                result.Value = DBNull.Value;
            }

            // set our comparison type
            switch (expression.NodeType)
            {
                case ExpressionType.Equal:
                    result.Comparison = ComparisonType.Equals;
                    break;
                case ExpressionType.GreaterThan:
                    result.Comparison = ComparisonType.GreaterThan;
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    result.Comparison = ComparisonType.Equals;
                    break;
                case ExpressionType.LessThan:
                    result.Comparison = ComparisonType.Equals;
                    break;
                case ExpressionType.LessThanOrEqual:
                    result.Comparison = ComparisonType.Equals;
                    break;
                case ExpressionType.NotEqual:
                    result.Comparison = ComparisonType.Equals;
                    break;
                default:
                    throw new Exception("ExpressionType not in tree");
            }


            return result;
        }
    }
}

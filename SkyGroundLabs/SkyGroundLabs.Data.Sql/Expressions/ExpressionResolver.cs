using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using SkyGroundLabs.Data.Sql.Commands;
using SkyGroundLabs.Data.Sql.Data;
using SkyGroundLabs.Data.Sql.Mapping;

namespace SkyGroundLabs.Data.Sql.Expressions
{
    public abstract class ExpressionResolver
    {
        protected static IEnumerable<ExpressionWhereResult> ResolveWhere<T>(Expression<Func<T, bool>> expression)
        {
            var evaluationResults = new List<ExpressionWhereResult>();
            // lambda string, tablename
            var TableNameLookup = new Dictionary<string, string>();

            for (var i = 0; i < expression.Parameters.Count; i++)
            {
                var parameter = expression.Parameters[i];

                TableNameLookup.Add(parameter.Name, DatabaseSchemata.GetTableName(parameter.Type));
            }

            _evaluateExpressionTree(expression.Body, evaluationResults, TableNameLookup);

            return evaluationResults;
        }

        protected static IEnumerable<ExpressionWhereResult> ResolveJoin<TParent, TChild>(Expression<Func<TParent, TChild, bool>> expression)
        {
            var evaluationResults = new List<ExpressionWhereResult>();
            // lambda string, tablename
            var TableNameLookup = new Dictionary<string, string>();

            for (var i = 0; i < expression.Parameters.Count; i++)
            {
                var parameter = expression.Parameters[i];

                TableNameLookup.Add(parameter.Name, DatabaseSchemata.GetTableName(parameter.Type));
            }

            _evaluateExpressionTree(expression.Body, evaluationResults, TableNameLookup);

            return evaluationResults;
        }

        protected static IEnumerable<ExpressionSelectResult> ResolveSelect<T>(Expression<Func<T, object>> expression)
        {
            var evaluationResults = new List<ExpressionSelectResult>();
            // lambda string, tablename
            var TableNameLookup = new Dictionary<string, string>();

            for (var i = 0; i < expression.Parameters.Count; i++)
            {
                var parameter = expression.Parameters[i];

                TableNameLookup.Add(parameter.Name, DatabaseSchemata.GetTableName(parameter.Type));
            }

            _evaltateSelectExpressionTree(expression.Body, evaluationResults, TableNameLookup);

            return evaluationResults;
        }

        private static void _evaltateSelectExpressionTree(Expression expression, ICollection<ExpressionSelectResult> evaluationResults,
            Dictionary<string, string> TableNameLookup)
        {
            if (expression.NodeType == ExpressionType.New)
            {
                var e = expression as NewExpression;

                for (var i = 0; i < e.Arguments.Count; i++)
                {
                    var arg = e.Arguments[i] as MemberExpression;
                    var columnAndTableName = _getColumnNameAndParameter(arg, TableNameLookup);

                    evaluationResults.Add(new ExpressionSelectResult
                    {
                        ColumnName = columnAndTableName.ColumnName,
                        TableName = columnAndTableName.TableName
                    });
                }
            }
            else if (expression.NodeType == ExpressionType.Convert)
            {
                var columnAndTableName = _getTableName(expression as dynamic, TableNameLookup);

                evaluationResults.Add(new ExpressionSelectResult
                {
                    ColumnName = columnAndTableName.ColumnName,
                    TableName = columnAndTableName.TableName
                });
            }
        }

        private static void _evaluateExpressionTree(Expression expression, ICollection<ExpressionWhereResult> evaluationResults, Dictionary<string, string> TableNameLookup)
        {
            if (_hasLeft(expression))
            {
                var result = _evaluate(((dynamic)expression).Right, TableNameLookup);

                evaluationResults.Add(result);

                _evaluateExpressionTree((expression as BinaryExpression).Left, evaluationResults, TableNameLookup);
            }
            else
            {
                var result = _evaluate(expression as dynamic, TableNameLookup);

                evaluationResults.Add(result);
            }
        }

        #region Helpers
        private static bool _hasLeft(Expression expression)
        {
            return expression.NodeType == ExpressionType.And
                || expression.NodeType == ExpressionType.AndAlso
                || expression.NodeType == ExpressionType.Or
                || expression.NodeType == ExpressionType.OrElse;
        }

        private static ExpressionSelectResult _getTableName(UnaryExpression expression, Dictionary<string, string> TableNameLookup)
        {
            var parameterExpression = expression.Operand as ParameterExpression;

            return new ExpressionSelectResult
            {
                TableName = TableNameLookup.ContainsKey(parameterExpression.Name) ? TableNameLookup[parameterExpression.Name] : parameterExpression.Name,
                ColumnName = "*"
            };
        }

        private static ExpressionSelectResult _getColumnNameAndParameter(object expression, Dictionary<string, string> TableNameLookup)
        {
            if (expression is MethodCallExpression)
            {
                return _getColumnNameAndParameter(((MethodCallExpression)expression).Object, TableNameLookup);
            }

            var e = expression as MemberExpression;

            return e.Expression.NodeType == ExpressionType.Parameter
                ? (new ExpressionSelectResult
                {
                    TableName = TableNameLookup.ContainsKey(((dynamic)e.Expression).Name) ? TableNameLookup[((dynamic)e.Expression).Name] : ((dynamic)e.Expression).Name,
                    ColumnName = e.Member.GetCustomAttribute<ColumnAttribute>() == null
                            ? e.Member.Name
                            : e.Member.GetCustomAttribute<ColumnAttribute>().Name
                })
                : _getColumnNameAndParameter(e.Expression as MemberExpression, TableNameLookup);
        }

        private static bool _hasParameter(object expression)
        {
            if (expression == null)
            {
                return false;
            }

            if (((Expression)expression).NodeType == ExpressionType.Parameter)
            {
                return true;
            }

            if (expression is ConstantExpression)
            {
                return false;
            }

            if (expression is MethodCallExpression)
            {
                return _hasParameter(((MethodCallExpression)expression).Object);
            }

            return _hasParameter(((MemberExpression)expression).Expression);
        }
        #endregion

        #region Expression Evaluation
        private static ExpressionWhereResult _evaluate(MethodCallExpression expression, Dictionary<string, string> TableNameLookup)
        {
            var result = new ExpressionWhereResult();
            var columnOptions = _getColumnNameAndParameter(expression.Arguments[0] is MemberExpression ? expression.Arguments[0] as dynamic : expression.Object as dynamic, TableNameLookup);
            result.PropertyName = columnOptions.ColumnName;
            result.TableName = columnOptions.TableName;
            result.CompareValue = _getValue((!(expression.Arguments[0] is MemberExpression)) ? expression.Arguments[0] as dynamic : expression.Object as dynamic);
            result.ComparisonType = _getComparisonType(expression.Method.Name);

            return result;
        }

        private static ExpressionWhereResult _evaluate(BinaryExpression expression, Dictionary<string, string> TableNameLookup)
        {
            var result = new ExpressionWhereResult();
            var leftSide = expression.Left;
            var rightSide = expression.Right;

            // need to check which side has the lambda parameter
            var leftSideHasParameter = _hasParameter(leftSide);
            var rightSideHasParameter = _hasParameter(rightSide);
            var columnOptions = _getColumnNameAndParameter(leftSideHasParameter ? leftSide : rightSide, TableNameLookup);

            result.PropertyName = columnOptions.ColumnName;
            result.TableName = columnOptions.TableName;

            // cast as dynamic so runtime can choose which method to use
            result.CompareValue = rightSideHasParameter ? _getColumnNameAndParameter(rightSide, TableNameLookup) : _getValue(leftSideHasParameter ? rightSide as dynamic : leftSide as dynamic);
            result.ComparisonType = _getComparisonType(expression.NodeType);

            return result;
        }

        private static ExpressionWhereResult _evaluate(UnaryExpression expression, Dictionary<string, string> TableNameLookup)
        {
            var result = _evaluate(expression.Operand as dynamic, TableNameLookup);

            result.ComparisonType = _getComparisonType(expression.NodeType);

            return result;
        }
        #endregion

        #region Get Comparison Types
        private static ComparisonType _getComparisonType(ExpressionType expresssionType)
        {
            switch (expresssionType)
            {
                case ExpressionType.Equal:
                    return ComparisonType.Equals;
                case ExpressionType.GreaterThan:
                    return ComparisonType.GreaterThan;
                case ExpressionType.GreaterThanOrEqual:
                    return ComparisonType.GreaterThanEquals;
                case ExpressionType.LessThan:
                    return ComparisonType.LessThan;
                case ExpressionType.LessThanOrEqual:
                    return ComparisonType.LessThanEquals;
                case ExpressionType.NotEqual:
                    return ComparisonType.NotEqual;
                case ExpressionType.Not:
                    return ComparisonType.NotEqual;
                default:
                    throw new Exception("ExpressionType not in tree");
            }
        }

        private static ComparisonType _getComparisonType(string expresssionType)
        {
            switch (expresssionType.ToUpper())
            {
                case "EQUALS":
                    return ComparisonType.Equals;
                case "NOT EQUALS":
                    return ComparisonType.GreaterThan;
                case "CONTAINS":
                    return ComparisonType.Contains;
                case "EQUsALS":
                    return ComparisonType.LessThan;
                case "EQUfgALS":
                    return ComparisonType.LessThanEquals;
                case "EQUaALS":
                    return ComparisonType.NotEqual;
                default:
                    throw new Exception("ExpressionType not in tree");
            }
        }
        #endregion

        #region Get Expression Values
        private static object _getValue(ConstantExpression expression)
        {
            return expression.Value;
        }

        private static object _getValue(MemberExpression expression)
        {
            var objectMember = Expression.Convert(expression, typeof(object));

            var getterLambda = Expression.Lambda<Func<object>>(objectMember);

            var getter = getterLambda.Compile();

            return getter();
        }

        private static object _getValue(MethodCallExpression expression)
        {
            var objectMember = Expression.Convert(expression, typeof(object));

            var getterLambda = Expression.Lambda<Func<object>>(objectMember);

            var getter = getterLambda.Compile();

            return getter();
        }
        #endregion

        //private static object _getValue(dynamic rightSide)
        //{
        //    Type t = rightSide.Expression.Value.GetType();
        //    var result = t.InvokeMember(
        //        rightSide.Member.Name,
        //        BindingFlags.GetField,
        //        null,
        //        rightSide.Expression.Value,
        //        null);

        //    return result;
        //}
    }
}

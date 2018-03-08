using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace RuleEngine.Utils
{
    public static class ExpressionExtensions
    {
        static ExpressionExtensions(){}
        private const int NumberOfSpaces = 2;
        public static string GetDebugView(this Expression exp)
        {
            if (exp == null)
                return null;

            var propertyInfo = typeof(Expression).GetProperty("DebugView", BindingFlags.Instance | BindingFlags.NonPublic);
            return propertyInfo.GetValue(exp) as string;
        }

        public static void TraceNode(this Expression expression, StringBuilder stringBuilder, int level = 0)
        {
            if (expression == null || stringBuilder == null) return;
            switch (expression)
            {
                case BinaryExpression binaryExpression:
                    binaryExpression.TraceNode(stringBuilder, level);
                    break;
                case ConstantExpression constantExpression:
                    constantExpression.TraceNode(stringBuilder, level);
                    break;
                case MemberExpression memberExpression:
                    memberExpression.TraceNode(stringBuilder, level);
                    break;
                case ParameterExpression parameterExpression:
                    parameterExpression.TraceNode(stringBuilder, level);
                    break;
                case LambdaExpression lambdaExpression:
                    lambdaExpression.TraceNode(stringBuilder, level);
                    break;
                case Expression expression2:
                    var levelSpace = new string(' ', level * NumberOfSpaces);
                    stringBuilder.Append($"|{levelSpace}|- Expression Type: {expression.GetType().Name}{Environment.NewLine}");
                    stringBuilder.Append($"|{levelSpace}|- Expression.NodeType: {expression2.NodeType}{Environment.NewLine}");
                    stringBuilder.Append($"|{levelSpace}|- Expression.DebugView: {expression2.GetDebugView()}{Environment.NewLine}");
                    break;
            }
        }

        public static void TraceNode(this BinaryExpression binaryExpression, StringBuilder stringBuilder, int level = 0)
        {
            if (stringBuilder == null) return;
            var levelSpace = new string(' ', level * NumberOfSpaces);
            stringBuilder.Append($"|{levelSpace}|- binaryExpression.NodeType: {binaryExpression.NodeType}{Environment.NewLine}");
            stringBuilder.Append($"|{levelSpace}|- binaryExpression.DebugView: {binaryExpression.GetDebugView()}{Environment.NewLine}");

            level++;
            binaryExpression.Left?.TraceNode(stringBuilder, level);
            binaryExpression.Right?.TraceNode(stringBuilder, level);
        }

        public static void TraceNode(this BlockExpression blockExpression, StringBuilder stringBuilder, int level = 0)
        {
            if (stringBuilder == null) return;
            var levelSpace = new string(' ', level * NumberOfSpaces);
            stringBuilder.Append($"|{levelSpace}|- blockExpression.NodeType: {blockExpression.NodeType}{Environment.NewLine}");
            stringBuilder.Append($"|{levelSpace}|- blockExpression.DebugView: {blockExpression.GetDebugView()}{Environment.NewLine}");

            level++;
            stringBuilder.Append($"|{levelSpace}|- Variables count: {blockExpression.Variables.Count}{Environment.NewLine}");
            foreach (var blockExpressionVariable in blockExpression.Variables)
                blockExpressionVariable.TraceNode(stringBuilder, level);

            stringBuilder.Append($"|{levelSpace}|- Expressions count: {blockExpression.Expressions.Count}{Environment.NewLine}");
            foreach (var blockExpressionExpression in blockExpression.Expressions)
                blockExpressionExpression.TraceNode(stringBuilder, level);
        }

        public static void TraceNode(this ConditionalExpression conditionalExpression, StringBuilder stringBuilder, int level = 0)
        {
            if (stringBuilder == null) return;
            var levelSpace = new string(' ', level * NumberOfSpaces);
            stringBuilder.Append($"|{levelSpace}|- conditionalExpression.NodeType: {conditionalExpression.NodeType}{Environment.NewLine}");
            stringBuilder.Append($"|{levelSpace}|- conditionalExpression.DebugView: {conditionalExpression.GetDebugView()}{Environment.NewLine}");

            level++;
            stringBuilder.Append($"|{levelSpace}|- IfFalse:{Environment.NewLine}");
            conditionalExpression.IfFalse.TraceNode(stringBuilder, level);
            stringBuilder.Append($"|{levelSpace}|- IfTrue:{Environment.NewLine}");
            conditionalExpression.IfTrue.TraceNode(stringBuilder, level);
        }

        public static void TraceNode(this ConstantExpression constantExpression, StringBuilder stringBuilder, int level = 0)
        {
            if (stringBuilder == null) return;
            var levelSpace = new string(' ', level * NumberOfSpaces);
            stringBuilder.Append($"|{levelSpace}|- constantExpression.Value: {constantExpression.Value ?? "null"}{Environment.NewLine}");
            stringBuilder.Append($"|{levelSpace}|- constantExpression.Type: {constantExpression.Type}{Environment.NewLine}");
            stringBuilder.Append($"|{levelSpace}|- constantExpression.DebugView: {constantExpression.GetDebugView()}{Environment.NewLine}");
        }

        public static void TraceNode(this DynamicExpression dynamicExpression, StringBuilder stringBuilder, int level = 0)
        {
            if (stringBuilder == null) return;
            var levelSpace = new string(' ', level * NumberOfSpaces);
            stringBuilder.Append($"|{levelSpace}|- dynamicExpression.NodeType: {dynamicExpression.NodeType}{Environment.NewLine}");
            stringBuilder.Append($"|{levelSpace}|- dynamicExpression.DebugView: {dynamicExpression.GetDebugView()}{Environment.NewLine}");
        }

        public static void TraceNode(this LambdaExpression lambdaExpression, StringBuilder stringBuilder, int level = 0)
        {
            if (stringBuilder == null) return;
            var levelSpace = new string(' ', level * NumberOfSpaces);
            stringBuilder.Append($"|{levelSpace}|- lambdaExpression.NodeType: {lambdaExpression.NodeType}{Environment.NewLine}");
            stringBuilder.Append($"|{levelSpace}|- lambdaExpression.DebugView: {lambdaExpression.GetDebugView()}{Environment.NewLine}");

            var nextLevel = level + 1;
            stringBuilder.Append($"|{levelSpace}|- Parameters count: {lambdaExpression.Parameters.Count}");
            foreach (var lambdaExpressionParameter in lambdaExpression.Parameters)
                lambdaExpressionParameter.TraceNode(stringBuilder, nextLevel);

            stringBuilder.Append("|");
            stringBuilder.Append($"|{levelSpace}|- Body [{lambdaExpression.Body.NodeType}]{Environment.NewLine}");
            lambdaExpression.Body?.TraceNode(stringBuilder, nextLevel);
        }

        public static void TraceNode(this MemberExpression memberExpression, StringBuilder stringBuilder, int level = 0)
        {
            if (stringBuilder == null) return;
            var levelSpace = new string(' ', level * NumberOfSpaces);
            stringBuilder.Append($"|{levelSpace}|- memberExpression.NodeType: {memberExpression.NodeType}{Environment.NewLine}");
            stringBuilder.Append($"|{levelSpace}|- memberExpression.DebugView: {memberExpression.GetDebugView()}{Environment.NewLine}");
        }

        public static void TraceNode(this MethodCallExpression methodCallExpression, StringBuilder stringBuilder, int level = 0)
        {
            if (stringBuilder == null) return;
            var levelSpace = new string(' ', level * NumberOfSpaces);
            stringBuilder.Append($"|{levelSpace}|- methodCallExpression.NodeType: {methodCallExpression.NodeType}{Environment.NewLine}");
            stringBuilder.Append($"|{levelSpace}|- methodCallExpression.DebugView: {methodCallExpression.GetDebugView()}{Environment.NewLine}");
        }

        public static void TraceNode(this NewExpression newExpression, StringBuilder stringBuilder, int level = 0)
        {
            if (stringBuilder == null) return;
            var levelSpace = new string(' ', level * NumberOfSpaces);
            stringBuilder.Append($"|{levelSpace}|- newExpression.NodeType: {newExpression.NodeType}{Environment.NewLine}");
            stringBuilder.Append($"|{levelSpace}|- newExpression.DebugView: {newExpression.GetDebugView()}{Environment.NewLine}");
        }

        public static void TraceNode(this ParameterExpression parameterExpression, StringBuilder stringBuilder, int level = 0)
        {
            if (stringBuilder == null) return;
            var levelSpace = new string(' ', level * NumberOfSpaces);
            stringBuilder.Append($"|{levelSpace}|- parameterExpression.Name: {parameterExpression.Name ?? "null"}{Environment.NewLine}");
            stringBuilder.Append($"|{levelSpace}|- parameterExpression.Type: {parameterExpression.Type}{Environment.NewLine}");
            stringBuilder.Append($"|{levelSpace}|- parameterExpression.DebugView: {parameterExpression.GetDebugView()}{Environment.NewLine}");
        }
    }
}
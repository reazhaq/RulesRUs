using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace RuleEngine.Utils
{
    public static class ExpressionExtensions
    {
        private static readonly string Nl = Environment.NewLine;
        private const int NumberOfSpaces = 2;
        public static string GetDebugView(this Expression exp)
        {
            if (exp == null)
                return null;

            var propertyInfo = typeof(Expression).GetProperty("DebugView", BindingFlags.Instance | BindingFlags.NonPublic);
            return propertyInfo.GetValue(exp) as string;
        }

        public static Expression GetInnerExpression(this Expression exp)
        {
            if (exp == null) return null;
            var propertyInfo = typeof(Expression).GetProperty("Expression", BindingFlags.Instance|BindingFlags.NonPublic);
            return propertyInfo?.GetValue(exp) as Expression;
        }

        public static void TraceNode(this Expression expression, StringBuilder sb, int level = 0)
        {
            if (expression == null || sb == null) return;
            switch (expression)
            {
                case BinaryExpression binaryExpression:
                    binaryExpression.TraceNode(sb, level);
                    break;
                case ConstantExpression constantExpression:
                    constantExpression.TraceNode(sb, level);
                    break;
                case MemberExpression memberExpression:
                    memberExpression.TraceNode(sb, level);
                    break;
                case ParameterExpression parameterExpression:
                    parameterExpression.TraceNode(sb, level);
                    break;
                case LambdaExpression lambdaExpression:
                    lambdaExpression.TraceNode(sb, level);
                    break;
                case UnaryExpression unaryExpression:
                    unaryExpression.TraceNode(sb, level);
                    break;
                case Expression expression2:
                    var levelSpace = new string(' ', level * NumberOfSpaces);
                    sb.Append($"|{levelSpace}|- Expression Type: {expression.GetType().Name}{Nl}");
                    sb.Append($"|{levelSpace}|- Expression.NodeType: {expression2.NodeType}{Nl}");
                    sb.Append($"|{levelSpace}|- Expression.DebugView: {expression2.GetDebugView()}{Nl}");
                    break;
            }
        }

        public static void TraceNode(this UnaryExpression unaryExpression, StringBuilder sb, int level = 0)
        {
            if (sb == null) return;
            var levelSpace = new string(' ', level * NumberOfSpaces);
            sb.Append($"|{levelSpace}|- unaryExpression.NodeType: {unaryExpression.NodeType}{Nl}");
            sb.Append($"|{levelSpace}|- unaryExpression.DebugView: {unaryExpression.GetDebugView()}{Nl}");
            sb.Append($"|{levelSpace}|- unaryExpression.Method: {unaryExpression.Method}{Nl}");
            sb.Append($"|{levelSpace}|- unaryExpression.Operand:{Nl}");

            level++;
            unaryExpression.Operand.TraceNode(sb, level);

            level++;
            var innerExpression = unaryExpression.Operand.GetInnerExpression();
            innerExpression.TraceNode(sb, level);
        }

        public static void TraceNode(this BinaryExpression binaryExpression, StringBuilder sb, int level = 0)
        {
            if (sb == null) return;
            var levelSpace = new string(' ', level * NumberOfSpaces);
            sb.Append($"|{levelSpace}|- binaryExpression.NodeType: {binaryExpression.NodeType}{Nl}");
            sb.Append($"|{levelSpace}|- binaryExpression.DebugView: {binaryExpression.GetDebugView()}{Nl}");

            level++;
            binaryExpression.Left?.TraceNode(sb, level);
            binaryExpression.Right?.TraceNode(sb, level);
        }

        public static void TraceNode(this BlockExpression blockExpression, StringBuilder sb, int level = 0)
        {
            if (sb == null) return;
            var levelSpace = new string(' ', level * NumberOfSpaces);
            sb.Append($"|{levelSpace}|- blockExpression.NodeType: {blockExpression.NodeType}{Nl}");
            sb.Append($"|{levelSpace}|- blockExpression.DebugView: {blockExpression.GetDebugView()}{Nl}");

            level++;
            sb.Append($"|{levelSpace}|- Variables count: {blockExpression.Variables.Count}{Nl}");
            foreach (var blockExpressionVariable in blockExpression.Variables)
                blockExpressionVariable.TraceNode(sb, level);

            sb.Append($"|{levelSpace}|- Expressions count: {blockExpression.Expressions.Count}{Nl}");
            foreach (var blockExpressionExpression in blockExpression.Expressions)
                blockExpressionExpression.TraceNode(sb, level);
        }

        public static void TraceNode(this ConditionalExpression conditionalExpression, StringBuilder sb, int level = 0)
        {
            if (sb == null) return;
            var levelSpace = new string(' ', level * NumberOfSpaces);
            sb.Append($"|{levelSpace}|- conditionalExpression.NodeType: {conditionalExpression.NodeType}{Nl}");
            sb.Append($"|{levelSpace}|- conditionalExpression.DebugView: {conditionalExpression.GetDebugView()}{Nl}");

            level++;
            sb.Append($"|{levelSpace}|- IfFalse:{Nl}");
            conditionalExpression.IfFalse.TraceNode(sb, level);
            sb.Append($"|{levelSpace}|- IfTrue:{Nl}");
            conditionalExpression.IfTrue.TraceNode(sb, level);
        }

        public static void TraceNode(this ConstantExpression constantExpression, StringBuilder sb, int level = 0)
        {
            if (sb == null) return;
            var levelSpace = new string(' ', level * NumberOfSpaces);
            sb.Append($"|{levelSpace}|- constantExpression.Value: {constantExpression.Value ?? "null"}{Nl}");
            sb.Append($"|{levelSpace}|- constantExpression.Type: {constantExpression.Type}{Nl}");
            sb.Append($"|{levelSpace}|- constantExpression.DebugView: {constantExpression.GetDebugView()}{Nl}");
        }

        public static void TraceNode(this DynamicExpression dynamicExpression, StringBuilder sb, int level = 0)
        {
            if (sb == null) return;
            var levelSpace = new string(' ', level * NumberOfSpaces);
            sb.Append($"|{levelSpace}|- dynamicExpression.NodeType: {dynamicExpression.NodeType}{Nl}");
            sb.Append($"|{levelSpace}|- dynamicExpression.DebugView: {dynamicExpression.GetDebugView()}{Nl}");
        }

        public static void TraceNode(this LambdaExpression lambdaExpression, StringBuilder sb, int level = 0)
        {
            if (sb == null) return;
            var levelSpace = new string(' ', level * NumberOfSpaces);
            sb.Append($"|{levelSpace}|- lambdaExpression.NodeType: {lambdaExpression.NodeType}{Nl}");
            sb.Append($"|{levelSpace}|- lambdaExpression.DebugView: {lambdaExpression.GetDebugView()}{Nl}");

            var nextLevel = level + 1;
            sb.Append($"|{levelSpace}|- Parameters count: {lambdaExpression.Parameters.Count}{Nl}");
            foreach (var lambdaExpressionParameter in lambdaExpression.Parameters)
                lambdaExpressionParameter.TraceNode(sb, nextLevel);

            sb.Append($"|{Nl}");
            sb.Append($"|{levelSpace}|- Body [{lambdaExpression.Body.NodeType}]{Nl}");
            lambdaExpression.Body?.TraceNode(sb, nextLevel);
        }

        public static void TraceNode(this MemberExpression memberExpression, StringBuilder sb, int level = 0)
        {
            if (sb == null) return;
            var levelSpace = new string(' ', level * NumberOfSpaces);
            sb.Append($"|{levelSpace}|- memberExpression.NodeType: {memberExpression.NodeType}{Nl}");
            sb.Append($"|{levelSpace}|- memberExpression.DebugView: {memberExpression.GetDebugView()}{Nl}");
            sb.Append($"|{levelSpace}|- memberExpression.Member - field: {(memberExpression.Member as FieldInfo)?.ToString()}{Nl}");
            sb.Append($"|{levelSpace}|- memberExpression.Member - prop: {(memberExpression.Member as PropertyInfo)?.ToString()}{Nl}");
        }

        public static void TraceNode(this MethodCallExpression methodCallExpression, StringBuilder sb, int level = 0)
        {
            if (sb == null) return;
            var levelSpace = new string(' ', level * NumberOfSpaces);
            sb.Append($"|{levelSpace}|- methodCallExpression.NodeType: {methodCallExpression.NodeType}{Nl}");
            sb.Append($"|{levelSpace}|- methodCallExpression.DebugView: {methodCallExpression.GetDebugView()}{Nl}");
        }

        public static void TraceNode(this NewExpression newExpression, StringBuilder sb, int level = 0)
        {
            if (sb == null) return;
            var levelSpace = new string(' ', level * NumberOfSpaces);
            sb.Append($"|{levelSpace}|- newExpression.NodeType: {newExpression.NodeType}{Nl}");
            sb.Append($"|{levelSpace}|- newExpression.DebugView: {newExpression.GetDebugView()}{Nl}");
        }

        public static void TraceNode(this ParameterExpression parameterExpression, StringBuilder sb, int level = 0)
        {
            if (sb == null) return;
            var levelSpace = new string(' ', level * NumberOfSpaces);
            sb.Append($"|{levelSpace}|- parameterExpression.Name: {parameterExpression.Name ?? "null"}{Nl}");
            sb.Append($"|{levelSpace}|- parameterExpression.Type: {parameterExpression.Type}{Nl}");
            sb.Append($"|{levelSpace}|- parameterExpression.DebugView: {parameterExpression.GetDebugView()}{Nl}");
        }
    }
}
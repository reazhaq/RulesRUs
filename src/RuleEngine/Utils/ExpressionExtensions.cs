using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

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

        public static void TraceNode(this Expression expression, int level = 0)
        {
            if (expression == null) return;
            switch (expression)
            {
                case BinaryExpression binaryExpression:
                    binaryExpression.TraceNode(level);
                    break;
                case ConstantExpression constantExpression:
                    constantExpression.TraceNode(level);
                    break;
                case MemberExpression memberExpression:
                    memberExpression.TraceNode(level);
                    break;
                case ParameterExpression parameterExpression:
                    parameterExpression.TraceNode(level);
                    break;
                case LambdaExpression lambdaExpression:
                    lambdaExpression.TraceNode(level);
                    break;
                case Expression expression2:
                    var levelSpace = new string(' ', level * NumberOfSpaces);
                    Debug.WriteLine($"|{levelSpace}|- Expression Type: {expression.GetType().Name}");
                    Debug.WriteLine($"|{levelSpace}|- Expression.NodeType: {expression2.NodeType}");
                    Debug.WriteLine($"|{levelSpace}|- Expression.DebugView: {expression2.GetDebugView()}");
                    break;
            }
        }

        public static void TraceNode(this BinaryExpression binaryExpression, int level = 0)
        {
            var levelSpace = new string(' ', level * NumberOfSpaces);
            Debug.WriteLine($"|{levelSpace}|- binaryExpression.NodeType: {binaryExpression.NodeType}");
            Debug.WriteLine($"|{levelSpace}|- binaryExpression.DebugView: {binaryExpression.GetDebugView()}");

            level++;
            binaryExpression.Left?.TraceNode(level);
            binaryExpression.Right?.TraceNode(level);
        }

        public static void TraceNode(this BlockExpression blockExpression, int level = 0)
        {
            var levelSpace = new string(' ', level * NumberOfSpaces);
            Debug.WriteLine($"|{levelSpace}|- blockExpression.NodeType: {blockExpression.NodeType}");
            Debug.WriteLine($"|{levelSpace}|- blockExpression.DebugView: {blockExpression.GetDebugView()}");

            level++;
            Debug.WriteLine($"|{levelSpace}|- Variables count: {blockExpression.Variables.Count}");
            foreach (var blockExpressionVariable in blockExpression.Variables)
                blockExpressionVariable.TraceNode(level);

            Debug.WriteLine($"|{levelSpace}|- Expressions count: {blockExpression.Expressions.Count}");
            foreach (var blockExpressionExpression in blockExpression.Expressions)
                blockExpressionExpression.TraceNode(level);
        }

        public static void TraceNode(this ConditionalExpression conditionalExpression, int level = 0)
        {
            var levelSpace = new string(' ', level * NumberOfSpaces);
            Debug.WriteLine($"|{levelSpace}|- conditionalExpression.NodeType: {conditionalExpression.NodeType}");
            Debug.WriteLine($"|{levelSpace}|- conditionalExpression.DebugView: {conditionalExpression.GetDebugView()}");

            level++;
            Debug.WriteLine($"|{levelSpace}|- IfFalse:");
            conditionalExpression.IfFalse.TraceNode(level);
            Debug.WriteLine($"|{levelSpace}|- IfTrue:");
            conditionalExpression.IfTrue.TraceNode(level);
        }

        public static void TraceNode(this ConstantExpression constantExpression, int level = 0)
        {
            var levelSpace = new string(' ', level * NumberOfSpaces);
            Debug.WriteLine($"|{levelSpace}|- constantExpression.Value: {constantExpression.Value ?? "null"}");
            Debug.WriteLine($"|{levelSpace}|- constantExpression.Type: {constantExpression.Type}");
            Debug.WriteLine($"|{levelSpace}|- constantExpression.DebugView: {constantExpression.GetDebugView()}");
        }

        public static void TraceNode(this DynamicExpression dynamicExpression, int level = 0)
        {
            var levelSpace = new string(' ', level * NumberOfSpaces);
            Debug.WriteLine($"|{levelSpace}|- dynamicExpression.NodeType: {dynamicExpression.NodeType}");
            Debug.WriteLine($"|{levelSpace}|- dynamicExpression.DebugView: {dynamicExpression.GetDebugView()}");
        }

        public static void TraceNode(this LambdaExpression lambdaExpression, int level = 0)
        {
            var levelSpace = new string(' ', level * NumberOfSpaces);
            Debug.WriteLine($"|{levelSpace}|- lambdaExpression.NodeType: {lambdaExpression.NodeType}");
            Debug.WriteLine($"|{levelSpace}|- lambdaExpression.DebugView: {lambdaExpression.GetDebugView()}");

            var nextLevel = level + 1;
            Debug.WriteLine($"|{levelSpace}|- Parameters count: {lambdaExpression.Parameters.Count}");
            foreach (var lambdaExpressionParameter in lambdaExpression.Parameters)
                lambdaExpressionParameter.TraceNode(nextLevel);

            Debug.WriteLine("|");
            Debug.WriteLine($"|{levelSpace}|- Body [{lambdaExpression.Body.NodeType}]");
            lambdaExpression.Body?.TraceNode(nextLevel);
        }

        public static void TraceNode(this MemberExpression memberExpression, int level = 0)
        {
            var levelSpace = new string(' ', level * NumberOfSpaces);
            Debug.WriteLine($"|{levelSpace}|- memberExpression.NodeType: {memberExpression.NodeType}");
            Debug.WriteLine($"|{levelSpace}|- memberExpression.DebugView: {memberExpression.GetDebugView()}");
        }

        public static void TraceNode(this MethodCallExpression methodCallExpression, int level = 0)
        {
            var levelSpace = new string(' ', level * NumberOfSpaces);
            Debug.WriteLine($"|{levelSpace}|- methodCallExpression.NodeType: {methodCallExpression.NodeType}");
            Debug.WriteLine($"|{levelSpace}|- methodCallExpression.DebugView: {methodCallExpression.GetDebugView()}");
        }

        public static void TraceNode(this NewExpression newExpression, int level = 0)
        {
            var levelSpace = new string(' ', level * NumberOfSpaces);
            Debug.WriteLine($"|{levelSpace}|- newExpression.NodeType: {newExpression.NodeType}");
            Debug.WriteLine($"|{levelSpace}|- newExpression.DebugView: {newExpression.GetDebugView()}");
        }

        public static void TraceNode(this ParameterExpression parameterExpression, int level = 0)
        {
            var levelSpace = new string(' ', level * NumberOfSpaces);
            Debug.WriteLine($"|{levelSpace}|- parameterExpression.Name: {parameterExpression.Name ?? "null"}");
            Debug.WriteLine($"|{levelSpace}|- parameterExpression.Type: {parameterExpression.Type}");
            Debug.WriteLine($"|{levelSpace}|- parameterExpression.DebugView: {parameterExpression.GetDebugView()}");
        }
    }
}
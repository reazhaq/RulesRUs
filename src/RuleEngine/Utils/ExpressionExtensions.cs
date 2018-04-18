using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace RuleEngine.Utils
{
    public static class ExpressionExtensions
    {
        public static string GetObjectToValidateFromExpression(this Expression exp)
        {
            var lastFieldOrProperty = (string)null;

            var keepLooping = true;
            MemberExpression memberExpression = null;
            while (keepLooping)
            {
                switch (exp.NodeType)
                {
                    case ExpressionType.Lambda:
                        exp = ((LambdaExpression)exp).Body;
                        break;
                    case ExpressionType.Convert:
                        exp = ((UnaryExpression)exp).Operand;
                        break;
                    case ExpressionType.MemberAccess:
                        memberExpression = ((MemberExpression)exp);
                        lastFieldOrProperty = memberExpression.Member.Name;
                        keepLooping = false;
                        break;
                }
            }

            // look for the rest of
            var suffixPart = (string)null;
            while (memberExpression != null && memberExpression.Expression.NodeType == ExpressionType.MemberAccess)
            {
                var propInfo = memberExpression.Expression.GetType().GetProperty("Member");
                var propValue = propInfo.GetValue(memberExpression.Expression, null) as PropertyInfo;
                if (propValue != null)
                    suffixPart = string.Format($"{propValue.Name}.{suffixPart}");
                memberExpression = memberExpression.Expression as MemberExpression;
            }
            return (suffixPart != null ? string.Format($"{suffixPart}{lastFieldOrProperty}") : lastFieldOrProperty);
        }

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
            var propertyInfo = typeof(Expression).GetProperty("Expression", BindingFlags.Instance | BindingFlags.NonPublic);
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
                case BlockExpression blockExpression:
                    blockExpression.TraceNode(sb, level);
                    break;
                case ConditionalExpression conditionalExpression:
                    conditionalExpression.TraceNode(sb, level);
                    break;
                case ConstantExpression constantExpression:
                    constantExpression.TraceNode(sb, level);
                    break;
                //case DebugInfoExpression debugInfoExpression:
                //    debugInfoExpression.TraceNode(sb, level);
                //    break;
                //case DefaultExpression defaultExpression:
                //    defaultExpression.TraceNode(sb, level);
                //    break;
                case DynamicExpression dynamicExpression:
                    dynamicExpression.TraceNode(sb, level);
                    break;
                //case GotoExpression gotoExpression:
                //    gotoExpression.TraceNode(sb, level);
                //    break;
                //case IndexExpression indexExpression:
                //    indexExpression.TraceNode(sb, level);
                //    break;
                case InvocationExpression invocationExpression:
                    invocationExpression.TraceNode(sb, level);
                    break;
                //case LabelExpression labelExpression:
                //    labelExpression.TraceNode(sb, level);
                //    break;
                case LambdaExpression lambdaExpression:
                    lambdaExpression.TraceNode(sb, level);
                    break;
                //case ListInitExpression listInitExpression:
                //    listInitExpression.TraceNode(sb, level);
                //    break;
                //case LoopExpression loopExpression:
                //    loopExpression.TraceNode(sb, level);
                //    break;
                case MemberExpression memberExpression:
                    memberExpression.TraceNode(sb, level);
                    break;
                //case MemberInitExpression memberInitExpression:
                //    memberInitExpression.TraceNode(sb, level);
                //    break;
                case MethodCallExpression methodCallExpression:
                    methodCallExpression.TraceNode(sb, level);
                    break;
                //case NewArrayExpression newArrayExpression:
                //    newArrayExpression.TraceNode(sb, level);
                //    break;
                case NewExpression newExpression:
                    newExpression.TraceNode(sb, level);
                    break;
                case ParameterExpression parameterExpression:
                    parameterExpression.TraceNode(sb, level);
                    break;
                //case RuntimeVariablesExpression runtimeVariablesExpression:
                //    runtimeVariablesExpression.TraceNode(sb, level);
                //    break;
                //case TryExpression tryExpression:
                //    tryExpression.TraceNode(sb, level);
                //    break;
                //case TypeBinaryExpression typeBinaryExpression:
                //    typeBinaryExpression.TraceNode(sb, level);
                //    break;
                case UnaryExpression unaryExpression:
                    unaryExpression.TraceNode(sb, level);
                    break;
                case Expression expression2:
                    var levelSpace = new string(' ', level * NumberOfSpaces);
                    sb.Append($"|{levelSpace}|- Default Trace:{Nl}");
                    sb.Append($"|{levelSpace}|- Expression.GetType(): {expression.GetType().Name}{Nl}");
                    sb.Append($"|{levelSpace}|- Expression.Type: {expression.Type}{Nl}");
                    sb.Append($"|{levelSpace}|- Expression.NodeType: {expression2.NodeType}{Nl}");
                    sb.Append($"|{levelSpace}|- Expression.DebugView: {expression2.GetDebugView()}{Nl}");
                    break;
            }
        }

        private static void TraceBaseInfo(this Expression expression, StringBuilder sb, int level = 0)
        {
            if (sb == null || expression == null) return;
            var levelSpace = new string(' ', level * NumberOfSpaces);
            sb.Append($"|{levelSpace}|- {expression.GetType()}.NodeType: {expression.NodeType}{Nl}");
            sb.Append($"|{levelSpace}|- {expression.GetType()}.Type: {expression.Type}{Nl}");
            sb.Append($"|{levelSpace}|- {expression.GetType()}.DebugView: {expression.GetDebugView()}{Nl}");
            sb.Append($"|{levelSpace}|- {expression.GetType()}.CanReduce: {expression.CanReduce}{Nl}");
            if (expression.CanReduce)
                expression.Reduce().TraceNode(sb, level + 1);
        }

        public static void TraceNode(this BinaryExpression binaryExpression, StringBuilder sb, int level = 0)
        {
            if (sb == null || binaryExpression == null) return;
            binaryExpression.TraceBaseInfo(sb, level);
            var levelSpace = new string(' ', level * NumberOfSpaces);

            level++;
            sb.Append($"|{Nl}");
            sb.Append($"|{levelSpace}|- Left:{Nl}");
            binaryExpression.Left?.TraceNode(sb, level);

            sb.Append($"|{Nl}");
            sb.Append($"|{levelSpace}|- Right:{Nl}");
            binaryExpression.Right?.TraceNode(sb, level);
        }

        public static void TraceNode(this BlockExpression blockExpression, StringBuilder sb, int level = 0)
        {
            if (sb == null || blockExpression == null) return;
            blockExpression.TraceBaseInfo(sb, level);
            var levelSpace = new string(' ', level * NumberOfSpaces);

            level++;
            if(blockExpression.Variables !=null)
            {
                sb.Append($"|{Nl}");
                sb.Append($"|{levelSpace}|- Variables count: {blockExpression.Variables.Count}{Nl}");
                foreach (var blockExpressionVariable in blockExpression.Variables)
                    blockExpressionVariable.TraceNode(sb, level);
            }

            if(blockExpression.Expressions !=null)
            {
                sb.Append($"|{Nl}");
                sb.Append($"|{levelSpace}|- Expressions count: {blockExpression.Expressions.Count}{Nl}");
                foreach (var blockExpressionExpression in blockExpression.Expressions)
                    blockExpressionExpression.TraceNode(sb, level);
            }
        }

        public static void TraceNode(this ConditionalExpression conditionalExpression, StringBuilder sb, int level = 0)
        {
            if (sb == null || conditionalExpression == null) return;
            conditionalExpression.TraceBaseInfo(sb, level);
            var levelSpace = new string(' ', level * NumberOfSpaces);

            level++;
            sb.Append($"|{Nl}");
            sb.Append($"|{levelSpace}|- IfTrue:{Nl}");
            conditionalExpression.IfTrue.TraceNode(sb, level);

            sb.Append($"|{Nl}");
            sb.Append($"|{levelSpace}|- IfFalse:{Nl}");
            conditionalExpression.IfFalse.TraceNode(sb, level);
        }

        public static void TraceNode(this ConstantExpression constantExpression, StringBuilder sb, int level = 0)
        {
            if (sb == null || constantExpression == null) return;
            constantExpression.TraceBaseInfo(sb, level);
            //var levelSpace = new string(' ', level * NumberOfSpaces);
        }

        public static void TraceNode(this DynamicExpression dynamicExpression, StringBuilder sb, int level = 0)
        {
            if (sb == null || dynamicExpression == null) return;
            dynamicExpression.TraceBaseInfo(sb, level);
            //var levelSpace = new string(' ', level * NumberOfSpaces);
        }

        public static void TraceNode(this InvocationExpression invocationExpression, StringBuilder sb, int level = 0)
        {
            if (sb == null || invocationExpression == null) return;
            invocationExpression.TraceBaseInfo(sb, level);
            var levelSpace = new string(' ', level * NumberOfSpaces);
            level++;
            sb.Append($"|{Nl}");
            sb.Append($"|{levelSpace}|- Expression:{Nl}");
            invocationExpression.Expression?.TraceNode(sb, level);

            if(invocationExpression.Arguments != null)
            {
                sb.Append($"|{Nl}");
                sb.Append($"|{levelSpace}|- Arguments count: {invocationExpression.Arguments.Count}{Nl}");
                foreach (var invocationExpressionArgument in invocationExpression.Arguments)
                    invocationExpressionArgument.TraceNode(sb, level);
            }
        }

        public static void TraceNode(this LambdaExpression lambdaExpression, StringBuilder sb, int level = 0)
        {
            if (sb == null || lambdaExpression == null) return;
            lambdaExpression.TraceBaseInfo(sb, level);
            var levelSpace = new string(' ', level * NumberOfSpaces);

            level++;
            if(lambdaExpression.Parameters !=null)
            {
                sb.Append($"|{Nl}");
                sb.Append($"|{levelSpace}|- Parameters count: {lambdaExpression.Parameters.Count}{Nl}");
                foreach (var lambdaExpressionParameter in lambdaExpression.Parameters)
                    lambdaExpressionParameter.TraceNode(sb, level);
            }

            sb.Append($"|{Nl}");
            sb.Append($"|{levelSpace}|- Body [{lambdaExpression.Body?.NodeType}]{Nl}");
            lambdaExpression.Body?.TraceNode(sb, level);
        }

        public static void TraceNode(this MemberExpression memberExpression, StringBuilder sb, int level = 0)
        {
            if (sb == null || memberExpression == null) return;
            memberExpression.TraceBaseInfo(sb, level);
            var levelSpace = new string(' ', level * NumberOfSpaces);

            sb.Append($"|{levelSpace}|- memberExpression.Member - field: {(memberExpression.Member as FieldInfo)?.ToString()}{Nl}");
            sb.Append($"|{levelSpace}|- memberExpression.Member - prop: {(memberExpression.Member as PropertyInfo)?.ToString()}{Nl}");
        }

        public static void TraceNode(this MethodCallExpression methodCallExpression, StringBuilder sb, int level = 0)
        {
            if (sb == null || methodCallExpression == null) return;
            methodCallExpression.TraceBaseInfo(sb, level);
            var levelSpace = new string(' ', level * NumberOfSpaces);

            level++;
            if(methodCallExpression.Arguments != null)
            {
                sb.Append($"|{Nl}");
                sb.Append($"|{levelSpace}|- Arguments Count: {methodCallExpression.Arguments.Count}{Nl}");
                foreach (var expression in methodCallExpression.Arguments)
                    expression.TraceNode(sb, level);
            }
        }

        public static void TraceNode(this NewExpression newExpression, StringBuilder sb, int level = 0)
        {
            if (sb == null || newExpression == null) return;
            newExpression.TraceBaseInfo(sb, level);
            var levelSpace = new string(' ', level * NumberOfSpaces);

            level++;
            if (newExpression.Arguments != null)
            {
                sb.Append($"|{Nl}");
                sb.Append($"|{levelSpace}|- Arguments Count: {newExpression.Arguments.Count}{Nl}");
                foreach (var expression in newExpression.Arguments)
                    expression.TraceNode(sb, level);
            }

            if (newExpression.Members != null)
            {
                sb.Append($"|{Nl}");
                sb.Append($"|{levelSpace}|- Members Count: {newExpression.Members.Count}{Nl}");
                foreach (var memberInfo in newExpression.Members)
                    sb.Append($"|{levelSpace}|- memberInfo.Name: {memberInfo.Name}");
            }
        }

        public static void TraceNode(this ParameterExpression parameterExpression, StringBuilder sb, int level = 0)
        {
            if (sb == null || parameterExpression == null) return;
            parameterExpression.TraceBaseInfo(sb, level);
            var levelSpace = new string(' ', level * NumberOfSpaces);

            sb.Append($"|{levelSpace}|- parameterExpression.Name: {parameterExpression.Name ?? "null"}{Nl}");
        }

        public static void TraceNode(this UnaryExpression unaryExpression, StringBuilder sb, int level = 0)
        {
            if (sb == null || unaryExpression == null) return;
            unaryExpression.TraceBaseInfo(sb, level);
            var levelSpace = new string(' ', level * NumberOfSpaces);

            sb.Append($"|{levelSpace}|- unaryExpression.Method: {unaryExpression.Method}{Nl}");
            sb.Append($"|{levelSpace}|- unaryExpression.IsLifted: {unaryExpression.IsLifted}{Nl}");
            sb.Append($"|{levelSpace}|- unaryExpression.IsLiftedToNull: {unaryExpression.IsLiftedToNull}{Nl}");

            level++;
            sb.Append($"|{Nl}");
            sb.Append($"|{levelSpace}|- unaryExpression.Operand:{Nl}");
            unaryExpression.Operand?.TraceNode(sb, level);

            sb.Append($"|{Nl}");
            sb.Append($"|{levelSpace}|- unaryExpression.Operand.GetInnerExpression():{Nl}");
            unaryExpression.Operand?.GetInnerExpression()?.TraceNode(sb, level);
        }
    }
}
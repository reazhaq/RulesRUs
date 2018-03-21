using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using RuleEngine.Rules;
using RuleEngine.Utils;

namespace RuleFactory.RulesFactory
{
    public static class ValidationRulesFactory
    {
        public static ValidationRule<T> CreateValidationRule<T>(Expression<Func<T, object>> objectToValidate)
        {
            //Debug.WriteLine($"------ {objectToValidate}");
            //var sb = new StringBuilder();
            //objectToValidate.TraceNode(sb);
            //Debug.WriteLine($"{sb.ToString()}");

            //Debug.WriteLine("---------------------------------");
            //sb.Clear();
            //var body = objectToValidate.Body;
            //body.TraceNode(sb);
            //Debug.WriteLine($"{sb.ToString()}");

            //////var foo = ((MemberExpression)body).Member.Name;
            //////Debug.WriteLine($"foo: {foo}");

            ////var memExp = GetMemberInfo(objectToValidate);
            ////Debug.WriteLine("---------------------------------");
            ////sb.Clear();
            ////memExp.TraceNode(sb);
            ////Debug.WriteLine($"memExp: {sb.ToString()}");

            ////var mem = memExp.Member;
            ////Debug.WriteLine("---------------------------------");
            ////Debug.WriteLine($"mem: {mem.ToString()}");

            ////var foo2 = mem.Name;
            ////Debug.WriteLine($"foo2: {foo2}");

            var body = ((UnaryExpression) objectToValidate.Body);
            MemberExpression memberExpression = (MemberExpression)body.Operand;
            MemberExpression memberExpressionOrg = memberExpression;

            string Path = "";
            while (memberExpression.Expression.NodeType == ExpressionType.MemberAccess)
            {
                var propInfo = memberExpression.Expression
                    .GetType().GetProperty("Member");
                var propValue = propInfo.GetValue(memberExpression.Expression, null) 
                    as PropertyInfo;
                Path = propValue.Name + "." + Path;

                memberExpression = memberExpression.Expression as MemberExpression;
            }

            Debug.WriteLine($"{Path+ memberExpressionOrg.Member.Name}");

            return null;
        }

        //////public static string GetPropertyName(this LambdaExpression expression)
        //////{
        //////    if (expression.Body is UnaryExpression)
        //////    {
        //////        UnaryExpression unex = (UnaryExpression)expression.Body;
        //////        if (unex.NodeType == ExpressionType.Convert)
        //////        {
        //////            Expression ex = unex.Operand;
        //////            MemberExpression mex = (MemberExpression)ex;
        //////            return mex.Member.Name;
        //////        }
        //////    }
        //////}

        //private static MemberExpression GetMemberInfo(Expression method)
        //{
        //    var sb = new StringBuilder();
        //    if (!(method is LambdaExpression lambda))
        //        throw new ArgumentNullException("method");

        //    MemberExpression memberExpr = null;

        //    if (lambda.Body.NodeType == ExpressionType.Convert)
        //    {
        //        var body = ((UnaryExpression) lambda.Body);
        //        sb.Clear();
        //        body.TraceNode(sb);
        //        Debug.WriteLine($"((UnaryExpression) lambda.Body):{Environment.NewLine}{sb.ToString()}");

        //        var operand = body.Operand;
        //        sb.Clear();
        //        (operand as Expression).TraceNode(sb);
        //        Debug.WriteLine($"(operand as Expression):{Environment.NewLine}{sb.ToString()}");

        //        memberExpr = body.Operand as MemberExpression;
        //        sb.Clear();
        //        memberExpr.TraceNode(sb);
        //        Debug.WriteLine($"memberExpr:{Environment.NewLine}{sb.ToString()}");
        //    }
        //    else if (lambda.Body.NodeType == ExpressionType.MemberAccess)
        //    {
        //        memberExpr = lambda.Body as MemberExpression;
        //    }

        //    if (memberExpr == null)
        //        throw new ArgumentException("method");

        //    return memberExpr;
        //}
        ////public static PropertyInfo GetPropertyInfo<TSource, TProperty>(
        ////    TSource source,
        ////    Expression<Func<TSource, TProperty>> propertyLambda)
        ////{
        ////    Type type = typeof(TSource);

        ////    MemberExpression member = propertyLambda.Body as MemberExpression;
        ////    if (member == null)
        ////        throw new ArgumentException(string.Format(
        ////            "Expression '{0}' refers to a method, not a property.",
        ////            propertyLambda.ToString()));

        ////    PropertyInfo propInfo = member.Member as PropertyInfo;
        ////    if (propInfo == null)
        ////        throw new ArgumentException(string.Format(
        ////            "Expression '{0}' refers to a field, not a property.",
        ////            propertyLambda.ToString()));

        ////    if (type != propInfo.ReflectedType &&
        ////        !type.IsSubclassOf(propInfo.ReflectedType))
        ////        throw new ArgumentException(string.Format(
        ////            "Expresion '{0}' refers to a property that is not from type {1}.",
        ////            propertyLambda.ToString(),
        ////            type));

        ////    return propInfo;
        ////}
    }
}
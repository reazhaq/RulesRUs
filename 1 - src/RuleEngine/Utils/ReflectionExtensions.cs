using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace RuleEngine.Utils
{
    public static class ReflectionExtensions
    {
        public static MethodInfo GetMethodInfo(this Type type, string methodName, IReadOnlyCollection<Type> parameters)
        {
            if (string.IsNullOrEmpty(methodName))
                throw new ArgumentNullException($"{nameof(methodName)} can't be null/empty");

            foreach (var methodInfo in type.GetMethods().Where(m => m.Name.Equals(methodName)))
            {
                var isGenericMethod = methodInfo.IsGenericMethod;
                var genericArguments = methodInfo.GetGenericArguments();

                var parametersForTheMethod = isGenericMethod
                    ? methodInfo.MakeGenericMethod(genericArguments).GetParameters()
                    : methodInfo.GetParameters();
                var parameterTypes = parametersForTheMethod.Select(pi => pi.ParameterType).ToList();

                var isExtensionMethod = methodInfo.IsDefined(typeof(ExtensionAttribute), false) &&
                                        methodInfo.IsStatic && parameterTypes.Count > 0;
                if (isExtensionMethod)
                    parameterTypes = parameterTypes.Skip(1).ToList();

                if (parameterTypes.Count == (parameters?.Count ?? 0) && (parameters == null || parameterTypes.SequenceEqual(parameters)))
                    return methodInfo;
            }

            return null;
        }
    }
}
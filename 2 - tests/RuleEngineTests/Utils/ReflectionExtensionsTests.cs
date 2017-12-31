using System;
using System.Collections.Generic;
using FluentAssertions;
using RuleEngine.Utils;
using Xunit;

namespace RuleEngineTests.Utils
{
    public static class SomeStringExtension
    {
        public static string SomeExtensionMethod(this string something, string somethingElse, int someNum)
        {
            return string.Format($"{somethingElse}+{someNum}+{something}");
        }
    }

    public class ReflectionExtensionsTests
    {
        [Fact]
        public void GetNativeMethodInfoFromMethodName()
        {
            var type = Type.GetType("System.String");
            var mi = type.GetMethodInfo("ToUpper", null);
            mi.Should().NotBeNull();

            var someString = Activator.CreateInstance(type, new[] { 's', 'o', 'm', 'e', 'S', 't', 'r', 'i', 'n', 'g' });
            var length = mi.Invoke(someString, null);
            length.Should().Be("SOMESTRING");
        }

        [Fact]
        public void GetExtensionMethodInfoFromMethodName()
        {
            var type = typeof(SomeStringExtension);
            var parameters = new[] { typeof(string), typeof(int) };
            var mi = type.GetMethodInfo("SomeExtensionMethod", parameters);
            mi.Should().NotBeNull();

            var someExtensionMethodResult = mi.Invoke(null, new object[] { "blah", "blah", 2 });
            someExtensionMethodResult.Should().Be("blah+2+blah");
        }

        [Fact]
        public void GetExtensionMethodInfoFromGenericTypeUsingMethodName()
        {
            var list = new List<string> { "one", "two", "three" };
            var type = typeof(ListExtensions);
            var parameters = new Type[] { typeof(string), typeof(IEqualityComparer<string>) };

            var mi = type.GetMethodInfo("ContainsValue", parameters, new[] { typeof(string) });
            var result = mi.Invoke(null, new object[] {list, "one", StringComparer.OrdinalIgnoreCase});
            result.Should().BeOfType<bool>().And.Be(true);
        }
    }
}


using System;
using System.Collections.Generic;
using FluentAssertions;
using RuleEngine.Utils;
using SampleModel;
using Xunit;
using Xunit.Abstractions;

namespace RuleEngine.Tests.Utils
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
        private readonly ITestOutputHelper _testOutputHelper;

        public ReflectionExtensionsTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void GetNativeMethodInfoFromMethodName()
        {
            var type = ReflectionExtensions.GetTypeFor("System.String");
            var mi = type.GetMethodInfo("ToUpper");
            mi.Should().NotBeNull();

            var someString = Activator.CreateInstance(type, new[] { 's', 'o', 'm', 'e', '-', 'S', 't', 'r', 'i', 'n', 'g' });
            var length = mi.Invoke(someString, null);
            length.Should().Be("SOME-STRING");
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
            var parameters = new[] { typeof(string), typeof(IEqualityComparer<string>) };

            var mi = type.GetMethodInfo("ContainsValue", parameters, new[] { typeof(string) });
            var result = mi.Invoke(null, new object[] {list, "one", StringComparer.OrdinalIgnoreCase});
            result.Should().BeOfType<bool>().And.Be(true);
        }

        [Fact]
        public void GetExtensionMethodInfoForGenericTypeWithoutParameterTypesReturnNull()
        {
            var type = typeof(ListExtensions);

            var expectedException = Assert.Throws<ArgumentNullException>(() => type.GetMethodInfo("ContainsValue"));
            expectedException.Should().BeOfType<ArgumentNullException>();
            expectedException.Source.Should().Be("RuleEngine");
            _testOutputHelper.WriteLine($"expectedException.Source: {expectedException.Source}");
            _testOutputHelper.WriteLine($"expectedException.Message: {expectedException.Message}");
        }

        [Fact]
        public void GetMethodInfoWithNoParam()
        {
            var type = typeof(Game);
            var flipActiveMethodInfo = type.GetMethodInfo("FlipActive");
            var game = new Game {Active = false};
            flipActiveMethodInfo.Invoke(game, new object[] { });
            game.Active.Should().BeTrue();
        }
    }
}


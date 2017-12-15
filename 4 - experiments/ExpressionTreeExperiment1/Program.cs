using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace ExpressionTreeExperiment1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //Test1();
            //Test2();
            Test3();
        }

        private static void Test3()
        {
            Expression<Func<int>> someDefault = () => 5;
            Expression<Func<int, Func<int>>> boo = i => someDefault.Compile();
            boo.TraceNode(0);
            var booCompiled = boo.Compile();
            var blah = booCompiled(99);
            Debug.WriteLine(blah());

            Expression<Func<string, int>> whatever = s => someDefault.Compile()();
            var whateverCompiled = whatever.Compile();
            Debug.WriteLine(whateverCompiled("something"));
        }

        private static void Test2()
        {
            Expression<Func<int>> someDefault = () => 5;
            Expression<Func<int, Expression<Func<int>>>> boo = i => someDefault;
            boo.TraceNode(0);
            var booCompiled = boo.Compile();
            var blah = booCompiled(99);
            Debug.WriteLine(blah);
            Debug.WriteLine(blah.Compile()());
        }

        private static void Test1()
        {
            Func<int, bool> func1 = x => x > 5;
            Expression<Func<int, bool>> expFunc1 = x => x > 5;

            Debug.WriteLine($"func1 = {func1}");
            Debug.WriteLine($"func1(5) = {func1(5)}");
            Debug.WriteLine($"func1(3) = {func1(3)}");
            Debug.WriteLine($"func1(7) = {func1(7)}");

            Debug.WriteLine($"expFunc1 = {expFunc1}");
            var expFunc1Compiled = expFunc1.Compile();
            Debug.WriteLine($"expFunc1.Compile = expFunc1Compiled = {expFunc1Compiled}");
            Debug.WriteLine($"expFunc1Compiled(5) = {expFunc1Compiled(5)}");
            Debug.WriteLine($"expFunc1Compiled(3) = {expFunc1Compiled(3)}");
            Debug.WriteLine($"expFunc1Compiled(7) = {expFunc1Compiled(7)}");

            expFunc1.TraceNode(0);
        }
    }

    static class ExtensionMethods
    {
        public static void TraceNode(this LambdaExpression lambdaExpression, int level)
        {
            var levelSpace = new string(' ', level * 2);
            Debug.WriteLine($"|{levelSpace}|- {lambdaExpression.NodeType}");
            Debug.WriteLine($"|{levelSpace}|- Parameters");
            foreach (var lambdaExpressionParameter in lambdaExpression.Parameters)
            {
                lambdaExpressionParameter.TraceNode(level+1);
            }
            Debug.WriteLine("|");
            Debug.WriteLine($"|{levelSpace}|- Body [{lambdaExpression.Body.NodeType}]");
            (lambdaExpression.Body as BinaryExpression)?.TraceNode(level+1);
        }

        public static void TraceNode(this ParameterExpression parameterExpression, int level)
        {
            var levelSpace = new string(' ', level * 2);
            Debug.WriteLine($"|{levelSpace}|- {parameterExpression.Name} ({parameterExpression.Type})");
        }

        public static void TraceNode(this BinaryExpression binaryExpression, int level)
        {
            var levelSpace = new string(' ', level * 2);
            Debug.WriteLine($"|{levelSpace}|- {binaryExpression.ToString()} ({binaryExpression.Type})");
            (binaryExpression.Left as ParameterExpression)?.TraceNode(level + 1);
            (binaryExpression.Right as ConstantExpression)?.TraceNode(level + 1);
        }

        public static void TraceNode(this ConstantExpression constantExpression, int level)
        {
            var levelSpace = new string(' ', level * 2);
            Debug.WriteLine($"|{levelSpace}|- {constantExpression.Value} ({constantExpression.Type})");
        }
    }
}

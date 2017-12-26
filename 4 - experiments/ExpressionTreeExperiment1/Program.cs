using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using RuleEngine.Utils;
using System.Linq;

namespace ExpressionTreeExperiment1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //Test1();
            //Test2();
            //Test3();
            //Test4();
            //Test5();

            Test6();
        }

        private static void Test6()
        {
            var moo = 5;
            var foo = "blah";
            Expression<Func<string>> blah = () => (moo == 6 ? foo : "boo");
            blah.TraceNode();


            var names = new List<string> {"one", "two", "three", "four"};
            if (names.ContainsValue("One", null))
                Debug.WriteLine("one");

            if (names.ContainsValue("One", StringComparer.OrdinalIgnoreCase))
                Debug.WriteLine("one.1");

            Expression<Func<IList<string>, string, bool>> findExpression = (someList, value) => someList.ContainsValue(value, StringComparer.OrdinalIgnoreCase);
            Debug.WriteLine(findExpression);
            findExpression.TraceNode();

            var foo3 = findExpression.Compile()(names, "three");

            var foo2 = names.ContainsValue("three", StringComparer.OrdinalIgnoreCase);

            Expression<Func<IList<int>, int, bool>> findIntExpression = (intList, value) => intList.ContainsValue(value, EqualityComparer<int>.Default);
            Debug.WriteLine(findIntExpression);
            findIntExpression.TraceNode();

            // T ValueToLookup
            // List<T> ValueList
            // IEqualityComparer<T> compararToUse
            // bool ContainsValue(T)

        }

        private static void Test5()
        {
            var foo = "somestring";
            var blah = foo.Split('.');
            foreach (var s in blah)
            {
                Debug.WriteLine(s);
            }

            foo = "something.blah";
            blah = foo.Split('.');
            foreach (var s in blah)
            {
                Debug.WriteLine(s);
            }
        }

        private static void Test4()
        {
            Expression<Func<int>> blah = () => 99;
            blah.TraceNode(0);
            var foo = blah.Compile();
            foo();
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

}

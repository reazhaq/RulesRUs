using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using RuleEngine.Utils;
using System.Linq;
using System.Reflection;
using RuleEngine.Rules;

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

            //Test6();
            //Test7();
            Test8();
        }

        private static void Test8()
        {
            Expression<Action<string>> blah = (string x) => Debug.WriteLine(x);
            blah.TraceNode();

            // Add the following directive to your file:
            // using System.Linq.Expressions;  

            // To demonstrate the assignment operation, we create a variable.
            ParameterExpression variableExpr = Expression.Variable(typeof(String), "sampleVar");

            // This expression represents the assignment of a value
            // to a variable expression.
            // It copies a value for value types, and
            // copies a reference for reference types.
            Expression assignExpr = Expression.Assign(
                variableExpr,
                Expression.Constant("Hello World!")
            );

            // The block expression allows for executing several expressions sequentually.
            // In this block, we pass the variable expression as a parameter,
            // and then assign this parameter a value in the assign expression.
            Expression blockExpr = Expression.Block(
                new ParameterExpression[] { variableExpr },
                assignExpr
            );

            // Print out the assign expression.
            Console.WriteLine(assignExpr.ToString());
            assignExpr.TraceNode();

            // The following statement first creates an expression tree,
            // then compiles it, and then executes it.  
            Console.WriteLine(Expression.Lambda<Func<String>>(blockExpr).Compile()());

            // This code example produces the following output:
            //
            // (sampleVar = "Hello World!")
            // Hello World!        
        }

        private static void Test7()
        {
            Expression<Func<string, string>> Blah = x => "blah";
            Blah.TraceNode();
            Debug.WriteLine(Blah.Compile()("moo"));

            Expression<Func<int, string>> blah2 = i => "blah2";
            blah2.TraceNode();
            Debug.WriteLine(blah2.Compile()(55));
        }

        private static void Test6()
        {
            var names = new List<string> { "one", "two", "three", "four" };


            Expression<Func<string, bool>> findValueExpression =
                s => names.Contains(s, StringComparer.OrdinalIgnoreCase);
            findValueExpression.TraceNode();
            var moo = findValueExpression.Compile();
            var mooo = moo("one");



            var foo = (IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase;
            Expression<Func<string, bool>> findExpression = s => names.Contains(s, foo);
            findExpression.TraceNode();
            var moo2 = findExpression.Compile();


            var comparerType = Type.GetType("System.StringComparer");
            var ignoreCase = comparerType.GetProperty("OrdinalIgnoreCase");

            Debug.WriteLine(foo);
            Debug.WriteLine(ignoreCase);

            //            //var moo = 5;
            //            //var foo = "blah";
            //            //Expression<Func<string>> blah = () => (moo == 6 ? foo : "boo");
            //            //blah.TraceNode();


            //            //if (names.ContainsValue("One", null))
            //            //    Debug.WriteLine("one");

            //            //if (names.ContainsValue("One", StringComparer.OrdinalIgnoreCase))
            //            //    Debug.WriteLine("one.1");

            //            //Expression<Func<IList<string>, string, bool>> findExpression = (someList, value) => someList.ContainsValue(value, StringComparer.OrdinalIgnoreCase);
            //            //Debug.WriteLine(findExpression);
            //            //findExpression.TraceNode();

            //            //var foo3 = findExpression.Compile()(names, "three");

            //            //var foo2 = names.ContainsValue("three", StringComparer.OrdinalIgnoreCase);

            //            //Expression<Func<IList<int>, int, bool>> findIntExpression = (intList, value) => intList.ContainsValue(value, EqualityComparer<int>.Default);
            //            //Debug.WriteLine(findIntExpression);
            //            //findIntExpression.TraceNode();

            //            //// T ValueToLookup
            //            //// List<T> ValueList
            //            //// IEqualityComparer<T> compararToUse
            //            //// bool ContainsValue(T)

            //            //var t = Type.GetType("StringComparer");
            //            //var b = t.GetProperty("OrdinalIgnoreCase");
            //            var t = typeof(StringComparer);
            //            var p = t.GetRuntimeProperty("OrdinalIgnoreCase");

            ////            var comparer = "StringComparer.OrdinalIgnoreCase";
            ////            var comparerParts = comparer.Split('.');
            ////            var type = Type.GetType(comparerParts[0]);
            //////            var c = type.GetRuntimeProperty(comparerParts[1]);

            ////            //var pc = p == c;

            //            var blah2 = Type.GetType("System.StringComparer");
            //            var blah22 = blah2.GetRuntimeProperty("OrdinalIgnoreCase");
            //            var mooo = p == blah22;

            //            ////Expression blah = Expression.Parameter(typeof(StringComparer));
            //            ////Debug.WriteLine(blah);
            //            ////blah.TraceNode();

            //            Expression<Func<string, bool>> findValueExpression =
            //                s => names.Contains(s, StringComparer.OrdinalIgnoreCase);
            //            //Debug.WriteLine(findValueExpression);
            //            //findValueExpression.TraceNode();
            //            //var foo4 = findValueExpression.Compile()("Four");

            //            var methodCallRule = new MethodCallRule<IList<string>, bool> {MethodToCall = "ContainsValue", MethodClassName = "RuleEngine.Utils.ListExtensions", Inputs=
            //            {
            //                "Four", StringComparer.OrdinalIgnoreCase
            //            }};
            //            var a = methodCallRule.Compile();
            //            var b = methodCallRule.Execute(names);
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

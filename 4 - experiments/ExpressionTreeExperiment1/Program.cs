using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;

namespace ExpressionTreeExperiment1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

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
        }
    }
}

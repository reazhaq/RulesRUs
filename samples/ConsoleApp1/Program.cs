using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using RuleEngine.Rules;
using RuleFactory;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var file = "order-rule.json";
            var foo = new SampleModel.Order();

            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new JsonConverterForRule());

            if (File.Exists(file))
            {
                using var stream = File.OpenText(file);
                var line = stream.ReadLine();
                while (!string.IsNullOrWhiteSpace(line))
                {
                    Debug.WriteLine(line);
                    var foo2 = JsonSerializer.Deserialize<Rule>(line, serializeOptions);
                    line = stream.ReadLine();
                }
            }

        }
    }
}

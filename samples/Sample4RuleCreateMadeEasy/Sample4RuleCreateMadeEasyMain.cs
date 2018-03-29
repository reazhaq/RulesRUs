using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using RuleEngine.Rules;
using RuleFactory;
using RuleFactory.RulesFactory;
using Sample4RuleCreateMadeEasy.Model;

namespace Sample4RuleCreateMadeEasy
{
    class Sample4RuleCreateMadeEasyMain
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Sample4RuleCreateMadeEasy - simple tutorial - quick start how-to");

            //var orderCustomerAndProductCannotBeNullRule = new ValidationRule<Order>
            //{
            //    OperatorToUse = "AndAlso",
            //    RuleError = new RuleError { Code = "c2", Message = "Customer and/or Product can't be null"},
            //    ChildrenRules =
            //    {
            //        new ValidationRule<Order>
            //        {
            //            OperatorToUse = "NotEqual",
            //            ObjectToValidate = "Customer",
            //            ValueToValidateAgainst = new ConstantRule<Customer>{Value = "null"}
            //        },
            //        new ValidationRule<Order>
            //        {
            //            OperatorToUse = "NotEqual",
            //            ObjectToValidate = "Product",
            //            ValueToValidateAgainst = new ConstantRule<Product>{Value = "null"}}
            //    }
            //};

            var nullCustomer = ConstantRulesFactory.CreateConstantRule<Customer>("null");
            var nullProduct = ConstantRulesFactory.CreateConstantRule<Product>("null");

            var child1Rule = ValidationRulesFactory.CreateValidationRule<Order>(o => o.Customer,
                LogicalOperatorAtTheRootLevel.NotEqual, nullCustomer);

            var child2Rule = ValidationRulesFactory.CreateValidationRule<Order>(o => o.Product,
                LogicalOperatorAtTheRootLevel.NotEqual, nullProduct);

            var orderCustomerAndProductCannotBeNullRule =
                ValidationRulesFactory.CreateValidationRule<Order>(ChildrenBindingOperator.AndAlso,
                    new List<Rule> {child1Rule, child2Rule});
            orderCustomerAndProductCannotBeNullRule.RuleError = new RuleError
            {
                Code = "c2", Message = "Customer and/or Product can't be null"
            };

            var compiledResult = orderCustomerAndProductCannotBeNullRule.Compile();
            Debug.WriteLine($"compiledResult: {compiledResult}"); // true

            var order = new Order();
            var ruleResult = orderCustomerAndProductCannotBeNullRule.IsValid(order);
            Debug.WriteLine($"ruleResult: {ruleResult}"); // false

            // add a customer object
            order.Customer = new Customer();
            ruleResult = orderCustomerAndProductCannotBeNullRule.IsValid(order);
            Debug.WriteLine($"ruleResult: {ruleResult}"); // false - because product is null

            // add a product object
            order.Product = new Product();
            ruleResult = orderCustomerAndProductCannotBeNullRule.IsValid(order);
            Debug.WriteLine($"ruleResult: {ruleResult}"); // true - because both are non-null

            var converter = new CustomRuleJsonConverter();
            var jsonDoc = JsonConvert.SerializeObject(orderCustomerAndProductCannotBeNullRule, Formatting.Indented, converter);
            Debug.WriteLine(jsonDoc);

            // this shall throw and null exception
            //orderCustomerAndProductCannotBeNullRule.IsValid(null);
        }
    }
}

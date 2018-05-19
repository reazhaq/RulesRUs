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
            var isValidOrder = orderCustomerAndProductCannotBeNullRule.IsValid(order);
            if(!isValidOrder)
                Debug.WriteLine($"orderCustomerAndProductCannotBeNullRule not valid: " +
                                $"{orderCustomerAndProductCannotBeNullRule.RuleError}");
            

            // add a customer object
            order.Customer = new Customer();
            isValidOrder = orderCustomerAndProductCannotBeNullRule.IsValid(order);
            if(!isValidOrder)
                Debug.WriteLine($"orderCustomerAndProductCannotBeNullRule not valid: " +
                                $"{orderCustomerAndProductCannotBeNullRule.RuleError}");

            // add a product object
            order.Product = new Product();
            isValidOrder = orderCustomerAndProductCannotBeNullRule.IsValid(order);
            if(!isValidOrder)
                Debug.WriteLine($"orderCustomerAndProductCannotBeNullRule not valid: " +
                                $"{orderCustomerAndProductCannotBeNullRule.RuleError}");

            var converter = new JsonConverterForRule();
            var jsonDoc = JsonConvert.SerializeObject(orderCustomerAndProductCannotBeNullRule, Formatting.Indented, converter);
            Debug.WriteLine($"orderCustomerAndProductCannotBeNullRule converted to Json:{Environment.NewLine}{jsonDoc}");

            // this shall throw a null exception
            //orderCustomerAndProductCannotBeNullRule.IsValid(null);
        }
    }
}

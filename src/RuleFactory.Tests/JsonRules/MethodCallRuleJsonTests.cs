﻿namespace RuleFactory.Tests.JsonRules;

public class MethodCallRuleJsonTests : IClassFixture<ExpressionRulesFixture>
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly Game _game1;
    private readonly Game _game2;

    public MethodCallRuleJsonTests(ExpressionRulesFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _game1 = fixture.Game1;
        _game2 = fixture.Game2;
        _testOutputHelper = testOutputHelper;
    }
    
    [Theory]
    [InlineData("Game 1", true)]
    [InlineData("game 1", true)]
    [InlineData("game 2", false)]
    [InlineData("gaMe 2", false)]
    public void CallEqualsMethodOnNameUsingConstantRuleToAndFromJson(string param1, bool expectedResult)
    {
        // call Equals method on Name string object
        // compiles to: Param_0.Name.Equals("Game 1", CurrentCultureIgnoreCase)
        var rule = new MethodCallRule<Game, bool>
        {
            ObjectToCallMethodOn = "Name",
            MethodToCall = "Equals",
            MethodParameters = { new ConstantRule<string> { Value = param1 },
                new ConstantRule<StringComparison> { Value = "CurrentCultureIgnoreCase" } }
        };

        // convert to json
        var ruleJson = JsonConvert.SerializeObject(rule, Formatting.Indented, new JsonConverterForRule());
        _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
        // re-hydrate from json
        var ruleFromJson = (MethodCallRule<Game, bool>)JsonConvert.DeserializeObject<Rule>(ruleJson, new JsonConverterForRule());
        var compileResult = ruleFromJson.Compile();
        compileResult.Should().BeTrue();
        _testOutputHelper.WriteLine($"{nameof(ruleFromJson)}:{Environment.NewLine}" +
                                    $"{ruleFromJson.ExpressionDebugView()}");

        var executeResult2 = ruleFromJson.Execute(_game1);
        executeResult2.Should().Be(expectedResult);
        executeResult2 = ruleFromJson.Execute(_game2);
        executeResult2.Should().Be(!expectedResult);
    }

    [Fact]
    public void CallAVoidMethodToAndFromJson()
    {
        // call FlipActive method on the game object
        // compiles to: Param_0.FlipActive()
        var rule = new MethodVoidCallRule<Game>
        {
            MethodToCall = "FlipActive"
        };

        // convert to json
        var ruleJson = JsonConvert.SerializeObject(rule, Formatting.Indented, new JsonConverterForRule());
        _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
        // re-hydrate from json
        var ruleFromJson = (MethodVoidCallRule<Game>)JsonConvert.DeserializeObject<Rule>(ruleJson, new JsonConverterForRule());
        var compileResult = ruleFromJson.Compile();
        compileResult.Should().BeTrue();
        _testOutputHelper.WriteLine($"{nameof(ruleFromJson)}:{Environment.NewLine}" +
                                    $"{ruleFromJson.ExpressionDebugView()}");
        var currentActiveState2 = _game1.Active;
        ruleFromJson.Execute(_game1);
        _game1.Active.Should().Be(!currentActiveState2);
    }

    [Fact]
    public void CallToUpperToAndFromJson()
    {
        var rule = new MethodCallRule<string, string>{MethodToCall = "ToUpper"};

        var converter = new JsonConverterForRule();
        // convert to json
        var ruleJson = JsonConvert.SerializeObject(rule, Formatting.Indented, converter);
        _testOutputHelper.WriteLine($"ruleJson:{Environment.NewLine}{ruleJson}");
        // re-hydrate from json
        var ruleFromJson = JsonConvert.DeserializeObject<MethodCallRule<string, string>>(ruleJson, converter);

        var compileResult = ruleFromJson.Compile();
        compileResult.Should().BeTrue();

        var foo = "foo";
        var FOO = ruleFromJson.Execute(foo);
        FOO.Should().Be("FOO");
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(1000, false)]
    public void CheckToSeeIfPlayerExistsInAGameToAndFromJson(int id, bool expectedResult)
    {
        // call HasPlayer method on the game object
        // compiles to: Param_0.HasPlayer(1000)
        var rule = new MethodCallRule<Game, bool>
        {
            MethodToCall = "HasPlayer",
            MethodParameters = { new ConstantRule<int> { Value = id.ToString() } }
        };

        // convert to json
        var ruleJson = JsonConvert.SerializeObject(rule, Formatting.Indented, new JsonConverterForRule());
        _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
        // re-hydrate from json
        var ruleFromJson = (MethodCallRule<Game, bool>)JsonConvert.DeserializeObject<Rule>(ruleJson, new JsonConverterForRule());
        var compileResult = ruleFromJson.Compile();
        compileResult.Should().BeTrue();
        _testOutputHelper.WriteLine($"{nameof(ruleFromJson)}:{Environment.NewLine}" +
                                    $"{ruleFromJson.ExpressionDebugView()}");
        var executeResult2 = ruleFromJson.Execute(_game1);
        executeResult2.Should().Be(expectedResult);
    }

    [Fact]
    public void CallAStringMethodOnDescriptionObjectToAndFromJson()
    {
        // Description is a string - Call Contains method on Description
        // compiles to: Param_0.Description.Contains("cool")
        var rule = new MethodCallRule<Game, bool>
        {
            MethodToCall = "Contains",
            ObjectToCallMethodOn = "Description",
            MethodParameters = { new ConstantRule<string> { Value = "cool" } }
        };

        // convert to json
        var ruleJson = JsonConvert.SerializeObject(rule, Formatting.Indented, new JsonConverterForRule());
        _testOutputHelper.WriteLine($"{nameof(ruleJson)}:{Environment.NewLine}{ruleJson}");
        // re-hydrate from json
        var ruleFromJson = (MethodCallRule<Game, bool>)JsonConvert.DeserializeObject<Rule>(ruleJson, new JsonConverterForRule());
        var compileResult = ruleFromJson.Compile();
        compileResult.Should().BeTrue();
        _testOutputHelper.WriteLine($"{nameof(ruleFromJson)}:{Environment.NewLine}" +
                                    $"{ruleFromJson.ExpressionDebugView()}");
        // check to see if _game1 description contains keyword "cool"
        var executeResult2 = ruleFromJson.Execute(_game1);
        executeResult2.Should().BeFalse();

        // check to see if _game2 description contains keyword "cool"
        executeResult2 = ruleFromJson.Execute(_game2);
        executeResult2.Should().BeTrue();
    }

    [Fact]
    public void CallCreateGameStaticMethodToAndFromJson()
    {
        //var game = Game.CreateGame();
        var ruleBefore = new StaticMethodCallRule<Game>
        {
            MethodClassName = "ModelForUnitTests.Game",
            MethodToCall = "CreateGame"
        };

        var jsonConverterForRule = new JsonConverterForRule();
        // convert to json
        var ruleJson = JsonConvert.SerializeObject(ruleBefore, jsonConverterForRule);
        _testOutputHelper.WriteLine($"ruleJson:{Environment.NewLine}{ruleJson}");
        // read from json
        var ruleAfter = JsonConvert.DeserializeObject<Rule>(ruleJson, jsonConverterForRule);

        var compileResult = ruleAfter.Compile();
        compileResult.Should().BeTrue();
        _testOutputHelper.WriteLine($"ruleAfter: {Environment.NewLine}" +
                                    $"{ruleAfter.ExpressionDebugView()}");

        var game = ((StaticMethodCallRule<Game>)ruleAfter).Execute();
        game.Should().NotBeNull();
    }

    [Fact]
    public void CallCreateGameStaticMethod2ToAndFromJson()
    {
        //var game = Game.CreateGame("cool game");
        var ruleBefore = new StaticMethodCallRule<Game>
        {
            MethodClassName = "ModelForUnitTests.Game",
            MethodToCall = "CreateGame",
            MethodParameters = { new ConstantRule<string> { Value = "cool game" } }
        };

        var jsonConverterForRule = new JsonConverterForRule();
        // convert to json
        var ruleJson = JsonConvert.SerializeObject(ruleBefore, jsonConverterForRule);
        _testOutputHelper.WriteLine($"ruleJson:{Environment.NewLine}{ruleJson}");
        // read from json
        var ruleAfter = JsonConvert.DeserializeObject<Rule>(ruleJson, jsonConverterForRule);

        var compileResult = ruleAfter.Compile();
        compileResult.Should().BeTrue();
        _testOutputHelper.WriteLine($"rule: {Environment.NewLine}" +
                                    $"{ruleAfter.ExpressionDebugView()}");

        var game = ((StaticMethodCallRule<Game>)ruleAfter).Execute();
        game.Should().NotBeNull();
        game.Name.Should().Be("cool game");
    }

    [Fact]
    public void CallCreateGameStaticMethod3ToAndFromJson()
    {
        //var game = Game.CreateGame("game", "description", 1, true);
        var ruleBefore = new StaticMethodCallRule<Game>
        {
            MethodClassName = "ModelForUnitTests.Game",
            MethodToCall = "CreateGame",
            MethodParameters =
            {
                new ConstantRule<string> {Value = "game"},
                new ConstantRule<string> {Value = "description"},
                new ConstantRule<int> {Value = "1"},
                new ConstantRule<bool> {Value = "true"}
            }
        };

        var jsonConverterForRule = new JsonConverterForRule();
        // convert to json
        var ruleJson = JsonConvert.SerializeObject(ruleBefore, jsonConverterForRule);
        _testOutputHelper.WriteLine($"ruleJson:{Environment.NewLine}{ruleJson}");
        // read from json
        var ruleAfter = JsonConvert.DeserializeObject<StaticMethodCallRule<Game>>(ruleJson, jsonConverterForRule);

        var compileResult = ruleAfter.Compile();
        compileResult.Should().BeTrue();
        _testOutputHelper.WriteLine($"rule: {Environment.NewLine}" +
                                    $"{ruleAfter.ExpressionDebugView()}");

        var game = ruleAfter.Execute();
        game.Should().NotBeNull();
        game.Name.Should().Be("game");
        _testOutputHelper.WriteLine($"{game}");
    }

    [Fact]
    public void CallStaticVoidMethodToAndFromJson()
    {
        var ruleBefore = MethodCallRulesFactory.CreateStaticVoidMethodCallRule("SomeVoidStaticMethod", "ModelForUnitTests.Game", null);

        var jsonConverterForRule = new JsonConverterForRule();
        // convert to json
        var ruleJson = JsonConvert.SerializeObject(ruleBefore, jsonConverterForRule);
        _testOutputHelper.WriteLine($"ruleJson:{Environment.NewLine}{ruleJson}");
        // read from json
        var ruleAfter = JsonConvert.DeserializeObject<Rule>(ruleJson, jsonConverterForRule);

        var compileResult = ruleAfter.Compile();
        compileResult.Should().BeTrue();
        _testOutputHelper.WriteLine($"rule: {Environment.NewLine}" +
                                    $"{ruleAfter.ExpressionDebugView()}");

        Game.SomeStaticIntValue = 0;
        (ruleAfter as StaticVoidMethodCallRule)?.Execute();
        Game.SomeStaticIntValue.Should().Be(1);
    }

    [Fact]
    public void CallStaticVoidMethod2ToAndFromJson()
    {
        var ruleBefore = MethodCallRulesFactory.CreateStaticVoidMethodCallRule("SomeVoidStaticMethod", "ModelForUnitTests.Game", 
            new List<Rule>{new ConstantRule<int> {Value = "99"}});

        var jsonConverterForRule = new JsonConverterForRule();
        // convert to json
        var ruleJson = JsonConvert.SerializeObject(ruleBefore, jsonConverterForRule);
        _testOutputHelper.WriteLine($"ruleJson:{Environment.NewLine}{ruleJson}");
        // read from json
        var ruleAfter = JsonConvert.DeserializeObject<Rule>(ruleJson, jsonConverterForRule);

        var compileResult = ruleAfter.Compile();
        compileResult.Should().BeTrue();
        _testOutputHelper.WriteLine($"rule: {Environment.NewLine}" +
                                    $"{ruleAfter.ExpressionDebugView()}");

        Game.SomeStaticIntValue = 0;
        (ruleAfter as StaticVoidMethodCallRule)?.Execute();
        Game.SomeStaticIntValue.Should().Be(99);
    }
}
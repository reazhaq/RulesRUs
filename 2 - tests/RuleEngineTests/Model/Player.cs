namespace RuleEngineTests.Model
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Country Country { get; set; }

        public CoOrdinate CurrentCoOrdinates { get; set; }
        public int CurrentScore { get; set; }
    }
}
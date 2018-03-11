using System.Collections.Generic;

namespace RuleFactory.Tests.Model
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Country Country { get; set; }

        public CoOrdinate CurrentCoOrdinates { get; set; }
        public int CurrentScore { get; set; }
    }

    public class PlayerCountryEqualityComparer : IEqualityComparer<Player>
    {
        public bool Equals(Player x, Player y)
        {
            if (object.ReferenceEquals(x, y)) return true;
            return x?.Country?.CountryCode == y?.Country?.CountryCode;
        }

        public int GetHashCode(Player obj)
        {
            var ccHashCode = obj.Country?.CountryCode?.GetHashCode();
            return ccHashCode.HasValue ? ccHashCode.Value : obj.Id;
        }
    }
}
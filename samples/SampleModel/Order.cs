
using System.Text.Json;

namespace SampleModel
{
    public class Order
    {
        public Customer Customer { get; set; }
        public Product Product { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions {WriteIndented = true});
        }
    }
}
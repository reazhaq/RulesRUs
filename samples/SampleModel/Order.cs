using Newtonsoft.Json;

namespace SampleModel
{
    public class Order
    {
        public Customer Customer { get; set; }
        public Product Product { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
namespace TestNinja.Mocking
{
    public class Product
    {
        public float ListPrice { get; set; }

        public float GetPrice(Customer customer) => customer.IsGold ? ListPrice * 0.7f : ListPrice;
    }

    public class Customer
    {
        public bool IsGold { get; set; }
    }
}

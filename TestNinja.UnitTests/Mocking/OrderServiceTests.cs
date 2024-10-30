using Moq;
using NUnit.Framework;
using TestNinja.Mocking;

namespace TestNinja.UnitTests.Mocking
{
    [TestFixture]
    public class OrderServiceTests
    {
        private OrderService _service;
        private Mock<IStorage> _storage;

        [SetUp]
        public void SetUp()
        {
            _storage = new Mock<IStorage>();
            _service = new OrderService(_storage.Object);
        }

        [Test]
        public void PlaceOrder_WhenCalled_ShouldStoreTheOrder()
        {
            var order = new Order();
            _service.PlaceOrder(order);

            _storage.Verify(s => s.Store(order));
        }
    }
}

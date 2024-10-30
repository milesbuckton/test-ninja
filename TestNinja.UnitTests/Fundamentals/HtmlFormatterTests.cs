using NUnit.Framework;
using TestNinja.Fundamentals;

namespace TestNinja.UnitTests.Fundamentals
{
    [TestFixture]
    public class HtmlFormatterTests
    {
        private HtmlFormatter _formatter;

        [SetUp]
        public void Setup()
        {
            _formatter = new HtmlFormatter();
        }

        [Test]
        public void FormatAsBoldShouldEncloseTheStringWithStrongElement()
        {
            var result = _formatter.FormatAsBold("abc");

            // Specific
            Assert.That(result, Is.EqualTo("<strong>abc</strong>").IgnoreCase);

            // More general
            Assert.That(result, Does.StartWith("<strong>").IgnoreCase);
            Assert.That(result, Does.EndWith("</strong>"));
            Assert.That(result, Does.Contain("abc"));
        }
    }
}

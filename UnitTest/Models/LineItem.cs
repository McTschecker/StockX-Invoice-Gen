using NUnit.Framework;
using StockX_Invoice_Gen.Sale;

namespace UnitTest.Models
{
    public class LineItemTests
    {
        [Test]
        public void testNoAdjustmentNeeded()
        {
            LineItem[] items = new LineItem[] { };
        }
    }
}
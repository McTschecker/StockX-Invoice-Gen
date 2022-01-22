using StockX_Invoice_Gen.Sale;

namespace StockX_Invoice_Gen.Exports
{
    internal abstract class Export
    {
        public abstract string Name { get; }
        public abstract string createInvoice(UnifiedSale sale);
    }
}
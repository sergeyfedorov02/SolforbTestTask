namespace DataContracts
{
    public class ReceiptDocumentItemDto
    {
        public long Id { get; set; }
        public string Number { get; set; }
        public DateOnly Date { get; set; }
        public ReceiptResourceDto ReceptItem { get; set; }
    }
}

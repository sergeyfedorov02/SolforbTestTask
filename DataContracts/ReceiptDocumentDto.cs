namespace DataContracts
{
    public class ReceiptDocumentDto
    {
        public long Id { get; set; }
        public string Number { get; set; }
        public DateOnly Date { get; set; }
        public List<ReceiptResourceDto> ReceiptResources { get; set; }
    }
}

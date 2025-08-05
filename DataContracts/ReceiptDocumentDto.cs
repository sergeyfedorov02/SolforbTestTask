namespace DataContracts
{
    public class ReceiptDocumentDto
    {
        public long Id { get; set; }
        public string Number { get; set; }
        public DateTime Date { get; set; }
        public List<ReceiptResourceDto> ReceiptResources { get; set; }
    }
}

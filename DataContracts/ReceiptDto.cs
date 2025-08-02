namespace DataContracts
{
    public class ReceiptDto
    {
        public long Id { get; set; }
        public long Number { get; set; }
        public DateTime Date { get; set; }
        public ResourceInStorageDto Resource { get; set; }
        public MeasurementInStorageDto Measurement { get; set; }
        public int Count { get; set; }
    }
}

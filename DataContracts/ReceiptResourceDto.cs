namespace DataContracts
{
    public class ReceiptResourceDto
    {
        public ResourceDto Resource { get; set; }
        public MeasurementDto Measurement { get; set; }
        public int Count { get; set; }
    }
}

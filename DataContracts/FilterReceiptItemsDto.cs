namespace DataContracts
{
    public class FilterReceiptItemsDto
    {
        public int? Skip { get; set; }
        public int? Top { get; set; }
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }

        public List<string> DocumentNumbers { get; set; }
        public List<long> ResourceIds { get; set; }
        public List<long> MeasurementIds { get; set; }
    }
}

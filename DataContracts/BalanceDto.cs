namespace DataContracts
{
    public class BalanceDto
    {
        public ResourceInBalanceDto Resource { get; set; }
        public MeasurementInBalanceDto Measurement { get; set; }
        public int Count { get; set; }
    }
}

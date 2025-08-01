namespace DataContracts
{
    public class FilterDto
    {
        public int? Skip { get; set; }
        public int? Top {  get; set; }
        public string Filter { get; set; }
        public string OrderBy {  get; set; }
    }
}

namespace DataContracts
{
    public class GridResultDto<T> where T : class
    {
        public int Count { get; set; }
        public List<T> Data { get; set; }
    }
}

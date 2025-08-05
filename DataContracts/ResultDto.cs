namespace DataContracts
{
    public sealed class ResultDto
    {
        private ResultDto()
        {

        }

        public static ResultDto CreateFromException(Exception ex)
        {
            return new ResultDto
            {
                Success = false,
                Exception = ex
            };
        }

        public static ResultDto CreateOk()
        {
            return new ResultDto
            {
                Success = true
            };
        }

        public bool Success { get; init; }
        public Exception Exception { get; init; }
    }
}

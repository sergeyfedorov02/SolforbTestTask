using DataContracts;
using Radzen;

namespace SolforbTestTask.Server.Services
{
    public interface IStorageService
    {
        Task<DataResultDto<GridResultDto<BalanceDto>>> GetBalanceAsync(Query query);

        Task<DataResultDto<GridResultDto<ReceiptDocumentItemDto>>> GetReceiptDocumentItemsAsync(FilterReceiptItemsDto query);

        Task<ResultDto> CreateReceiptDocumentAsync(ReceiptDocumentDto receiptDocumentDto);

        Task<DataResultDto<List<string>>> GetReceiptsDocumentNumbersFilterAsync();
        Task<DataResultDto<List<ResourceDto>>> GetResourcesFilterAsync();
        Task<DataResultDto<List<MeasurementDto>>> GetMeasurementsFilterAsync();
    }
}

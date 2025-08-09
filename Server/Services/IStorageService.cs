using DataContracts;
using Radzen;

namespace SolforbTestTask.Server.Services
{
    public interface IStorageService
    {
        Task<DataResultDto<GridResultDto<BalanceDto>>> GetBalanceAsync(Query query);

        Task<DataResultDto<GridResultDto<ReceiptDocumentItemDto>>> GetReceiptDocumentItemsAsync(FilterReceiptItemsDto query);

        Task<ResultDto> CreateReceiptDocumentAsync(ReceiptDocumentDto receiptDocumentDto);

        Task<DataResultDto<GridResultDto<string>>> GetReceiptsDocumentNumbersFilterAsync(Query query);
        Task<DataResultDto<GridResultDto<ResourceDto>>> GetResourcesFilterAsync(Query query);
        Task<DataResultDto<GridResultDto<MeasurementDto>>> GetMeasurementsFilterAsync(Query query);

        Task<DataResultDto<ReceiptDocumentDto>> GetWholeReceiptDocumentAsync(ReceiptDocumentItemDto receiptDocumentItemDto);

        Task<ResultDto> UpdateReceiptDocumentAsync(ReceiptDocumentDto receiptDocumentDto);
    }
}

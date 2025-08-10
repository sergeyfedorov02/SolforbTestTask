 using DataContracts;
using Radzen;

namespace SolforbTestTask.Client.Services
{
    public interface IStorageService
    {
        Task<DataResultDto<GridResultDto<BalanceDto>>> GetBalanceAsync(FilterDto filterDto);

        Task<DataResultDto<GridResultDto<ReceiptDocumentItemDto>>> GetReceiptDocumentItems(FilterReceiptItemsDto filterDto);

        Task<ResultDto> CreateReceiptDocumentAsync(ReceiptDocumentDto receiptDocumentDto);

        Task<DataResultDto<GridResultDto<string>>> GetReceiptsDocumentNumbersFilterAsync(FilterDto filterDto);
        Task<DataResultDto<GridResultDto<ResourceDto>>> GetResourcesFilterAsync(FilterDto filterDto);
        Task<DataResultDto<GridResultDto<MeasurementDto>>> GetMeasurementsFilterAsync(FilterDto filterDto);

        Task<DataResultDto<ReceiptDocumentDto>> GetWholeReceiptDocumentAsync(ReceiptDocumentItemDto receiptDocumentItemDto);

        Task<ResultDto> UpdateReceiptDocumentAsync(ReceiptDocumentDto receiptDocumentDto);

        Task<ResultDto> CanRemoveReceiptResourceAsync(long receiptResourceId);
        Task<ResultDto> DeleteReceiptDocumentAsync(long receiptDocumentId);

        /// <summary>
        /// Получение Resource или Measurement для фильтров в Balance
        /// </summary>
        /// <param name="filterBalanceDto"></param>
        /// <returns></returns>
        Task<DataResultDto<GridResultDto<BalanceDto>>> GetBalanceFiltersAsync(FilterBalanceDto filterBalanceDto);
    }
}

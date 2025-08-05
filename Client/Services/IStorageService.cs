using DataContracts;
using Radzen;

namespace SolforbTestTask.Client.Services
{
    public interface IStorageService
    {
        Task<DataResultDto<GridResultDto<BalanceDto>>> GetBalanceAsync(FilterDto filterDto);

        Task<DataResultDto<GridResultDto<ReceiptDocumentDto>>> GetReceiptAsync(FilterDto filterDto);

        Task<ResultDto> CreateReceiptDocumentAsync(ReceiptDocumentDto receiptDocumentDto);
    }
}

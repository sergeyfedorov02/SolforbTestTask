using DataContracts;
using Radzen;
using SolforbTestTask.Client.Services;
using System.Threading.Tasks;

namespace SolforbTestTask.Server.Services
{
    public interface IStorageService
    {
        Task<DataResultDto<GridResultDto<BalanceDto>>> GetBalanceAsync(Query query);

        Task<DataResultDto<GridResultDto<ReceiptDocumentItemDto>>> GetReceiptDocumentItemsAsync(Query query);

        Task<ResultDto> CreateReceiptDocumentAsync(ReceiptDocumentDto receiptDocumentDto);
    }
}

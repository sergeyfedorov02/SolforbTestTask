using DataContracts;
using Radzen;
using System.Threading.Tasks;

namespace SolforbTestTask.Server.Services
{
    public interface IStorageService
    {
        Task<DataResultDto<GridResultDto<BalanceDto>>> GetBalanceAsync(Query query);
        Task<DataResultDto<GridResultDto<ReceiptDto>>> GetReceiptAsync(Query query);
    }
}

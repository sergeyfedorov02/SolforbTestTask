using DataContracts;
using Radzen;

namespace SolforbTestTask.Server.Services
{
    public interface IStorageService
    {
        Task<DataResultDto<GridResultDto<BalanceDto>>> GetBalanceAsync(Query query);
    }
}

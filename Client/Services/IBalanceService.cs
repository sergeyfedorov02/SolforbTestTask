using DataContracts;
using Radzen;

namespace SolforbTestTask.Client.Services
{
    public interface IBalanceService
    {
        Task<DataResultDto<GridResultDto<BalanceDto>>> GetBalanceAsync(FilterDto filterDto);
    }
}

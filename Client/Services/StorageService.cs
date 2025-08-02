using DataContracts;
using System.Net.Http.Json;

namespace SolforbTestTask.Client.Services
{
    public class StorageService : IStorageService
    {
        private readonly HttpClient _httpClient;

        public StorageService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DataResultDto<GridResultDto<BalanceDto>>> GetBalanceAsync(FilterDto filterDto)
        {
            try
            {
                var x = await _httpClient.PostAsJsonAsync($"api/storage/getBalance?", filterDto);

                if (!x.IsSuccessStatusCode)
                {
                    throw new Exception($"Ошибка при получении данных для Balance от Сервера {x.StatusCode}");
                }

               var data = await x.Content.ReadFromJsonAsync<GridResultDto<BalanceDto>>();
               return DataResultDto<GridResultDto<BalanceDto>>.CreateFromData(data);
            }
            catch (Exception ex)
            {
                return DataResultDto<GridResultDto<BalanceDto>>.CreateFromException(ex);
            }
        }
    }
}

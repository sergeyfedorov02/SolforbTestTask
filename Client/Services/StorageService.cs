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

        public async Task<DataResultDto<GridResultDto<ReceiptDocumentDto>>> GetReceiptAsync(FilterDto filterDto)
        {
            try
            {
                var x = await _httpClient.PostAsJsonAsync($"api/storage/getReceipt?", filterDto);

                if (!x.IsSuccessStatusCode)
                {
                    throw new Exception($"Ошибка при получении данных для Receipt от Сервера {x.StatusCode}");
                }

                var data = await x.Content.ReadFromJsonAsync<GridResultDto<ReceiptDocumentDto>>();
                return DataResultDto<GridResultDto<ReceiptDocumentDto>>.CreateFromData(data);
            }
            catch (Exception ex)
            {
                return DataResultDto<GridResultDto<ReceiptDocumentDto>>.CreateFromException(ex);
            }
        }

        /// <summary>
        /// Создание ReceiptDocument
        /// </summary>
        /// <param name="receiptDocumentDto"></param>
        /// <returns></returns>
        public async Task<ResultDto> CreateReceiptDocumentAsync(ReceiptDocumentDto receiptDocumentDto)
        {
            try
            {
                var x = await _httpClient.PostAsJsonAsync("api/storage/createReceiptDocument", receiptDocumentDto);

                if (!x.IsSuccessStatusCode)
                {
                    return ResultDto.CreateFromException(new Exception($"Ошибка при создании ReceiptDocument от Сервера статус={x.StatusCode}"));
                }
                return ResultDto.CreateOk();
            }

            catch (Exception ex)
            {
                return ResultDto.CreateFromException(new Exception("Ошибка при создании ReceiptDocument от Сервера", ex));
            }
        }
    }
}

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

        /// <summary>
        /// Получение записей для страницы Balance
        /// </summary>
        /// <param name="filterDto"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Получение записей для страницы Receipts
        /// </summary>
        /// <param name="filterDto"></param>
        /// <returns></returns>
        public async Task<DataResultDto<GridResultDto<ReceiptDocumentItemDto>>> GetReceptDocumentItems(FilterReceiptItemsDto filterDto)
        {
            try
            {
                var x = await _httpClient.PostAsJsonAsync($"api/storage/getReceiptItems", filterDto);

                if (!x.IsSuccessStatusCode)
                {
                    throw new Exception($"Ошибка при получении данных для Receipt от Сервера {x.StatusCode}");
                }

                var data = await x.Content.ReadFromJsonAsync<GridResultDto<ReceiptDocumentItemDto>>();
                return DataResultDto<GridResultDto<ReceiptDocumentItemDto>>.CreateFromData(data);
            }
            catch (Exception ex)
            {
                return DataResultDto<GridResultDto<ReceiptDocumentItemDto>>.CreateFromException(ex);
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

        /// <summary>
        /// Получение Numbers для фильтрации
        /// </summary>
        /// <returns></returns>
        public async Task<DataResultDto<GridResultDto<string>>> GetReceiptsDocumentNumbersFilterAsync(FilterDto filterDto)
        {
            try
            {
                var x = await _httpClient.PostAsJsonAsync("api/storage/getReceiptsDocumentNumbersFilter", filterDto);

                if (!x.IsSuccessStatusCode)
                {
                    throw new Exception($"Ошибка при получении номеров документов: {x.StatusCode}");
                }

                var data = await x.Content.ReadFromJsonAsync<GridResultDto<string>>();
                return DataResultDto<GridResultDto<string>>.CreateFromData(data);
            }
            catch (Exception ex)
            {
                return DataResultDto<GridResultDto<string>>.CreateFromException(ex);
            }
        }

        /// <summary>
        /// Получение Resources для фильтрации
        /// </summary>
        /// <returns></returns>
        public async Task<DataResultDto<GridResultDto<ResourceDto>>> GetResourcesFilterAsync(FilterDto filterDto)
        {
            try
            {
                var x = await _httpClient.PostAsJsonAsync("api/storage/getResourcesFilter", filterDto);

                if (!x.IsSuccessStatusCode)
                {
                    throw new Exception($"Ошибка при получении ресурсов для фильтров: {x.StatusCode}");
                }

                var data = await x.Content.ReadFromJsonAsync<GridResultDto<ResourceDto>>();
                return DataResultDto<GridResultDto<ResourceDto>>.CreateFromData(data);
            }
            catch (Exception ex)
            {
                return DataResultDto<GridResultDto<ResourceDto>>.CreateFromException(ex);
            }
        }

        /// <summary>
        /// Получение Measurements для фильтрации
        /// </summary>
        /// <returns></returns>
        public async Task<DataResultDto<GridResultDto<MeasurementDto>>> GetMeasurementsFilterAsync(FilterDto filterDto)
        {
            try
            {
                var x = await _httpClient.PostAsJsonAsync("api/storage/getMeasurementsFilter", filterDto);

                if (!x.IsSuccessStatusCode)
                {
                    throw new Exception($"Ошибка при получении единиц измерения для фильтров: {x.StatusCode}");
                }

                var data = await x.Content.ReadFromJsonAsync<GridResultDto<MeasurementDto>>();
                return DataResultDto<GridResultDto<MeasurementDto>>.CreateFromData(data);
            }
            catch (Exception ex)
            {
                return DataResultDto<GridResultDto<MeasurementDto>>.CreateFromException(ex);
            }
        }
    }
}

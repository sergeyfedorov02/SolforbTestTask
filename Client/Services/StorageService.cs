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
        public async Task<DataResultDto<GridResultDto<ReceiptDocumentItemDto>>> GetReceiptDocumentItems(FilterReceiptItemsDto filterDto)
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

        /// <inheritdoc />
        public async Task<DataResultDto<GridResultDto<BalanceDto>>> GetBalanceFiltersAsync(FilterBalanceDto filterBalanceDto)
        {
            try
            {
                var x = await _httpClient.PostAsJsonAsync("api/storage/getBalanceFilters", filterBalanceDto);

                if (!x.IsSuccessStatusCode)
                {
                    throw new Exception($"Ошибка при получении {filterBalanceDto.ColumnName} для фильтров: {x.StatusCode}");
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

        /// <summary>
        /// Получение ReceiptDocumentDto из переданного ReceiptDocumentItemDto
        /// </summary>
        /// <param name="receiptDocumentItemDto"></param>
        /// <returns></returns>
        public async Task<DataResultDto<ReceiptDocumentDto>> GetWholeReceiptDocumentAsync(ReceiptDocumentItemDto receiptDocumentItemDto)
        {
            try
            {
                var x = await _httpClient.PostAsJsonAsync("api/storage/getWholeReceiptDocument", receiptDocumentItemDto);

                if (!x.IsSuccessStatusCode)
                {
                    throw new Exception($"Ошибка при получении полного ReceiptDocument: {x.StatusCode}");
                }

                var data = await x.Content.ReadFromJsonAsync<ReceiptDocumentDto>();
                return DataResultDto<ReceiptDocumentDto>.CreateFromData(data);
            }
            catch (Exception ex)
            {
                return DataResultDto<ReceiptDocumentDto>.CreateFromException(ex);
            }
        }

        /// <summary>
        /// Обновление ReceiptDocument
        /// </summary>
        /// <param name="receiptDocumentDto"></param>
        /// <returns></returns>
        public async Task<ResultDto> UpdateReceiptDocumentAsync(ReceiptDocumentDto receiptDocumentDto)
        {
            try
            {
                var x = await _httpClient.PutAsJsonAsync("api/storage/updateReceiptDocument", receiptDocumentDto);

                if (!x.IsSuccessStatusCode)
                {
                    return ResultDto.CreateFromException(new Exception($"Ошибка при редактировании ReceiptDocument от Сервера статус={x.StatusCode}"));
                }
                return ResultDto.CreateOk();
            }
            catch (Exception ex)
            {
                return ResultDto.CreateFromException(ex);
            }
        }

        public async Task<ResultDto> CanRemoveReceiptResourceAsync(long receiptResourceId)
        {
            try
            {
                var x = await _httpClient.GetAsync($"api/storage/canRemoveReceiptResource?receiptResourceId={receiptResourceId}");

                if (!x.IsSuccessStatusCode)
                {
                    return ResultDto.CreateFromException(
                        new Exception($"Ошибка при удалении ReceiptResource во время обновления ReceiptDocument от Сервера статус={x.StatusCode}"));
                }

                return ResultDto.CreateOk();
            }
            catch (Exception ex)
            {
                return ResultDto.CreateFromException(ex);
            }
        }

        /// <summary>
        /// Удаление ReceiptDocument
        /// </summary>
        /// <param name="receiptDocumentId"></param>
        /// <returns></returns>
        public async Task<ResultDto> DeleteReceiptDocumentAsync(long receiptDocumentId)
        {
            try
            {
                var x = await _httpClient.DeleteAsync($"api/storage/deleteReceiptDocument/{receiptDocumentId}");

                if (!x.IsSuccessStatusCode)
                {
                    return ResultDto.CreateFromException(new Exception($"Ошибка при удалении ReceiptDocument от Сервера статус={x.StatusCode}"));
                }

                return ResultDto.CreateOk();
            }
            catch (Exception ex)
            {
                return ResultDto.CreateFromException(ex);
            }
        }
    }
}

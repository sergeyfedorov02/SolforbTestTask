using DataContracts;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SolforbTestTask.Client.Services
{
    public class DirectoryService : IDirectoryService
    {
        private readonly HttpClient _httpClient;

        public DirectoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Получение списка всех Resource
        /// </summary>
        /// <param name="filterDirectoryDto"></param>
        /// <returns></returns>
        public async Task<DataResultDto<GridResultDto<ResourceDto>>> GetResourceAsync(FilterDirectoryDto filterDirectoryDto)
        {
            try
            {
                var x = await _httpClient.PostAsJsonAsync($"api/directory/getResource?", filterDirectoryDto);

                if (!x.IsSuccessStatusCode)
                {
                    throw new Exception($"Ошибка при получении данных для Resource от Сервера {x.StatusCode}");
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
        /// Создание Resource 
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public async Task<ResultDto> CreateResourceAsync(string resourceName)
        {
            try
            {
                var x = await _httpClient.PostAsJsonAsync("api/directory/createResource", resourceName);

                if (!x.IsSuccessStatusCode)
                {
                    return ResultDto.CreateFromException(new Exception($"Ошибка при создании Resource от Сервера статус={x.StatusCode}"));
                }
                return ResultDto.CreateOk();
            }

            catch (Exception ex)
            {
                return ResultDto.CreateFromException(new Exception("Ошибка при создании Resource от Сервера", ex));
            }
        }

        /// <summary>
        /// Редактирование Resource
        /// </summary>
        /// <param name="resourceDto"></param>
        /// <returns></returns>
        public async Task<ResultDto> UpdateResourceAsync(ResourceDto resourceDto)
        {
            try
            {
                var x = await _httpClient.PutAsJsonAsync("api/directory/updateResource", resourceDto);

                if (!x.IsSuccessStatusCode)
                {
                    return ResultDto.CreateFromException(new Exception($"Ошибка при редактировании Resource от Сервера статус={x.StatusCode}"));
                }
                return ResultDto.CreateOk();
            }
            catch (Exception ex)
            {
                return ResultDto.CreateFromException(ex);
            }
        }

        /// <summary>
        /// Архивирование Resource
        /// </summary>
        /// <param name="resourceDto"></param>
        /// <returns></returns>
        public async Task<ResultDto> ArchiveResourceAsync(ResourceDto resourceDto)
        {
            try
            {
                var x = await _httpClient.PostAsJsonAsync("api/directory/archiveResource", resourceDto);

                if (!x.IsSuccessStatusCode)
                {
                    return ResultDto.CreateFromException(new Exception($"Ошибка при архивировании Resource от Сервера статус={x.StatusCode}"));
                }

                return ResultDto.CreateOk();
            }
            catch (Exception ex)
            {
                return ResultDto.CreateFromException(ex);
            }
        }

        /// <summary>
        /// Удаление Resource
        /// </summary>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        public async Task<ResultDto> DeleteResourceAsync(long resourceId)
        {
            try
            {
                var x = await _httpClient.DeleteAsync($"api/directory/deleteResource/{resourceId}");

                if (!x.IsSuccessStatusCode)
                {
                    return ResultDto.CreateFromException(new Exception($"Ошибка при удалении Resource от Сервера статус={x.StatusCode}"));
                }

                return ResultDto.CreateOk();
            }
            catch (Exception ex)
            {
                return ResultDto.CreateFromException(ex);
            }
        }

        /// <summary>
        /// Получение списка всех Measurement
        /// </summary>
        /// <param name="filterDirectoryDto"></param>
        /// <returns></returns>
        public async Task<DataResultDto<GridResultDto<MeasurementDto>>> GetMeasurementAsync(FilterDirectoryDto filterDirectoryDto)
        {
            try
            {
                var x = await _httpClient.PostAsJsonAsync($"api/directory/getMeasurement?", filterDirectoryDto);

                if (!x.IsSuccessStatusCode)
                {
                    throw new Exception($"Ошибка при получении данных для Measurement от Сервера {x.StatusCode}");
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
        /// Создание Measurement
        /// </summary>
        /// <param name="measurementName"></param>
        /// <returns></returns>
        public async Task<ResultDto> CreateMeasurementAsync(string measurementName)
        {
            try
            {
                var x = await _httpClient.PostAsJsonAsync("api/directory/createMeasurement", measurementName);

                if (!x.IsSuccessStatusCode)
                {
                    return ResultDto.CreateFromException(new Exception($"Ошибка при создании Measurement от Сервера статус={x.StatusCode}"));
                }
                return ResultDto.CreateOk();
            }

            catch (Exception ex)
            {
                return ResultDto.CreateFromException(new Exception("Ошибка при создании Measurement от Сервера", ex));
            }
        }

        /// <summary>
        /// Редактирование Measurement
        /// </summary>
        /// <param name="measurementDto"></param>
        /// <returns></returns>
        public async Task<ResultDto> UpdateMeasurementAsync(MeasurementDto measurementDto)
        {
            try
            {
                var x = await _httpClient.PutAsJsonAsync("api/directory/updateMeasurement", measurementDto);

                if (!x.IsSuccessStatusCode)
                {
                    return ResultDto.CreateFromException(new Exception($"Ошибка при редактировании Measurement от Сервера статус={x.StatusCode}"));
                }
                return ResultDto.CreateOk();
            }
            catch (Exception ex)
            {
                return ResultDto.CreateFromException(ex);
            }
        }

        /// <summary>
        /// Архивирование Measurement
        /// </summary>
        /// <param name="measurementDto"></param>
        /// <returns></returns>
        public async Task<ResultDto> ArchiveMeasurementAsync(MeasurementDto measurementDto)
        {
            try
            {
                var x = await _httpClient.PostAsJsonAsync("api/directory/archiveMeasurement", measurementDto);

                if (!x.IsSuccessStatusCode)
                {
                    return ResultDto.CreateFromException(new Exception($"Ошибка при архивировании Measurement от Сервера статус={x.StatusCode}"));
                }

                return ResultDto.CreateOk();
            }
            catch (Exception ex)
            {
                return ResultDto.CreateFromException(ex);
            }
        }

        /// <summary>
        /// Удаление Measurement
        /// </summary>
        /// <param name="measurementId"></param>
        /// <returns></returns>
        public async Task<ResultDto> DeleteMeasurementAsync(long measurementId)
        {
            try
            {
                var x = await _httpClient.DeleteAsync($"api/directory/deleteMeasurement/{measurementId}");

                if (!x.IsSuccessStatusCode)
                {
                    return ResultDto.CreateFromException(new Exception($"Ошибка при удалении Measurement от Сервера статус={x.StatusCode}"));
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

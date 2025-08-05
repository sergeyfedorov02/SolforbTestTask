using DataContracts;
using Microsoft.AspNetCore.Mvc;
using Radzen;
using SolforbTestTask.Server.Services;

namespace SolforbTestTask.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DirectoryController : ControllerBase
    {
        private readonly IDirectoryService _directoryService;  // доступ к базе данных
        private ILogger<StorageController> Logger { get; }

        public DirectoryController(
            IDirectoryService directoryService,  // для связи БД и объектной модели C# (в папке Entities)
            // IValidator<CsvRecordDto> validator,
            ILogger<StorageController> logger)
        {
            _directoryService = directoryService;
            //_validator = validator;
            Logger = logger;
        }

        /// <summary>
        /// Получение записей из Resource
        /// </summary>
        /// <param name="filterDirectoryDto"></param>
        /// <returns></returns>
        [HttpPost("getResource")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ResourceDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GridResultDto<ResourceDto>>> GetResource(FilterDirectoryDto filterDirectoryDto)
        {
            var result = await _directoryService.GetResourceAsync(new Query
            {
                Skip = filterDirectoryDto.Skip,
                Top = filterDirectoryDto.Top,
                Filter = filterDirectoryDto.Filter,
                OrderBy = filterDirectoryDto.OrderBy
            }, filterDirectoryDto.Status);

            if (!result.Success)
            {
                Logger.LogError(result.Exception, "Ошибка при получении данных для Resource от Сервиса");
                return StatusCode(500, "Ошибка при получении данных для Resource от Сервиса");

            }
            return Ok(result.Data);
        }

        /// <summary>
        /// Создание Resource
        /// </summary>
        /// <param name="resourceDto"></param>
        /// <returns></returns>
        [HttpPost("createResource")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResourceDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> CreateResource([FromBody] ResourceDto resourceDto)
        {
            if (string.IsNullOrWhiteSpace(resourceDto.Name))
            {
                return BadRequest("Не указано наименование");
            }

            var result = await _directoryService.CreateResourceAsync(resourceDto);

            if (!result.Success)
            {
                Logger.LogError(result.Exception, "Ошибка при создании Resource от Сервиса");
                return StatusCode(500, "Ошибка при создании Resource от Сервиса");

            }
            return Ok(true);
        }

        /// <summary>
        /// Редактирование Resource
        /// </summary>
        /// <param name="resourceDto"></param>
        /// <returns></returns>
        [HttpPut("updateResource")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResourceDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> UpdateResource([FromBody] ResourceDto resourceDto)
        {
            if (string.IsNullOrWhiteSpace(resourceDto.Name))
            {
                return BadRequest("Нельзя указать пустое наименование");
            }

            var result = await _directoryService.UpdateResourceAsync(resourceDto);

            if (!result.Success)
            {
                Logger.LogError(result.Exception, "Ошибка при обновлении Resource от Сервиса");
                return StatusCode(500, "Ошибка при обновлении Resource от Сервиса");
            }

            return Ok(true);
        }

        /// <summary>
        /// Архивирование Resource
        /// </summary>
        /// <param name="resourceDto"></param>
        /// <returns></returns>
        [HttpPost("archiveResource")]
        public async Task<ActionResult<bool>> ArchiveResource([FromBody] ResourceDto resourceDto)
        {
            var result = await _directoryService.ArchiveResourceAsync(resourceDto);

            if (!result.Success)
            {
                Logger.LogError(result.Exception, "Ошибка при архивировании Resource");
                return BadRequest(result.Exception?.Message);
            }

            return Ok(true);
        }

        /// <summary>
        /// Удаление Resource
        /// </summary>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        [HttpDelete("deleteResource/{resourceId}")]
        public async Task<ActionResult<bool>> DeleteResource(long resourceId)
        {
            var result = await _directoryService.DeleteResourceAsync(resourceId);

            if (!result.Success)
            {
                Logger.LogError(result.Exception, "Ошибка при удалении Resource");
                return BadRequest(result.Exception?.Message);
            }

            return Ok(true);
        }

        /// <summary>
        /// Получение записей из Measurement
        /// </summary>
        /// <param name="filterDirectoryDto"></param>
        /// <returns></returns>
        [HttpPost("getMeasurement")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<MeasurementDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GridResultDto<MeasurementDto>>> GetMeasurement(FilterDirectoryDto filterDirectoryDto)
        {
            var result = await _directoryService.GetMeasurementAsync(new Query
            {
                Skip = filterDirectoryDto.Skip,
                Top = filterDirectoryDto.Top,
                Filter = filterDirectoryDto.Filter,
                OrderBy = filterDirectoryDto.OrderBy
            }, filterDirectoryDto.Status);

            if (!result.Success)
            {
                Logger.LogError(result.Exception, "Ошибка при получении данных для Measurement от Сервиса");
                return StatusCode(500, "Ошибка при получении данных для Measurement от Сервиса");

            }
            return Ok(result.Data);
        }

        /// <summary>
        /// Создание Measurement
        /// </summary>
        /// <param name="measurementDto"></param>
        /// <returns></returns>
        [HttpPost("createMeasurement")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MeasurementDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> CreateMeasurement([FromBody] MeasurementDto measurementDto)
        {
            if (string.IsNullOrWhiteSpace(measurementDto.Name))
            {
                return BadRequest("Не указано наименование");
            }

            var result = await _directoryService.CreateMeasurementAsync(measurementDto);

            if (!result.Success)
            {
                Logger.LogError(result.Exception, "Ошибка при создании Measurement от Сервиса");
                return StatusCode(500, "Ошибка при создании Measurement от Сервиса");

            }
            return Ok(true);
        }

        /// <summary>
        /// Редактирование Measurement
        /// </summary>
        /// <param name="measurementDto"></param>
        /// <returns></returns>
        [HttpPut("updateMeasurement")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MeasurementDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> UpdateMeasurement([FromBody] MeasurementDto measurementDto)
        {
            if (string.IsNullOrWhiteSpace(measurementDto.Name))
            {
                return BadRequest("Нельзя указать пустое наименование");
            }

            var result = await _directoryService.UpdateMeasurementAsync(measurementDto);

            if (!result.Success)
            {
                Logger.LogError(result.Exception, "Ошибка при обновлении Measurement от Сервиса");
                return StatusCode(500, "Ошибка при обновлении Measurement от Сервиса");
            }

            return Ok(true);
        }

        /// <summary>
        /// Архивирование Measurement
        /// </summary>
        /// <param name="measurementDto"></param>
        /// <returns></returns>
        [HttpPost("archiveMeasurement")]
        public async Task<ActionResult<bool>> ArchiveMeasurement([FromBody] MeasurementDto measurementDto)
        {
            var result = await _directoryService.ArchiveMeasurementAsync(measurementDto);

            if (!result.Success)
            {
                Logger.LogError(result.Exception, "Ошибка при архивировании Measurement");
                return BadRequest(result.Exception?.Message);
            }

            return Ok(true);
        }

        /// <summary>
        /// Удаление Measurement
        /// </summary>
        /// <param name="measurementId"></param>
        /// <returns></returns>
        [HttpDelete("deleteMeasurement/{measurementId}")]
        public async Task<ActionResult<bool>> DeleteMeasurement(long measurementId)
        {
            var result = await _directoryService.DeleteMeasurementAsync(measurementId);

            if (!result.Success)
            {
                Logger.LogError(result.Exception, "Ошибка при удалении Measurement");
                return BadRequest(result.Exception?.Message);
            }

            return Ok(true);
        }
    }
}

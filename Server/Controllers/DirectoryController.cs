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
        /// <param name="resourceId"></param>
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
        /// <param name="id"></param>
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
    }
}

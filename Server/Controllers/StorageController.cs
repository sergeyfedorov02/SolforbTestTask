using DataContracts;
using Microsoft.AspNetCore.Mvc;
using Radzen;
using SolforbTestTask.Server.Services;

namespace SolforbTestTask.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StorageController : ControllerBase
    {
        private readonly IStorageService _storageService;  // доступ к базе данных
        private ILogger<StorageController> Logger { get; }

        public StorageController(
            IStorageService storageService,  // для связи БД и объектной модели C# (в папке Entities)
            // IValidator<CsvRecordDto> validator,
            ILogger<StorageController> logger)
        {
            _storageService = storageService;
            //_validator = validator;
            Logger = logger;
        }

        /// <summary>
        /// Получение записей из Balance
        /// </summary>
        /// <returns></returns>
        [HttpPost("getBalance")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<BalanceDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GridResultDto<BalanceDto>>> GetBalance(FilterDto filterDto)
        {
            var result = await _storageService.GetBalanceAsync(new Query
            {
                Skip = filterDto.Skip,
                Top = filterDto.Top,
                Filter = filterDto.Filter,
                OrderBy = filterDto.OrderBy,
            });

            if (!result.Success)
            {
                Logger.LogError(result.Exception, "Ошибка при получении данных для Balance от Сервиса");
                return StatusCode(500, "Ошибка при получении данных для Balance от Сервиса");

            }
            return Ok(result.Data);
        }

        /// <summary>
        /// Получение записей из Receipt
        /// </summary>
        /// <returns></returns>
        [HttpPost("getReceiptItems")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<BalanceDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GridResultDto<ReceiptDocumentItemDto>>> GetReceptDocumentItems(FilterReceiptItemsDto filterDto)
        {
            var result = await _storageService.GetReceiptDocumentItemsAsync(filterDto);

            if (!result.Success)
            {
                Logger.LogError(result.Exception, "Ошибка при получении данных для Receipt от Сервиса");
                return StatusCode(500, "Ошибка при получении данных для Receipt от Сервиса");

            }
            return Ok(result.Data);
        }

        /// <summary>
        /// Создание ReceiptDocument
        /// </summary>
        /// <param name="receiptDocumentDto"></param>
        /// <returns></returns>
        [HttpPost("createReceiptDocument")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> CreateReceiptDocument([FromBody] ReceiptDocumentDto receiptDocumentDto)
        {
            if (string.IsNullOrWhiteSpace(receiptDocumentDto.Number))
            {
                return BadRequest("Не указан номер документа");
            }

            var result = await _storageService.CreateReceiptDocumentAsync(receiptDocumentDto);

            if (!result.Success)
            {
                Logger.LogError(result.Exception, "Ошибка при создании ReceiptDocument от Сервиса");
                return StatusCode(500, "Ошибка при создании ReceiptDocument от Сервиса");

            }
            return Ok(true);
        }

        /// <summary>
        /// Получение ReceiptsDocument.Number для фильтров
        /// </summary>
        /// <returns></returns>
        [HttpPost("getReceiptsDocumentNumbersFilter")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<string>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<string>>> GetReceiptsDocumentNumbersFilter(FilterDto filterDto)
        {
            var result = await _storageService.GetReceiptsDocumentNumbersFilterAsync(new Query
            {
                Skip = filterDto.Skip,
                Top = filterDto.Top,
                Filter = filterDto.Filter,
                OrderBy = filterDto.OrderBy,
            });

            if (!result.Success)
            {
                Logger.LogError(result.Exception, "Ошибка при получении номеров документов для фильтров");
                return StatusCode(500, "Ошибка при получении данных");
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Получение Resource для фильтров
        /// </summary>
        /// <returns></returns>
        [HttpPost("getResourcesFilter")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<string>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<string>>> GetResourcesFilter(FilterDto filterDto)
        {
            var result = await _storageService.GetResourcesFilterAsync(new Query
            {
                Skip = filterDto.Skip,
                Top = filterDto.Top,
                Filter = filterDto.Filter,
                OrderBy = filterDto.OrderBy,
            });

            if (!result.Success)
            {
                Logger.LogError(result.Exception, "Ошибка при получении ресурсов для фильтров");
                return StatusCode(500, "Ошибка при получении данных");
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Получение Measurement для фильтров
        /// </summary>
        /// <returns></returns>
        [HttpPost("getMeasurementsFilter")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<string>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<string>>> GetMeasurementsFilter(FilterDto filterDto)
        {
            var result = await _storageService.GetMeasurementsFilterAsync(new Query
            {
                Skip = filterDto.Skip,
                Top = filterDto.Top,
                Filter = filterDto.Filter,
                OrderBy = filterDto.OrderBy,
            });

            if (!result.Success)
            {
                Logger.LogError(result.Exception, "Ошибка при получении единиц измерения для фильтров");
                return StatusCode(500, "Ошибка при получении данных");
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Получение ReceiptDocumentDto из одного ReceiptDocumentItemDto
        /// </summary>
        /// <param name="receiptDocumentItemDto"></param>
        /// <returns></returns>
        [HttpPost("getWholeReceiptDocument")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ReceiptDocumentDto>> GetWholeReceiptDocument([FromBody] ReceiptDocumentItemDto receiptDocumentItemDto)
        {
            var result = await _storageService.GetWholeReceiptDocumentAsync(receiptDocumentItemDto);

            if (!result.Success)
            {
                Logger.LogError(result.Exception, "Ошибка при редактировании ReceiptDocument от Сервиса");
                return StatusCode(500, "Ошибка при редактировании ReceiptDocument от Сервиса");

            }
            return Ok(result.Data);
        }

        /// <summary>
        /// Обновление ReceiptDocument
        /// </summary>
        /// <param name="receiptDocumentDto"></param>
        /// <returns></returns>
        [HttpPut("updateReceiptDocument")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> UpdateReceiptDocument([FromBody] ReceiptDocumentDto receiptDocumentDto)
        {
            if (string.IsNullOrWhiteSpace(receiptDocumentDto.Number))
            {
                return BadRequest("Не указан номер документа");
            }

            var result = await _storageService.UpdateReceiptDocumentAsync(receiptDocumentDto);

            if (!result.Success)
            {
                Logger.LogError(result.Exception, "Ошибка при создании ReceiptDocument от Сервиса");
                return StatusCode(500, "Ошибка при создании ReceiptDocument от Сервиса");

            }
            return Ok(true);
        }
    }
}

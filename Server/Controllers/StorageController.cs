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
    }
}

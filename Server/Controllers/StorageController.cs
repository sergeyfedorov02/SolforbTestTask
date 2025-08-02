using DataContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Radzen;
using SolforbTestTask.Server.Services;
using System.ComponentModel.DataAnnotations;

namespace SolforbTestTask.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StorageController : ControllerBase
    {
        private readonly IStorageService _balanceService;  // доступ к базе данных
        private ILogger<StorageController> Logger { get; }

        public StorageController(
            IStorageService balanceService,  // для связи БД и объектной модели C# (в папке Entities)
            // IValidator<CsvRecordDto> validator,
            ILogger<StorageController> logger)
        {
            _balanceService = balanceService;
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
            var result = await _balanceService.GetBalanceAsync(new Query
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
        [HttpPost("getReceipt")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<BalanceDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GridResultDto<ReceiptDto>>> GetReceipt(FilterDto filterDto)
        {
            var result = await _balanceService.GetReceiptAsync(new Query
            {
                Skip = filterDto.Skip,
                Top = filterDto.Top,
                Filter = filterDto.Filter,
                OrderBy = filterDto.OrderBy,
            });

            if (!result.Success)
            {
                Logger.LogError(result.Exception, "Ошибка при получении данных для Receipt от Сервиса");
                return StatusCode(500, "Ошибка при получении данных для Receipt от Сервиса");

            }
            return Ok(result.Data);
        }
    }
}

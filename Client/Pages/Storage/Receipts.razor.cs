using DataContracts;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using SolforbTestTask.Client.Components;
using SolforbTestTask.Client.Extensions;
using SolforbTestTask.Client.Services;

namespace SolforbTestTask.Client.Pages.Storage
{
    public partial class Receipts
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected IStorageService StorageService { get; set; }

        [Inject]
        protected ILogger<Index> Logger { get; set; }

        protected LocalizedDataGrid<ReceiptDocumentItemDto> grid;
        protected bool isLoading;

        protected IEnumerable<ReceiptDocumentItemDto> receipts;
        protected int count;

        protected int pageSize = 20;
        protected readonly IEnumerable<int> pageSizeOptions = [10, 20, 50];

        // настройка базовых значений для фильтров по дате
        private DateOnly? fromDate = DateOnly.FromDateTime(DateTime.Now).AddDays(-7);
        private DateOnly? toDate = DateOnly.FromDateTime(DateTime.Now).AddDays(7);

        // состояния для фильтров
        private IEnumerable<string> selectedDocumentNumbers = [];
        private IEnumerable<long> selectedResources = [];
        private IEnumerable<long> selectedMeasurements = [];

        // данные для фильтров (текущие значения -> постепенная подгрузка)
        private List<string> currentDocumentNumbers = [];
        private List<ResourceDto> currentResources = [];
        private List<MeasurementDto> currentMeasurements = [];

        private int countDocumentNumbers;
        private int countResources;
        private int countMeasurements;

        /// <summary>
        /// инициализация данных и фильтров
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            // чтобы работали фильтры с учетом вирутализации (постепенная подгрузка)
            await LoadFilterDataAsync();
        }

        /// <summary>
        /// загрузка данных для фильтров
        /// </summary>
        /// <returns></returns>
        private async Task LoadFilterDataAsync()
        {
            // номера документов
            await LoadDataFilterDocumentNumbers(new LoadDataArgs { Top = 1 });

            // ресурсы
            await LoadDataFilterResources(new LoadDataArgs { Top = 1 });

            // единицы измерения
            await LoadDataFilterMeasurements(new LoadDataArgs { Top = 1 });
        }

        /// <summary>
        /// данные для фильтра по DocumentNumbers
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected async Task LoadDataFilterDocumentNumbers(LoadDataArgs args)
        {
            var docResult = await StorageService.GetReceiptsDocumentNumbersFilterAsync(new FilterDto
            {
                Skip = args.Skip,
                Top = args.Top ?? 1
            });


            if (docResult.Success)
            {
                currentDocumentNumbers = docResult.Data.Data;
                countDocumentNumbers = docResult.Data.Count;
            }
        }

        /// <summary>
        /// данные для фильтра по Resource
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected async Task LoadDataFilterResources(LoadDataArgs args)
        {
            var resResult = await StorageService.GetResourcesFilterAsync(new FilterDto
            {
                Skip = args.Skip,
                Top = args.Top ?? 1
            });


            if (resResult.Success)
            {
                currentResources = resResult.Data.Data;
                countResources = resResult.Data.Count;
            }
        }

        /// <summary>
        /// данные для фильтра по Measurement
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected async Task LoadDataFilterMeasurements(LoadDataArgs args)
        {
            var measResult = await StorageService.GetMeasurementsFilterAsync(new FilterDto
            {
                Skip = args.Skip,
                Top = args.Top ?? 1
            });


            if (measResult.Success)
            {
                currentMeasurements = measResult.Data.Data;
                countMeasurements = measResult.Data.Count;
            }
        }

        /// <summary>
        /// Группировка по DocumentId для отображения по группам (GroupHeader)
        /// </summary>
        /// <param name="args"></param>
        private void OnRender(DataGridRenderEventArgs<ReceiptDocumentItemDto> args)
        {
            if (args.FirstRender)
            {
                args.Grid.Groups.Add(new GroupDescriptor
                {
                    Property = nameof(ReceiptDocumentItemDto.Id),
                });
                StateHasChanged();
            }
        }

        /// <summary>
        /// Получение номера документа в GroupHeader
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        private string GetDocumentNumber(Group group)
        {
            var groupFirstItem = group.Data.Items.OfType<ReceiptDocumentItemDto>().First();
            return groupFirstItem.Number;
        }

        /// <summary>
        /// Получение даты документа в GroupHeader
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        private string GetDocumentDate(Group group)
        {
            var groupFirstItem = group.Data.Items.OfType<ReceiptDocumentItemDto>().First();
            return groupFirstItem.Date.ToString("dd.MM.yyyy");
        }

        /// <summary>
        /// Подгрузка элементов в гриде/таблице
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected async Task LoadData(LoadDataArgs args)
        {
            isLoading = true;

            var result = await StorageService.GetReceptDocumentItems(new FilterReceiptItemsDto
            {
                Skip = args.Skip,
                Top = args.Top,
                FromDate = fromDate,
                ToDate = toDate,
                DocumentNumbers = [.. selectedDocumentNumbers],
                ResourceIds = [.. selectedResources],
                MeasurementIds = [.. selectedMeasurements],
            });

            if (result.Success)
            {
                count = result.Data.Count;
                receipts = [.. result.Data.Data];
            }
            else
            {
                Logger.LogError(result.Exception, $"Ошибка при получении таблицы Receipt (filter={args.Filter})");
                count = 0;
                receipts = [];
            }

            isLoading = false;
            StateHasChanged();
        }

        /// <summary>
        /// Вызов диалогового окна для создания документа поступления
        /// </summary>
        /// <returns></returns>
        protected async Task CreateReceiptDocumentDialogBox()
        {
            await DialogService.OpenAsync<CreateReceiptDocument>(
                "Добавление документа поступления",
                new Dictionary<string, object> { },
                new DialogOptions
                {
                    Resizable = false,
                    Draggable = true,
                    Style = "min-width:1100px; min-height:600px;",
                    CloseDialogOnOverlayClick = false
                }
            );

            await grid.Reload();
        }

        /// <summary>
        /// Двойное нажатие на группу (GroupHeader)
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        private async Task OnGroupDoubleClick(Group group)
        {
            var gg = 0;
        }

        private bool IsResetFiltersDisabled()
        {
            return fromDate == null && toDate == null &&
                !selectedDocumentNumbers.Any() &&
                !selectedResources.Any() &&
                !selectedMeasurements.Any(); 
        }

        /// <summary>
        /// Двойное нажатие на любой элемент внутри группы
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private async Task OnRowDoubleClick(DataGridRowMouseEventArgs<ReceiptDocumentItemDto> args)
        {
            var xx = 0;
            //await DialogService.OpenAsync<ViewResource>(
            //    $"Редактирование ресурса \"{args.Data.Name}\"",
            //    new Dictionary<string, object> { { "ResourceDto", args.Data } },
            //    new DialogOptions
            //    {
            //        Resizable = false,
            //        Draggable = true,
            //        CloseDialogOnOverlayClick = false
            //    }
            //);
            //await grid.Reload();
        }

        /// <summary>
        /// Реализация метода сброса всех фильтров
        /// </summary>
        /// <returns></returns>
        protected void ResetFilters()
        {
            fromDate = null;
            toDate = null;

            selectedDocumentNumbers = [];
            selectedResources = [];
            selectedMeasurements = [];

            StateHasChanged();
        }

        /// <summary>
        /// Подсветки подсказок для выбранного элемента
        /// </summary>
        /// <param name="elementReference"></param>
        /// <param name="text"></param>
        /// <param name="options"></param>
        protected void ShowTooltip(ElementReference elementReference, string text, TooltipOptions options = null)
        {
            TooltipService.Open(elementReference, text, options);
        }
    }
}

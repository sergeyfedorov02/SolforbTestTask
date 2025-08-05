using DataContracts;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using SolforbTestTask.Client.Components;
using SolforbTestTask.Client.Services;

namespace SolforbTestTask.Client.Pages.Directory
{
    public partial class Measurements
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
        protected IDirectoryService DirectoryService { get; set; }

        [Inject]
        protected ILogger<Index> Logger { get; set; }

        [Parameter]
        public int StatusType { get; set; } = 1;

        protected RadzenDataGrid<MeasurementDto> grid;
        protected bool isLoading;

        protected IEnumerable<MeasurementDto> measurements;
        protected int count;

        protected int pageSize = 20;
        protected readonly IEnumerable<int> pageSizeOptions = [10, 20, 50];

        /// <summary>
        /// Инициализация
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            await EnsureValidStatusType();
        }

        /// <summary>
        /// Перезагрузка отображаемых данных при переходе между /measurements/1 и /measurements/2
        /// </summary>
        /// <returns></returns>
        protected override async Task OnParametersSetAsync()
        {
            await EnsureValidStatusType();
        }

        /// <summary>
        /// Проверка статуса (защита от невалидных, например, /measurements/3 -> будет переход на /measurements/1)
        /// </summary>
        /// <returns></returns>
        private async Task EnsureValidStatusType()
        {
            if (StatusType != 1 && StatusType != 2)
            {
                NavigationManager.NavigateTo("/measurements/1");
            }
        }

        /// <summary>
        /// Подгрузка элементов в гриде/таблице
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected async Task LoadData(LoadDataArgs args)
        {
            isLoading = true;

            var result = await DirectoryService.GetMeasurementAsync(new FilterDirectoryDto
            {
                Skip = args.Skip,
                Top = args.Top,
                Filter = args.Filter,
                OrderBy = args.OrderBy,
                Status = StatusType
            });

            if (result.Success)
            {
                count = result.Data.Count;
                measurements = [.. result.Data.Data];
            }
            else
            {
                Logger.LogError(result.Exception, $"Ошибка при получении таблицы Measurement (filter={args.Filter})");
                count = 0;
                measurements = [];
            }

            isLoading = false;
            StateHasChanged();
        }

        /// <summary>
        /// Навигация между архивом и рабочими единицами измерения
        /// </summary>
        private void NavigateToArchiveOrWork()
        {
            NavigationManager.NavigateTo(
                $"/measurements/{(StatusType == 1 ? 2 : 1)}",
                forceLoad: true // перезагрузка
            );
        }

        /// <summary>
        /// Вызов диалогового окна для создания единицы измерения
        /// </summary>
        /// <returns></returns>
        protected async Task CreateMeasurementDialogBox()
        {
            await DialogService.OpenAsync<CreateMeasurement>(
                "Добавление единицы измерения",
                new Dictionary<string, object> { },
                new DialogOptions
                {
                    Resizable = false,
                    Draggable = true,
                    CloseDialogOnOverlayClick = false
                }
            );

            await grid.Reload();
        }

        /// <summary>
        /// Вызов диалогового окна по двойному нажатию для рендактирования единицы измерения
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private async Task OnRowDoubleClick(DataGridRowMouseEventArgs<MeasurementDto> args)
        {
            await DialogService.OpenAsync<ViewMeasurement>(
                $"Редактирование единицы измерения \"{args.Data.Name}\"",
                new Dictionary<string, object> { { "MeasurementDto", args.Data } },
                new DialogOptions
                {
                    Resizable = false,
                    Draggable = true,
                    CloseDialogOnOverlayClick = false
                }
            );
            await grid.Reload();
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

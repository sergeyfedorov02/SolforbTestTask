using DataContracts;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using SolforbTestTask.Client.Components;
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

        protected RadzenDataGrid<ReceiptDocumentDto> grid;
        protected bool isLoading;

        protected IEnumerable<ReceiptDocumentDto> receipts;
        protected int count;

        protected int pageSize = 20;
        protected readonly IEnumerable<int> pageSizeOptions = [10, 20, 50];

        /// <summary>
        /// Подгрузка элементов в гриде/таблице
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected async Task LoadData(LoadDataArgs args)
        {
            isLoading = true;

            var result = await StorageService.GetReceiptAsync(new FilterDto
            {
                Skip = args.Skip,
                Top = args.Top,
                Filter = args.Filter,
                OrderBy = args.OrderBy
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
        /// Реализация метода сброса всех фильтров
        /// </summary>
        /// <returns></returns>
        protected async Task ResetFiltersAsync()
        {
            foreach (var c in grid.ColumnsCollection)
            {
                c.ClearFilters();
            }

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

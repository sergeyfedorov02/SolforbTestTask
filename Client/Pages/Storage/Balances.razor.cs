using DataContracts;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using SolforbTestTask.Client.Services;

namespace SolforbTestTask.Client.Pages.Storage
{
    public partial class Balances
    {
        // inject - это внедрение зависимости(dependency injection) для компонентов asp.net core
        // чтобы самому не создавать элементы, а чтобы asp.net core сам их подставлял
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

        protected RadzenDataGrid<BalanceDto> grid;
        protected bool isLoading;

        protected IEnumerable<BalanceDto> balances;
        protected int count;

        protected int pageSize = 20;
        protected readonly IEnumerable<int> pageSizeOptions = [10, 20, 50];

        /// <summary>
        /// Загрузка значений фильтров по Resource и Measurement
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private async Task LoadColumnFilterData(DataGridLoadColumnFilterDataEventArgs<BalanceDto> args)
        {
            var columnProperty = args.Column.Property;
            var result = await StorageService.GetBalanceFiltersAsync(new FilterBalanceDto
            {
                ColumnName = columnProperty,
                Filter = args.Filter
            });

            if (result.Success)
            {
                args.Data = result.Data.Data;
                args.Count = result.Data.Count;
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

            var result = await StorageService.GetBalanceAsync(new FilterDto
            {
                Skip = args.Skip,
                Top = args.Top,
                Filter = args.Filter,
                OrderBy = args.OrderBy
            });

            if (result.Success)
            {
                count = result.Data.Count;
                balances = [.. result.Data.Data];
            }
            else
            {
                Logger.LogError(result.Exception, $"Ошибка при получении таблицы Balance (filter={args.Filter})");
                count = 0;
                balances = [];
            }

            isLoading = false;
            StateHasChanged();
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

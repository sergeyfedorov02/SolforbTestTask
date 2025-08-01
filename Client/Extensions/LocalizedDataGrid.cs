using Radzen.Blazor;

namespace SolforbTestTask.Client.Extensions
{
    /// <summary>
    /// Для русификации грида
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class LocalizedDataGrid<TItem> : RadzenDataGrid<TItem>
    {

        public LocalizedDataGrid()
        {
            AndOperatorText = "И";
            OrOperatorText = "ИЛИ";
            ApplyFilterText = "Применить";
            ClearFilterText = "Очистить";
            FilterText = "Фильтр";
            ContainsText = "Содержит";
            DoesNotContainText = "Не содержит";
            EqualsText = "Равен";
            NotEqualsText = "Не равен";
            StartsWithText = "Начинается с";
            EndsWithText = "Заканчивается на";
            GroupPanelText = "Перетащите сюда заголовок столбца для группировки по нему";
            LessThanOrEqualsText = "Меньше или равен";
            LessThanText = "Меньше";
            GreaterThanText = "Больше";
            GreaterThanOrEqualsText = "Больше или равен";
            EmptyText = "Запрос не вернул данных";
            PageSizeText = "записей на странице";
            AllColumnsText = "Все";
            ColumnsShowingText = "столбцов показывается";
            IsNullText = "Нет значения";
            IsNotNullText = "Есть значение";
            IsEmptyText = "Пустое значение";
            IsNotEmptyText = "Не пустое значение";
        }
    }
}

using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardPirates.Bot.V2
{
    internal class WarehouseService
    {
        
        public static WarehouseItem[] Items { get; private set; }

        public static void UpdateData() {
            try
            {
                List<List<string?>> values = GoogleSheetsService.GetValues(SheetType.Warehouse, "B:G");
                Items = ParseItems(values);
                Log.Information("Склад обновлен");
            }
            catch (Exception e)
            {
                Log.Error(e, "Ошибка при считывании склада");
            }
            
        }

        private static WarehouseItem[] ParseItems(List<List<string?>> values)
        {
            var statuses = SplitByStatus(values);
            return values
                .Select((value, index) => new { value, index })
                .Where(x => x.value.Count >= 5 && (!x.value[4]?.StartsWith("ГОТОВО К ВЫДАЧЕ") ?? false))
                .Select(x =>
                    new WarehouseItem(x.value,
                        GetStatus(statuses, x.index),
                        x.index
                    )
                ).ToArray();
        }

        private static string GetStatus(Dictionary<string, int> statuses, int index)
        {
            return statuses
                .Where(x => x.Value < index)
                .OrderByDescending(x => x.Value)
                .FirstOrDefault(new KeyValuePair<string, int>(String.Empty, 0))
                .Key;
        }

        private static Dictionary<string, int> SplitByStatus(List<List<string?>> values)
        {
            var firstCol = values.Select(x => !x.Any() ? String.Empty : x[0] ?? string.Empty).ToArray();
            return firstCol
                .Select((value, index) => new { value, index })
                .Where(pair => pair.value.StartsWith("===="))
                .ToDictionary(pair => pair.value, pair => pair.index);  
        }

        private static WarehouseItem[] Get(string tgContact)
        {
            return Items.Where(x => x.TGContact == tgContact).ToArray();
        }

        public static string PrepareTGMessage(string tgContact)
        {
            var rqs = Get(tgContact);
            if (!rqs.Any()) {
                return "На складе для вас ничего не обнаружено 😒";
            }
            var dict = rqs.GroupBy(x => x.Status)
                .ToDictionary(x => x.Key, x => x.ToArray())
                .Select(x =>
                    $"<b>{x.Key}:</b>" + Environment.NewLine +
                    String.Join("", x.Value.Select(y => $"📦 {y.Unit}" + Environment.NewLine))
                );
            return string.Join("", dict); 
        }
    }
}

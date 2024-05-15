using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardPirates.Bot.V2
{
    internal class KickOrderService
    {
        public static KickOrderItem[] Items { get; private set; }

        public static void UpdateData()
        {
            try { 
                List<List<string?>> values = GoogleSheetsService.GetValues(SheetType.KickOrder, "C:H");

                Items = ParseItems(values);
                Log.Information("КикЗаказно обновлен");
            }
            catch (Exception e)
            {
                Log.Error(e, "Ошибка при считывании КикЗаказано");
            }

        }

        private static KickOrderItem[] ParseItems(List<List<string?>> values)
        {
            return values
                .Select((value, index) => new { value, index })
                .Where(x => x.index > 4)
                .Where(x => x.value.Count > 3)
                .Select(x => new KickOrderItem(x.value, x.index))
                .ToArray();
        }

        private static KickOrderItem[] Get(string tgContact)
        {
            return Items.Where(x => x.TGContact == tgContact).ToArray();
        }

        public static string PrepareTGMessage(string tgContact) {
            var rqs = Get(tgContact)
                .GroupBy(x => x.Project)
                .ToDictionary(x => x.Key, x => x.ToArray())
                .Select(x =>
                    $"<b>🎲{x.Key}:</b>" + Environment.NewLine +
                    String.Join("", x.Value.Select(y => $"  ✅{y.Item}({y.Count})" + Environment.NewLine))
                );
            if (!rqs.Any()) {
                return "Вы не записались ни в один заказанный проект 😒";
            }
            return "Заказанные проекты:" + Environment.NewLine + string.Join("", rqs);
        }
    }
}

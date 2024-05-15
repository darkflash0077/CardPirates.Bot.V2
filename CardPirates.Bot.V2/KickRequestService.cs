using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardPirates.Bot.V2
{
    internal class KickRequestService
    {
        public static KickRequestItem[] Items { get; private set; }

        public static void UpdateData()
        {
            try { 
                List<List<string?>> values = GoogleSheetsService.GetValues(SheetType.KickRequest, "C:H");

                Items = ParseItems(values);
                Log.Information("КикЗапись обновлен");
            }
            catch (Exception e)
            {
                Log.Error(e, "Ошибка при считывании КакЗапись");
            }

        }

        private static KickRequestItem[] ParseItems(List<List<string?>> values)
        {
            return values
                .Select((value, index) => new { value, index })
                .Where(x => x.value.Count > 3)
                .Select(x => new KickRequestItem(x.value, x.index))
                .ToArray();
        }

        private static KickRequestItem[] Get(string tgContact)
        {
            return Items.Where(x => x.TGContact == tgContact).ToArray();
        }

        public static string PrepareTGMessage(string tgContact) {
            var rqs = Get(tgContact)
                .GroupBy(x => x.Project)
                .ToDictionary(x => x.Key, x => x.ToArray())
                .Select(x =>
                    $"<b>🎲{x.Key}:</b>" + Environment.NewLine +
                    String.Join("", x.Value.Select(y => $"  ✏️{y.Item}({y.Count})" + Environment.NewLine))
                );
            if (!rqs.Any()) {
                return "Вы не записались ни в один проект 😒";
            }
            return "Запись:" + Environment.NewLine + string.Join("", rqs);
        }
    }
}

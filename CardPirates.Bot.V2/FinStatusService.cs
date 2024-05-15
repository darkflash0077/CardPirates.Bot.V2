using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardPirates.Bot.V2
{
    internal class FinStatusService
    {
        public static FinStatusItem[] Items { get; private set; }

        public static void UpdateData()
        {
            try { 
            List<List<string?>> values = GoogleSheetsService.GetValues(SheetType.FinStatus, "A:G");

            Items = ParseItems(values);
            Log.Information("Финансы обновлены");
            }
            catch (Exception e)
            {
                Log.Error(e, "Ошибка при считывании Финансы");
            }

        }

        private static FinStatusItem[] ParseItems(List<List<string?>> values)
        {
            return values
                .Select((value, index) => new { value, index})
                .Where(x => x.index > 4)
                .Select(x => new FinStatusItem(x.value, x.index))
                .ToArray();
        }

        private static FinStatusItem[] Get(string tgContact)
        {
            return Items.Where(x => x.TGContact == tgContact).ToArray();
        }

        public static string PrepareTGMessage(string tgContact)
        {
            var rqs = Get(tgContact);
            if (rqs.Count() > 1) {
                return "Произошла внутренняя ошибка 😒. Обратитесь в поддержку.";
            }
            if (rqs.Count() == 0)
            {
                return "Возможно вам еще не присвое идентификатор.";
            }
            var fin = rqs.First();
            if (fin.PayAmount > 0)
            {
                return $@"Ваш долг составляет ровно {fin.PayAmount} рублей копеек." + Environment.NewLine +
                    "Платить ровно столько через СБП на номер 89199912889 на Тинькофф." + Environment.NewLine +
                    "Пожалуйста, не округляте копейки при оплате, они нужны для определения источника платежа!";
            }
            else {
                return $@"💪 В вашем сундуке {fin.PositiveBalance} рублей копеек."  + Environment.NewLine + Environment.NewLine +
                    "Вы ничего не должны капитану. Ваша пиратская честь чиста!";
            }
        }
    }
}

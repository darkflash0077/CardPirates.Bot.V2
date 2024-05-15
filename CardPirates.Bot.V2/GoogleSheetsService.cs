using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardPirates.Bot.V2
{
    internal class GoogleSheetsService
    {
        /// <summary>
        /// Словарь с маппингом типа таблицы и ее имени
        /// </summary>
        static Dictionary<SheetType, string> SheetsNames = new Dictionary<SheetType, string>() {
            { SheetType.FinStatus, "Финансы"},
            { SheetType.Warehouse, "Склад"},
            { SheetType.KickRequest, "КикЗапись"},
            { SheetType.KickOrder, "КикЗаказано"}
        };

        public static SheetsService Service { get; set; }
        static string SpreadsheetId { get; set; } = string.Empty;
        public static SpreadsheetsResource.ValuesResource Values => Service.Spreadsheets.Values;
        static GoogleSheetsService() {            
            Service = new SheetsService(new BaseClientService.Initializer()
            {
                ApplicationName = "CardPirates.Bot.V2",
                ApiKey = Environment.GetEnvironmentVariable("GoogleApiKey") 
            });
            SpreadsheetId = Environment.GetEnvironmentVariable("SpreadsheetId");
        }

        /// <summary>
        /// Обновить все таблицы
        /// </summary>
        public static void UpdateAll()
        {
            try
            {
                WarehouseService.UpdateData();
                FinStatusService.UpdateData();
                KickRequestService.UpdateData();
                KickOrderService.UpdateData();
            }
            catch (Exception e)
            {
                Log.Error("Ошибка", e);
                throw;
            }            
        }

        /// <summary>
        /// Получить данные из Google-таблицы ПКМ   
        /// </summary>
        /// <param name="sheetType">Тип запрашиваемых данных</param>
        /// <param name="range">Запрашиваемый диапазон</param>
        /// <returns></returns>
        public static List<List<string?>> GetValues(SheetType sheetType, string range) =>
            Values.Get(SpreadsheetId, $"{SheetsNames[sheetType]}!{range}").Execute().Values
            .Select(x => x.Select(y => Convert.ToString(y)).ToList()).ToList();            
    }
}

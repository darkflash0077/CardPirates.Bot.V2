// See https://aka.ms/new-console-template for more information

using CardPirates.Bot.V2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Serilog;
Log.Logger = new LoggerConfiguration()
       .WriteTo.Console()
       .CreateLogger();
GoogleSheetsService.UpdateAll();
var bot = new BotMain();
while (bot.State == State.Running)
{
    Thread.Sleep(TimeSpan.FromMinutes(5));
    GoogleSheetsService.UpdateAll();
    Console.WriteLine("HeartBit");
}
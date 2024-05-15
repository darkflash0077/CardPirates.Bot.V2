using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CardPirates.Bot.V2
{
    internal class BotMain
    {
        public State State { get; private set; }
        public static long admin;
        TelegramBotClient Bot;
        public BotMain() {
            var checkEnvVar = new string[] { "GoogleApiKey", "SpreadsheetId", "BotToken", "AdminTGId" }
                .Where(x => !Environment.GetEnvironmentVariables().Contains(x));
            if (checkEnvVar.Any())
            {
                Log.Error($"Нужны следущие переменные среды: {string.Join(", ", checkEnvVar)}");
                return;
            }
            admin = Convert.ToInt64(Environment.GetEnvironmentVariable("AdminTGId"));
            Bot = new TelegramBotClient(Environment.GetEnvironmentVariable("BotToken"));
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new UpdateType[] { UpdateType.Message }, 
            };
            Bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            State = State.Running;
        }

        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            SendLog($"Ошибка бота {Newtonsoft.Json.JsonConvert.SerializeObject(exception)}", true);
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                if (update.Message != null)
                {
                    await HandleMessage(update.Message);
                }
            }
            catch (Exception e) 
            {
                SendLog($"Ошибка бота {Newtonsoft.Json.JsonConvert.SerializeObject(e)}", true);
                State = State.Closed;
            }                      
        }

        private async Task HandleMessage(Message message)
        {
            if (string.IsNullOrEmpty(message.Chat.Username)) {
                await Bot.SendTextMessageAsync(message.Chat, "У вас скрыт логин");
                return;
            }
            string tgName = $"@{message.Chat.Username}";
            switch (message.Text)
            {
                case "/orders":
                    await Bot.SendTextMessageAsync(message.Chat, WarehouseService.PrepareTGMessage(tgName), parseMode: ParseMode.Html);
                    break;
                case "/balance":
                    await Bot.SendTextMessageAsync(message.Chat, FinStatusService.PrepareTGMessage(tgName), parseMode: ParseMode.Html);
                    break;
                case "/requests":
                    await Bot.SendTextMessageAsync(message.Chat, KickRequestService.PrepareTGMessage(tgName), parseMode: ParseMode.Html);
                    break;
                case "/booked":
                    await Bot.SendTextMessageAsync(message.Chat, KickOrderService.PrepareTGMessage(tgName), parseMode: ParseMode.Html);
                    break;
                default:
                    break;
            }
        }

        private void SendLog(string str, bool withAdminNotification = false)
        {
            Log.Information(str);
            if (withAdminNotification)
            {
                SendToAdmin(str);
            }
        }

        private void SendToAdmin(string str)
        {
            if (admin == 0) {
                Log.Warning("Идентификатор Админа не указан.");
                return;
            }
            Bot.SendTextMessageAsync(admin, str);
        }
    }
    internal enum State
    {
        Running,
        Closing,
        Closed
    }
}

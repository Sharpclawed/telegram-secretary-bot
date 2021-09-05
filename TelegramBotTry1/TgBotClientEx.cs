using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using TelegramBotTry1.Settings;

namespace TelegramBotTry1
{
    public class TgBotClientEx : TelegramBotClient, ITgBotClientEx
    {
        public TgBotClientEx(string token, HttpClient httpClient = null) : base(token, httpClient)
        {
        }

        public TgBotClientEx(string token, IWebProxy webProxy) : base(token, webProxy)
        {
        }

        public async Task SendTextMessagesAsListAsync(ChatId chatId, IList<string> msgs, СorrespondenceType сorrespondenceType)
        {
            var totalMessagesLimit = сorrespondenceType == СorrespondenceType.Personal 
                ? TgBotSettings.SendLimitForAllChatsPerSecond
                : TgBotSettings.SendLimitForOneChatPerMinute;
            var preparedSet = msgs.Take(totalMessagesLimit);

            foreach (var msg in preparedSet)
                await SendTextMessageAsync(chatId, msg);
        }

        public async Task SendTextMessagesAsSingleTextAsync(ChatId chatId, IEnumerable<string> msgs, string caption, ParseMode parseMode = ParseMode.Default, bool removeLinkPreview = false)
        {
            var result = string.Join("\r\n", msgs);
            await SendTextMessageAsync(chatId, $"{caption}\r\n{result}", parseMode, removeLinkPreview);
        }
        
        public async Task SendTextMessagesAsExcelReportAsync<T>(ChatId chatId, List<T> msgs, string caption = null)
        {
            var sheetsWithRows = msgs.ToLookup(_ => caption ?? "Лист1");
            await SendTextMessagesAsExcelReportAsync(chatId, sheetsWithRows, caption);
        }

        public async Task SendTextMessagesAsExcelReportAsync<T>(ChatId chatId, ILookup<string, T> rowsWithSheets, string caption = null)
        {
            await using var fileStream = ReportCreator.Create(rowsWithSheets);
            var fileToSend = new InputOnlineFile(fileStream, (caption ?? "report") + ".xls");
            await SendDocumentAsync(chatId, fileToSend, caption);
        }
    }

    public enum СorrespondenceType
    {
        Chat,
        Personal
    }

    public interface ITgBotClientEx : ITelegramBotClient
    {
        Task SendTextMessagesAsListAsync(ChatId chatId, IList<string> msgs, СorrespondenceType сorrespondenceType);
        Task SendTextMessagesAsSingleTextAsync(ChatId chatId, IEnumerable<string> msgs, string caption, ParseMode parseMode = ParseMode.Default, bool removeLinkPreview = false);
        Task SendTextMessagesAsExcelReportAsync<T>(ChatId chatId, List<T> msgs, string caption = null);
        Task SendTextMessagesAsExcelReportAsync<T>(ChatId chatId, ILookup<string, T> rowsWithSheets, string caption = null);
    }
}
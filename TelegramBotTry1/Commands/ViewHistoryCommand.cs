using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotTry1.DataProviders;
using TelegramBotTry1.Domain;

namespace TelegramBotTry1.Commands
{
    public class ViewHistoryCommand : IBotCommand
    {
        private readonly ITgBotClientEx tgClient;
        private readonly ChatId chatId;
        public DateTime Begin { get; }
        public DateTime End { get; }
        public string ChatName { get; }

        public ViewHistoryCommand(ITgBotClientEx tgClient, ChatId chatId, DateTime begin, DateTime end, string chatName)
        {
            this.tgClient = tgClient;
            this.chatId = chatId;
            Begin = begin;
            End = end;
            ChatName = chatName;
        }

        public async Task ProcessAsync()
        {
            var result = HistoryProvider.GetRows(Begin, End, ChatName, null);

            if (result.Error != null)
            {
                await tgClient.SendTextMessageAsync(chatId, result.Error);
                return;
            }

            await tgClient.SendTextMessagesAsExcelReportAsync(
                chatId,
                result.Messages,
                result.Caption,
                new[]
                {
                    nameof(IMessageDataSet.Date),
                    nameof(IMessageDataSet.Message),
                    nameof(IMessageDataSet.UserFirstName),
                    nameof(IMessageDataSet.UserLastName),
                    nameof(IMessageDataSet.UserName),
                    nameof(IMessageDataSet.UserId)
                },
                msg => msg.ChatName);
        }
    }
}
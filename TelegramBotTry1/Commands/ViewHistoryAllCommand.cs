using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotTry1.DataProviders;
using TelegramBotTry1.Domain;

namespace TelegramBotTry1.Commands
{
    public class ViewHistoryAllCommand : IBotCommand
    {
        private readonly ITgBotClientEx tgClient;
        private readonly ChatId chatId;
        public DateTime Begin { get; }
        public DateTime End { get; }

        public ViewHistoryAllCommand(ITgBotClientEx tgClient, ChatId chatId, DateTime begin, DateTime end)
        {
            this.tgClient = tgClient;
            this.chatId = chatId;
            Begin = begin;
            End = end;
        }

        public async Task ProcessAsync()
        {
            var result = HistoryProvider.GetRows(Begin, End, null, null);

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
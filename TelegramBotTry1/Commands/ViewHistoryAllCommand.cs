using System;
using System.Threading.Tasks;
using DAL.Models;
using Telegram.Bot.Types;
using TelegramBotTry1.DataProviders;

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
                    nameof(MessageDataSet.Date),
                    nameof(MessageDataSet.Message),
                    nameof(MessageDataSet.UserFirstName),
                    nameof(MessageDataSet.UserLastName),
                    nameof(MessageDataSet.UserName),
                    nameof(MessageDataSet.UserId)
                },
                msg => msg.ChatName);
        }
    }
}
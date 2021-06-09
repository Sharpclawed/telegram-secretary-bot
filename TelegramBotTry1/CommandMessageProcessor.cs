using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotTry1.Commands;
using TelegramBotTry1.DataProviders;
using TelegramBotTry1.Domain;

namespace TelegramBotTry1
{
    public static class CommandMessageProcessor
    {
        public static async Task ProcessTextMessageAsync(ITelegramBotClientAdapter botClient, Message message)
        {
            var isMessagePersonal = message.Chat.Title == null;
            if (!isMessagePersonal)
                return;

            if (message.Text.StartsWith("/help"))
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, TipProvider.GetHelpTip(), ParseMode.Markdown);
                return;
            }

            var isAdminAsking = DbRepository.IsAdmin(message.From.Id);
            if (!isAdminAsking)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "У вас не хватает прав");
                return;
            }

            IBotCommand command;
            try
            {
                command = CommandDetector.Parse(message.Text);
            }
            catch (InvalidCastException)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Неизвестная команда");
                return;
            }

            var result = command.Process();
            if (result.Error != null)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, result.Error);
                return;
            }

            switch (command)
            {
                case ViewAdminsCommand _:
                case ViewBkCommand _:
                case ViewOneTimeChatsCommand _:
                {
                    await botClient.SendTextMessagesAsSingleTextAsync(message.Chat.Id, result.Records, result.Caption);
                    break;
                }
                case ViewWaitersCommand _:
                {
                    var formattedRecords = result.Messages.Select(Formatter.Waiters).ToList();
                    await botClient.SendTextMessagesAsListAsync(message.Chat.Id, formattedRecords, ChatType.Personal);
                    break;
                }
                case ViewInactiveChatsCommand _:
                {
                    await botClient.SendTextMessagesAsExcelReportAsync(
                        message.Chat.Id,
                        result.Messages,
                        result.Caption,
                        new[]
                        {
                            nameof(IMessageDataSet.Date),
                            nameof(IMessageDataSet.ChatName),
                            nameof(IMessageDataSet.Message),
                            nameof(IMessageDataSet.UserFirstName),
                            nameof(IMessageDataSet.UserLastName),
                            nameof(IMessageDataSet.UserName),
                            nameof(IMessageDataSet.UserId)
                        });
                    break;
                }
                case AddAdminCommand _:
                case AddBkCommand _:
                case AddOnetimeChatCommand _:
                case RemoveAdminCommand _:
                case RemoveBkCommand _:
                case RemoveOnetimeChatCommand _:
                    await botClient.SendTextMessageAsync(message.Chat.Id, result.Message);
                    break;
                case ViewHistoryCommand _:
                case ViewHistoryOfCommand _:
                case ViewHistoryAllCommand _:
                    await botClient.SendTextMessagesAsExcelReportAsync(
                        message.Chat.Id,
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
                    break;
            }
        }
    }
}
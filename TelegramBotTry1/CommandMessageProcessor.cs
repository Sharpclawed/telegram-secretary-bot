using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotTry1.DataProviders;
using TelegramBotTry1.Domain;
using TelegramBotTry1.Dto;
using TelegramBotTry1.Enums;

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

            if (message.Text.StartsWith("/history"))
            {
                var command = new HistoryCommand(message.Text);
                var isAdminAsking = DbRepository.IsAdmin(message.From.Id);
                if (!isAdminAsking)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "У вас не хватает прав");
                    return;
                }

                var historyResult = CommandProcessor.ProcessHistoryCommand(command);

                if (historyResult.Error != null)
                    await botClient.SendTextMessageAsync(message.Chat.Id, historyResult.Error);
                else
                    await botClient.SendTextMessagesAsExcelReportAsync(
                        message.Chat.Id,
                        historyResult.Records,
                        historyResult.Caption,
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
            else if (message.Text.StartsWith("/add") || message.Text.StartsWith("/remove"))
            {
                var command = new ManagingCommand(message.Text, message.From.Id, message.From.Username);
                if (command.ManagingType == ManagingType.Unknown || command.EntityType == EntityType.Unknown)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Неизвестная команда");
                    return;
                }

                var isAdminAsking = DbRepository.IsAdmin(message.From.Id);
                if (!isAdminAsking)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "У вас не хватает прав");
                    return;
                }

                var processResult = CommandProcessor.ProcessSetDataCommand(command);
                if (processResult.Error != null)
                    await botClient.SendTextMessageAsync(message.Chat.Id, processResult.Error);
                else
                    await botClient.SendTextMessageAsync(message.Chat.Id, processResult.Message);
            }
            else if (message.Text.StartsWith("/view"))
            {
                var command = new ManagingCommand(message.Text);
                if (command.ManagingType == ManagingType.Unknown || command.EntityType == EntityType.Unknown)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Неизвестная команда");
                    return;
                }

                var isAdminAsking = DbRepository.IsAdmin(message.From.Id);
                if (!isAdminAsking)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "У вас не хватает прав");
                    return;
                }

                switch (command.EntityType)
                {
                    case EntityType.Admin:
                    {
                        var result = CommandProcessor.ProcessViewAdminsList();
                        if (result.Error != null)
                            await botClient.SendTextMessageAsync(message.Chat.Id, result.Error);
                        else
                            await botClient.SendTextMessagesAsSingleTextAsync(message.Chat.Id, result.Records, result.Caption);
                        break;
                    }
                    case EntityType.Bookkeeper:
                    {
                        var result = CommandProcessor.ProcessViewBookkeepersList();
                        if (result.Error != null)
                            await botClient.SendTextMessageAsync(message.Chat.Id, result.Error);
                        else
                            await botClient.SendTextMessagesAsSingleTextAsync(message.Chat.Id, result.Records, result.Caption);
                        break;
                    }
                    case EntityType.Waiter:
                    {
                        var result = CommandProcessor.ProcessViewWaiters();
                        if (result.Error != null)
                            await botClient.SendTextMessageAsync(message.Chat.Id, result.Error);
                        else
                        {
                            var formattedRecords = result.Records.Select(Formatter.Waiters).ToList();
                            await botClient.SendTextMessagesAsListAsync(message.Chat.Id, formattedRecords, ChatType.Personal);
                        }
                        break;
                    }
                    case EntityType.InactiveChatException:
                    {
                        var result = CommandProcessor.ProcessViewInactiveChatExceptions();
                        if (result.Error != null)
                            await botClient.SendTextMessageAsync(message.Chat.Id, result.Error);
                        else
                            await botClient.SendTextMessagesAsSingleTextAsync(message.Chat.Id, result.Records, result.Caption);
                        break;
                    }
                    case EntityType.InactiveChat:
                    {
                        var result = CommandProcessor.ProcessViewInactiveChats();

                        if (result.Error != null)
                            await botClient.SendTextMessageAsync(message.Chat.Id, result.Error);
                        else
                            await botClient.SendTextMessagesAsExcelReportAsync(
                                message.Chat.Id,
                                result.Records,
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
                }
            }
        }
    }
}
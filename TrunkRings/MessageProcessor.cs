using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using TrunkRings.Domain.Models;
using TrunkRings.Domain.Services;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TrunkRings.Commands;
using TrunkRings.Settings;

namespace TrunkRings
{
    public class MessageProcessor
    {
        private readonly ITgBotClientEx tgClient;
        private readonly IAdminService adminService;
        private readonly IMessageService messageService;
        private readonly ILogger logger;
        private readonly CommandDetector commandDetector;

        public MessageProcessor(ITgBotClientEx tgClient, IAdminService adminService, IBkService bkService,
            IOneTimeChatService oneTimeChatService, IMessageService messageService, ILogger logger)
        {
            this.tgClient = tgClient;
            this.adminService = adminService;
            this.messageService = messageService;
            this.logger = logger;
            commandDetector = new CommandDetector(tgClient, adminService, bkService, oneTimeChatService, messageService);
        }

        public async Task ProcessMessageAsync(Message message)
        {
            try
            {
                SaveToDatabase(message);

                switch (message.Type)
                {
                    case MessageType.Text:
                        await ProcessTextMessage(message);
                        break;
                    case MessageType.ChatMembersAdded:
                        await ProcessChatMembersAdded(message);
                        break;
                }
            }
            catch (Exception exception)
            {
                logger.LogError(exception.ToString());
                switch (exception)
                {
                    case SocketException _:
                    case ObjectDisposedException _:
                        await tgClient.SendTextMessageAsync(ChatIds.Debug,
                            "Пропала коннекция к базе. Отключаюсь, чтобы не потерялись данные. mr\r\n"
                            + "Пожалуйста, включите меня в течение суток");
                        throw;
                    default:
                        await tgClient.SendTextMessageAsync(ChatIds.Debug, exception.ToString());
                        break;
                }
            }
        }

        private async Task ProcessChatMembersAdded(Message message)
        {
            var chatToReport = ChatIds.LogDistributing;
            var newMembers = message.NewChatMembers;
            var command = new CheckAddedMemberCommand(tgClient, chatToReport, newMembers, message.Chat);
            await command.ProcessAsync();
        }

        private async Task ProcessTextMessage(Message message)
        {
            var isCommand = message.Text.First() == '/';
            var isMessagePersonal = message.Chat.Title == null;
            if (!isCommand || !isMessagePersonal)
                return;

            var isAdminAsking = adminService.IfAdmin(message.From.Id);
            if (!isAdminAsking)
            {
                await tgClient.SendTextMessageAsync(message.Chat.Id, "У вас не хватает прав");
                return;
            }

            var command = commandDetector.Parse(message);
            await command.ProcessAsync();
        }

        private void SaveToDatabase(Message message)
        {
            //todo automapper
            var recievedDataSet = new DomainMessage
            {
                MessageId = message.MessageId,
                Date = message.Date,
                UserName = message.From.Username,
                UserFirstName = message.From.FirstName,
                UserLastName = message.From.LastName,
                UserId = message.From.Id,
                ChatId = message.Chat.Id,
                ChatName = message.Chat.Title,
                Message = message.Type switch
                {
                    MessageType.Text => message.Text,
                    MessageType.Sticker => message.Sticker.Emoji,
                    MessageType.Contact => message.Contact.FirstName + " " + message.Contact.LastName + " (" +
                                           message.Contact.UserId + "): " + message.Contact.PhoneNumber,
                    _ => "MessageType: " + message.Type
                }
            };

            messageService.Save(recievedDataSet);
        }
    }
}
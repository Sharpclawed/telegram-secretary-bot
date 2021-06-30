﻿using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotTry1.Domain;

namespace TelegramBotTry1.Commands
{
    public class AddOnetimeChatCommand : IBotCommand
    {
        private readonly ITgBotClientEx tgClient;
        private readonly ChatId chatId;
        public string ChatName { get; }

        public AddOnetimeChatCommand(ITgBotClientEx tgClient, ChatId chatId, string chatName)
        {
            this.tgClient = tgClient;
            this.chatId = chatId;
            ChatName = chatName;
        }

        public async Task ProcessAsync()
        {
            using (var context = new MsgContext())
            {
                var messageDataSets = context.Set<MessageDataSet>().AsNoTracking();
                var onetimeChatDataSets = context.Set<OnetimeChatDataSet>();
                var chat = messageDataSets.GetChatByChatName(ChatName);
                if (!onetimeChatDataSets.Any(x => x.ChatId == chat.Id))
                {
                    onetimeChatDataSets.Add(new OnetimeChatDataSet
                    {
                        ChatName = chat.Name,
                        ChatId = chat.Id
                    });
                    context.SaveChanges();
                }
            }

            var result = "Команда обработана";
            await tgClient.SendTextMessageAsync(chatId, result);
        }
    }
}
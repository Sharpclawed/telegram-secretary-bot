using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TrunkRings.Commands
{
    class SendHelpTipCommand : IBotCommand
    {
        private readonly ITgBotClientEx tgClient;
        private readonly ChatId chatId;

        public SendHelpTipCommand(ITgBotClientEx tgClient, ChatId chatId)
        {
            this.tgClient = tgClient;
            this.chatId = chatId;
        }

        public async Task ProcessAsync()
        {
            var helpTip = new StringBuilder();
            helpTip.AppendLine("*Получить историю:*")
                .AppendLine("/history: \"Название чата\" \"Дата начала\" \"Кол-во дней\" — _выдает в Эксель-файл историю переписки в указанном чате с даты начала на количество дней вперед_")
                       .AppendLine("/historyall: \"Дата начала\" \"Кол-во дней\" — _выдает выдает в Эксель-файл историю переписки всех частов, где есть бот с даты начала на количество дней вперед_")
                       .AppendLine("/historyof: \"id аккаунта\" \"Дата начала\" \"Кол-во дней\" — _выдает в Эксель-файл историю переписки по чатам бухгалтера с указанным id аккаунта с даты начала на количество дней вперед_")
                       .AppendLine()
                       .AppendLine("*Работа с админами:*")
                       .AppendLine("/addadmin: \"username\" — _добавить пользователя телеграмм в админы чат-бота - добавляем по юзернейм, могут запускать только админы_")
                       .AppendLine("/removeadmin: \"username\" — _убрать пользователя телеграмм из админов чат-бота, убрать по юзернейм, могут запускать только админы_")
                       .AppendLine("/viewadmins — _выводит список пользователей телеграмм, которые являются админами чат-бота, могут запускать только админы_")
                       .AppendLine()
                       .AppendLine("*Работа с бухгалтерами:*")
                       .AppendLine("/addbk: \"username\" — _добавить пользователя телеграмм в бухгалтеры для  чат-бота - добавляем по юзернейм, команду запускает админ. Нужно обозначить бухгалтеров, чтоб неотвеченные сообщения только от директоров проверять_")
                       .AppendLine("/removebk: \"username\" — _убрать пользователя телеграмм из бухгалтеров для  чат-бота - добавляем по юзернейм, команду запускает админ_")
                       .AppendLine("/viewbk — _выводит список пользователей телеграмм, которые помеченные  бухгалтерами для чат-бота, могут запускать только админы_")
                       .AppendLine("/viewwaiters — _выдает в чат список чатов, где есть неотвеченные больше 2 часов сообщения от директора_")
                       .AppendLine()
                       .AppendLine("*Работа с чатами:*")
                       .AppendLine("/addonetimechat: \"chatname\" — _пометить чат как неактивный, по которому не надо контролировать молчание директора в течении недели_")
                       .AppendLine("/removeonetimechat: \"chatname\" — _убрать пометку неактивности с чата_")
                       .AppendLine("/viewonetimechats — _вывести список чатов, помеченными неактивными_")
                       .AppendLine("/viewinactivechats — _вывести в Эксель список чатов, в которых не было сообщений от директора больше недели_")
                       .AppendLine("/getchatid \"chatname\" — _получить id чата по названию_")
                   ;

            await tgClient.SendTextMessageAsync(chatId, helpTip.ToString(), ParseMode.Markdown);
        }
    }
}
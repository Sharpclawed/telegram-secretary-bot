using System.Text.RegularExpressions;
using TelegramBotTry1.Enums;

namespace TelegramBotTry1.Dto
{
    //TODO Should be different commands
    public class ManagingCommand
    {
        public ManagingType ManagingType { get; }
        public EntityType EntityType { get; }
        public string EntityName { get; }
        public int? UserId { get; }
        public string UserName { get; }

        public ManagingCommand(string messageText, int? userId = null, string userName = null)
        {
            //двоеточие не обязательное
            var regex = new Regex(@"^[/](add|remove|delete|view)(admins|admin|bk|waiters|inactivechats|onetimechats|onetimechat)\s*(.*)$");
            var match = regex.Match(messageText);
            if (match == Match.Empty)
            {
                ManagingType = ManagingType.Unknown;
                return;
            }

            switch (match.Groups[1].Value)
            {
                case "add":
                    ManagingType = ManagingType.Add;
                    break;
                case "remove":
                case "delete":
                    ManagingType = ManagingType.Remove;
                    break;
                case "view":
                    ManagingType = ManagingType.ViewList;
                    break;
                default:
                    ManagingType = ManagingType.Unknown;
                    break;
            }

            switch (match.Groups[2].Value)
            {
                case "admin":
                case "admins":
                    EntityType = EntityType.Admin;
                    break;
                case "bk":
                    EntityType = EntityType.Bookkeeper;
                    break;
                case "waiters":
                    EntityType = EntityType.Waiter;
                    break;
                case "inactivechats":
                    EntityType = EntityType.InactiveChat;
                    break;
                case "onetimechats":
                case "onetimechat":
                    EntityType = EntityType.InactiveChatException;
                    break;
                default:
                    EntityType = EntityType.Unknown;
                    break;
            }

            EntityName = match.Groups[3].Value.TrimStart(':', ' ');
            UserId = userId;
            UserName = userName;
        }
    }
}
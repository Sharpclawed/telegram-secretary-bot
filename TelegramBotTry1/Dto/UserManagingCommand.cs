using System.Text.RegularExpressions;
using TelegramBotTry1.Enums;

namespace TelegramBotTry1.Dto
{
    public class UserManagingCommand
    {
        public string UserUserName { get; }
        public ManagingType ManagingType { get; }
        public UserEntityType UserType { get; }

        public UserManagingCommand(string messageText)
        {
            var regex = new Regex(@"^[/](add|remove|delete)(admin|bk)[:]\s*(.*)$");
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
                default:
                    ManagingType = ManagingType.Unknown;
                    break;
            }

            switch (match.Groups[2].Value)
            {
                case "admin":
                    UserType = UserEntityType.Admin;
                    break;
                case "bk":
                    UserType = UserEntityType.Bookkeeper;
                    break;
                default:
                    UserType = UserEntityType.Unknown;
                    break;
            }

            UserUserName = match.Groups[3].Value;
        }
    }
}
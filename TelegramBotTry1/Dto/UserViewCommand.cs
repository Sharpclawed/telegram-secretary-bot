using System.Text.RegularExpressions;
using TelegramBotTry1.Enums;

namespace TelegramBotTry1.Dto
{
    public class UserViewCommand
    {
        public ManagingType ManagingType { get; }
        public UserType UserType { get; }

        public UserViewCommand(string messageText)
        {
            var regex = new Regex(@"^[/](view)(admins|bk)$");
            var match = regex.Match(messageText);
            if (match == Match.Empty)
            {
                ManagingType = ManagingType.Unknown;
                return;
            }

            switch (match.Groups[1].Value)
            {
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
                    UserType = UserType.Admin;
                    break;
                case "bk":
                    UserType = UserType.Bookkeeper;
                    break;
                default:
                    UserType = UserType.Unknown;
                    break;
            }
        }
    }
}
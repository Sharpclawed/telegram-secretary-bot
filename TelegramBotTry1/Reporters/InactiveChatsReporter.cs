using System;
using System.Net.Sockets;
using System.Timers;
using TelegramBotTry1.Commands;
using TelegramBotTry1.Domain;

namespace TelegramBotTry1.Reporters
{
    public class InactiveChatsReporter : IReporter
    {
        private readonly ITelegramBotClientAdapter botClient;
        private Timer timer;
        private DateTime lastInactiveChatCheckUtc = DateTime.UtcNow.Date;

        public InactiveChatsReporter(ITelegramBotClientAdapter botClient)
        {
            this.botClient = botClient;
            Init();
        }

        private void Init()
        {
            timer = new Timer
            {
                Interval = 1000 * 60 * 5 //5 minutes
            };
            timer.Elapsed += ViewInactiveChatsAsync;
            timer.AutoReset = true;
        }

        public void Start()
        {
            timer.Start();
        }

        private async void ViewInactiveChatsAsync(object sender, ElapsedEventArgs e)
        {
            try
            {
                var scheduledRunUtc = DateTime.UtcNow.Date.AddHours(4); //9 часов по-нашему
                if (DateTime.UtcNow > scheduledRunUtc
                    && scheduledRunUtc.Date > lastInactiveChatCheckUtc.Date
                    && scheduledRunUtc.DayOfWeek == DayOfWeek.Sunday)
                {
                    var sinceDate = scheduledRunUtc.AddDays(-28);
                    var untilDate = scheduledRunUtc;
                    var command = new ViewInactiveChatsCommand(sinceDate, untilDate);
                    var result = command.Process();

                    await botClient.SendTextMessagesAsExcelReportAsync(
                        ChatIds.Unanswered,
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
                    lastInactiveChatCheckUtc = scheduledRunUtc;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
                switch (exception)
                {
                    case SocketException _:
                    case ObjectDisposedException _:
                        await botClient.SendTextMessageAsync(ChatIds.Botva, "Пропала коннекция к базе. Отключаюсь, чтобы не потерялись данные. vic\r\n"
                                                                    + "Пожалуйста, включите меня в течение суток");
                        throw;
                    default:
                        await botClient.SendTextMessageAsync(ChatIds.Test125, exception.ToString());
                        break;
                }
            }
        }
    }
}
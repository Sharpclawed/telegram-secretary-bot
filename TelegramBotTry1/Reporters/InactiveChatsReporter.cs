using System;
using System.Net.Sockets;
using System.Timers;
using TelegramBotTry1.Commands;

namespace TelegramBotTry1.Reporters
{
    public class InactiveChatsReporter : IReporter
    {
        private readonly ITgBotClientEx tgClient;
        private readonly BotCommander botCommander;
        private Timer timer;
        private DateTime lastInactiveChatCheckUtc = DateTime.UtcNow.Date;

        public InactiveChatsReporter(ITgBotClientEx tgClient, BotCommander botCommander)
        {
            this.tgClient = tgClient;
            this.botCommander = botCommander;
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
                    var command = new ViewInactiveChatsCommand(tgClient, ChatIds.Unanswered, sinceDate, untilDate);
                    await command.ProcessAsync();

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
                        await botCommander.SendMessageAsync(ChatIds.Botva, "Пропала коннекция к базе. Отключаюсь, чтобы не потерялись данные. vic\r\n"
                                                                           + "Пожалуйста, включите меня в течение суток");
                        throw;
                    default:
                        await botCommander.SendMessageAsync(ChatIds.Test125, exception.ToString());
                        break;
                }
            }
        }
    }
}
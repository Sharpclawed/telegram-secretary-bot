using System;
using System.Linq;
using DAL;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using TelegramBotTry1.DomainExtensions;
using TelegramBotTry1.Dto;

namespace TelegramBotTry1.DataProviders
{
    public static class HistoryProvider
    {
        public static CommandResult GetRows(DateTime begin, DateTime end, string exactChatName, long? exactUserId)
        {
            using (var context = new SecretaryContext())
            {
                var messageDataSets = context.Set<MessageDataSet>().AsNoTracking()
                    .GetActualDates(begin, end)
                    .GetActualChats(exactChatName)
                    .GetActualUser(exactUserId)
                    .ToList();

                if (!messageDataSets.Any())
                    return new CommandResult { Error = "За указанный период сообщений не найдено" };

                return new CommandResult { Messages = messageDataSets, Caption = "История сообщений" };
            }
        }
    }
}
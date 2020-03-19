using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TelegramBotTry1;
using FluentAssertions;

namespace FunctionalTests
{
    [TestFixture]
    public class DataSetExtensionsTest
    {
        private List<IMessageDataSet> dataSet;

        [SetUp]
        public void SetUp()
        {
            dataSet = new List<IMessageDataSet>
            {
                GetMessage(1, "Чат^1", new DateTime(2017, 1, 10), 1, 1, "Message 1.1"),
                GetMessage(1, "Чат^1", new DateTime(2017, 1, 2), 2, 2, "Message 1.2"),
                GetMessage(1, "Чат^1", new DateTime(2017, 1, 12), 3, 3, "Message 1.3"),
                GetMessage(2, "Chat2", new DateTime(2017, 1, 3), 1, 4, "Message 2.1"),
                GetMessage(2, "Chat2", new DateTime(2017, 1, 7), 2, 5, "Message 2.2"),
                GetMessage(3, "Chat3", new DateTime(2017, 1, 5), 1, 6, "Message 3.1")
            };
        }

        [Test]
        public void GetActualDatesTest()
        {
            var config = new HistoryCommandConfig("/historyof: 118274261 03.01.2017 3");
            var sut = dataSet.AsQueryable()
                .GetActualDates(config)
                .Select(x => x.Date).ToArray();
            sut.ShouldBeEquivalentTo(new[] {new DateTime(2017, 1, 3), new DateTime(2017, 1, 5)});
        }

        [Test]
        public void GetActualChatsTest()
        {
            dataSet.Add(GetMessage(4, "Чат^1", new DateTime(2017, 1, 1), 1, 4, "Message 4.1"));
            dataSet.Add(GetMessage(1, "Чат^4", new DateTime(2017, 1, 11), 1, 7, "Message 1.4"));

            var config = new HistoryCommandConfig("/history: Чат^1 01.01.2017 12");
            var sut = dataSet.AsQueryable()
                .GetActualChats(config)
                .Select(x => x.Message).ToArray();
            //sut.ShouldBeEquivalentTo(new[] { "Message 1.1", "Message 1.2", "Message 1.3", "Message 1.4" });
            sut.ShouldBeEquivalentTo(new[] { "Message 1.1", "Message 1.2", "Message 1.3", "Message 4.1" });
        }

        [Test]
        public void GetActualUserTest()
        {
            var config = new HistoryCommandConfig("/historyof: 2 01.01.2017 10");
            var sut = dataSet.AsQueryable()
                .GetActualUser(config);
            sut.All(x => new long[] {1, 2}.Contains(x.ChatId)).Should().BeTrue();
            sut.Count(x => x.ChatId == 2).Should().Be(2);
        }

        private IMessageDataSet GetMessage(long chatId, string chatName, DateTime date, long userId, long messageId, string message)
        {
            return new MessageDataSet
            {
                MessageDataSetId = Guid.NewGuid(),
                ChatId = chatId,
                ChatName = chatName,
                Date = date,
                UserId = userId,
                MessageId = messageId,
                Message = message
            };
        }
    }
}

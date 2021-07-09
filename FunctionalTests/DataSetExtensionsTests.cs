using System;
using System.Collections.Generic;
using System.Linq;
using DAL.Models;
using Domain;
using NUnit.Framework;
using FluentAssertions;
using TelegramBotTry1.DomainExtensions;

namespace FunctionalTests
{
    [TestFixture]
    public class DataSetExtensionsTests
    {
        //todo should be isolated
        private List<MessageDataSet> dataSet;

        [SetUp]
        public void SetUp()
        {
            dataSet = new List<MessageDataSet>
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
            var sut = dataSet.AsQueryable()
                .GetActualDates(DateTime.Parse("03.01.2017"), DateTime.Parse("06.01.2017"))
                .Select(x => x.Date).ToArray();
            sut.Should().BeEquivalentTo(new[] {new DateTime(2017, 1, 3), new DateTime(2017, 1, 5)});
        }

        [Test]
        public void GetActualDatesTest2()
        {
            var sut = dataSet.AsQueryable()
                .GetActualDates(DateTime.Parse("03.01.2017"), DateTime.Parse("06.01.2017"))
                .Select(x => x.Date).ToArray();
            sut.Should().BeEquivalentTo(new[] { new DateTime(2017, 1, 3), new DateTime(2017, 1, 5) });
        }

        [Test]
        public void GetActualChatsTest()
        {
            dataSet.Add(GetMessage(4, "Чат^1", new DateTime(2017, 1, 1), 1, 4, "Message 4.1"));
            dataSet.Add(GetMessage(1, "Чат^4", new DateTime(2017, 1, 11), 1, 7, "Message 1.4"));

            var sut = dataSet.AsQueryable()
                .GetActualChats("Чат^1")
                .Select(x => x.Message).ToArray();
            //sut.ShouldBeEquivalentTo(new[] { "Message 1.1", "Message 1.2", "Message 1.3", "Message 1.4" });
            sut.Should().BeEquivalentTo(new[] { "Message 1.1", "Message 1.2", "Message 1.3", "Message 4.1" });
        }

        [Test]
        public void GetActualUserTest()
        {
            var sut = dataSet.AsQueryable()
                .GetActualUser(2);
            sut.All(x => new long[] {1, 2}.Contains(x.ChatId)).Should().BeTrue();
            sut.Count(x => x.ChatId == 2).Should().Be(2);
        }

        [Test]
        public void GetUserByUserName_getsIfExists()
        {
            var sut = dataSet.AsQueryable()
                .GetUserByUserName("user 2 user name");

            sut.UserId.Should().Be(2);
        }

        [Test]
        public void GetUserByUserName_throwsIfNotExists()
        {
            Assert.Throws<ArgumentException>(() => dataSet.AsQueryable()
                .GetUserByUserName("incorrect name"));
        }

        [Test]
        public void GetChatByChatName_getsIfExists()
        {
            var sut = dataSet.AsQueryable()
                .GetChatByChatName("Chat2");

            sut.Id.Should().Be(2);
        }

        [Test]
        public void GetChatByChatName_throwsIfNotExists()
        {
            Assert.Throws<ArgumentException>(() => dataSet.AsQueryable()
                .GetChatByChatName("incorrect name"));
        }

        private MessageDataSet GetMessage(long chatId, string chatName, DateTime date, long userId, long messageId, string message)
        {
            return new MessageDataSet
            {
                MessageDataSetId = Guid.NewGuid(),
                ChatId = chatId,
                ChatName = chatName,
                Date = date,
                UserId = userId,
                UserFirstName = "user " + userId + " first name",
                UserLastName = "user " + userId + " last name",
                UserName = "user " + userId + " user name",
                MessageId = messageId,
                Message = message
            };
        }

        [Test]
        public void ObviouslySuperfluousFilterTest()
        {
            var testCases = new List<string>
            {
                "Ааа, понятно",
                "Ага ... спсб!",
                "Айгуль, добрый день. Оплатил. Спасибо",
                "Анна, добрый день! Хорошо",
                "вроде бы все хорошо",
                "Все подписал, спасибо",
                "Все понял. Спасибо.",
                "Готово",
                "Громадное спасибо! Очень приятно!))",
                "Да, пришлю",
                "Да, точно",
                "Да, хорошо!",
                "Доброе утро, благодарю!",
                "Доброе утро, Гузель. Спасибо",
                "Доброе утро! спасибо большое",
                "Добрый! Взаимно!",
                "Добрый день, Марина. Хорошо.",
                "Добрый день, хорошо",
                "Добрый день,Ольга!Спасибо!",
                "Добрый! Спасибо. Гляну.",
                "Здравствуйте Наталья, хорошо",
                "Здравствуйте, Светлана. Взаимно:)",
                "И вам💐",
                "Огромное спасибо",
                "Огонь",
                "ок, сделаю",
                "Ок. Спасибо.",
                "Ольга, доброе утро",
                "Оля, супер, спассибо",
                "Отлично, спасибо.",
                "Поняла, спасибо большое!",
                "Понятно)",
                "Светлана, спасибо большое! ❤️😘",
                "Спасиб!",
                "Спасибо большое!!! Очень приятно!",
                "Спасибо большое за поздравления!!",
                "Спасибо вам большое",
                "Спасибо за поздравление:))",
                "Спасибо за поздравления )",
                "Спасибо Насть",
                "Спасибо огромное",
                "спасибо Света!",
                "спасибо ☺️",
                "Спасибо. Девочки. 👍😁👏",
                "Спасибо, дорогие Коллеги!",
                "Спасибо, ок",
                "Спасибо. ☺️",
                "Спасибо👍🏻",
                "Спаибо🌷",
                "Спасиьо",
                "спсибо",
                "Утро доброе !    Принято.",
                "Хорошо, конечно",
                "Хорошо, понял, спасибо",
                "Хорошо, сделаю",
                "хорошо, спасибо :)",
                "Шпасиба! 🥳",
                "ясно спс",
                "okay",
                "😊",
                "🌸🌸🌸",
                "✅",
            };
            var set = testCases.Select(z => new MessageDataSet{Message =  z}).ToList();
            var sut = set.FilterObviouslySuperfluous();

            sut.Should().BeEquivalentTo(new List<MessageDataSet>());
        }
    }
}

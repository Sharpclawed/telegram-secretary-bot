using System;
using FluentAssertions;
using NUnit.Framework;
using Telegram.Bot.Types;
using TelegramBotTry1;
using TelegramBotTry1.Commands;

namespace FunctionalTests
{
    [TestFixture]
    public class CommandParsingTests
    {
        private CommandDetector commandDetector;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            ITgBotClientEx nullClient = null;
            commandDetector = new CommandDetector(nullClient, null, null, null);
        }

        [Test]
        public void HistoryofTest()
        {
            var sut = commandDetector.Parse(TextToMessage("/historyof: 118274261 28.03.2020 3"));
            sut.Should().BeOfType(typeof(ViewHistoryOfCommand));
            ((ViewHistoryOfCommand)sut).Begin.Should().BeSameDateAs(new DateTime(2020, 3, 28));
            ((ViewHistoryOfCommand)sut).End.Should().BeSameDateAs(new DateTime(2020, 3, 31));
            ((ViewHistoryOfCommand)sut).UserId.Should().Be(118274261);
        }

        [Test]
        public void HistoryallTest()
        {
            var sut = commandDetector.Parse(TextToMessage("/historyall: 28.03.2020 3"));
            sut.Should().BeOfType(typeof(ViewHistoryAllCommand));
            ((ViewHistoryAllCommand)sut).Begin.Should().BeSameDateAs(new DateTime(2020, 3, 28));
            ((ViewHistoryAllCommand)sut).End.Should().BeSameDateAs(new DateTime(2020, 3, 31));
        }

        [Test]
        public void HistoryTest()
        {
            var sut = commandDetector.Parse(TextToMessage("/history: bazinga 28.03.2020 3"));
            sut.Should().BeOfType(typeof(ViewHistoryCommand));
            ((ViewHistoryCommand)sut).Begin.Should().BeSameDateAs(new DateTime(2020, 3, 28));
            ((ViewHistoryCommand)sut).End.Should().BeSameDateAs(new DateTime(2020, 3, 31));
            ((ViewHistoryCommand)sut).ChatName.Should().BeEquivalentTo("bazinga");
        }

        [Test]
        public void AddAdminTest()
        {
            var sut = commandDetector.Parse(new Message{Text = "/addadmin: bazinga", From = new User{Id = 0, Username = ""}, Chat = new Chat{Id = 0}});
            sut.Should().BeOfType(typeof(AddAdminCommand));
            ((AddAdminCommand)sut).AdminName.Should().Be("bazinga");
        }

        [Test]
        public void RemoveAdminTest()
        {
            var sut = commandDetector.Parse(new Message { Text = "/removeadmin: bazinga", From = new User { Id = 0, Username = "" }, Chat = new Chat { Id = 0 } });
            sut.Should().BeOfType(typeof(RemoveAdminCommand));
            ((RemoveAdminCommand)sut).AdminName.Should().Be("bazinga");
        }

        [Test]
        public void ViewAdminsTest()
        {
            var sut = commandDetector.Parse(TextToMessage("/viewadmins"));
            sut.Should().BeOfType(typeof(ViewAdminsCommand));
        }

        [Test]
        public void AddBkTest()
        {
            var sut = commandDetector.Parse(TextToMessage("/addbk: bazinga"));
            sut.Should().BeOfType(typeof(AddBkCommand));
            ((AddBkCommand)sut).BkName.Should().Be("bazinga");
        }

        [Test]
        public void RemoveBkTest()
        {
            var sut = commandDetector.Parse(TextToMessage("/removebk: bazinga"));
            sut.Should().BeOfType(typeof(RemoveBkCommand));
            ((RemoveBkCommand)sut).BkName.Should().Be("bazinga");
        }

        [Test]
        public void ViewBkTest()
        {
            var sut = commandDetector.Parse(TextToMessage("/viewbk"));
            sut.Should().BeOfType(typeof(ViewBkCommand));
        }

        [Test]
        public void ViewWaiterTest()
        {
            var sut = commandDetector.Parse(TextToMessage("/viewwaiters"));
            sut.Should().BeOfType(typeof(ViewWaitersCommand));
        }

        [Test]
        public void AddInactiveChatExceptionsTest()
        {
            var sut = commandDetector.Parse(TextToMessage("/addonetimechat: bazinga"));
            sut.Should().BeOfType(typeof(AddOnetimeChatCommand));
            ((AddOnetimeChatCommand)sut).ChatName.Should().Be("bazinga");
        }

        [Test]
        public void RemoveInactiveChatExceptionsTest()
        {
            var sut = commandDetector.Parse(TextToMessage("/removeonetimechat: bazinga"));
            sut.Should().BeOfType(typeof(RemoveOnetimeChatCommand));
            ((RemoveOnetimeChatCommand)sut).ChatName.Should().Be("bazinga");
        }

        [Test]
        public void ViewInactiveChatExceptionsTest()
        {
            var sut = commandDetector.Parse(TextToMessage("/viewonetimechats"));
            sut.Should().BeOfType(typeof(ViewOneTimeChatsCommand));
        }

        [Test]
        public void ViewInactiveChatsTest()
        {
            var sut = commandDetector.Parse(TextToMessage("/viewinactivechats"));
            sut.Should().BeOfType(typeof(ViewInactiveChatsCommand));
        }

        [Test]
        public void WrongCommandTest()
        {
            var sut = commandDetector.Parse(TextToMessage("WrongCommand"));
            sut.Should().BeOfType(typeof(SendMessageCommand));
        }

        private Message TextToMessage(string text)
        {
            return new() {Text = text, Chat = new Chat { Id = 0 } };
        }
    }
}
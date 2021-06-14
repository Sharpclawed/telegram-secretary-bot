using System;
using FluentAssertions;
using NUnit.Framework;
using TelegramBotTry1;
using TelegramBotTry1.Commands;

namespace FunctionalTests
{
    [TestFixture]
    public class CommandParsingTests
    {
        [Test]
        public void HistoryofTest()
        {
            var sut = CommandDetector.Parse("/historyof: 118274261 28.03.2020 3");
            sut.Should().BeOfType(typeof(ViewHistoryOfCommand));
            ((ViewHistoryOfCommand)sut).Begin.Should().BeSameDateAs(new DateTime(2020, 3, 28));
            ((ViewHistoryOfCommand)sut).End.Should().BeSameDateAs(new DateTime(2020, 3, 31));
            ((ViewHistoryOfCommand)sut).UserId.Should().Be(118274261);
        }

        [Test]
        public void HistoryallTest()
        {
            var sut = CommandDetector.Parse("/historyall: 28.03.2020 3");
            sut.Should().BeOfType(typeof(ViewHistoryAllCommand));
            ((ViewHistoryAllCommand)sut).Begin.Should().BeSameDateAs(new DateTime(2020, 3, 28));
            ((ViewHistoryAllCommand)sut).End.Should().BeSameDateAs(new DateTime(2020, 3, 31));
        }

        [Test]
        public void HistoryTest()
        {
            var sut = CommandDetector.Parse("/history: bazinga 28.03.2020 3");
            sut.Should().BeOfType(typeof(ViewHistoryCommand));
            ((ViewHistoryCommand)sut).Begin.Should().BeSameDateAs(new DateTime(2020, 3, 28));
            ((ViewHistoryCommand)sut).End.Should().BeSameDateAs(new DateTime(2020, 3, 31));
            ((ViewHistoryCommand)sut).ChatName.Should().BeEquivalentTo("bazinga");
        }

        [Test]
        public void AddAdminTest()
        {
            var sut = CommandDetector.Parse("/addadmin: bazinga", 0, "");
            sut.Should().BeOfType(typeof(AddAdminCommand));
            ((AddAdminCommand)sut).AdminName.Should().Be("bazinga");
        }

        [Test]
        public void RemoveAdminTest()
        {
            var sut = CommandDetector.Parse("/removeadmin: bazinga", 0, "");
            sut.Should().BeOfType(typeof(RemoveAdminCommand));
            ((RemoveAdminCommand)sut).AdminName.Should().Be("bazinga");
        }

        [Test]
        public void ViewAdminsTest()
        {
            var sut = CommandDetector.Parse("/viewadmins");
            sut.Should().BeOfType(typeof(ViewAdminsCommand));
        }

        [Test]
        public void AddBkTest()
        {
            var sut = CommandDetector.Parse("/addbk: bazinga");
            sut.Should().BeOfType(typeof(AddBkCommand));
            ((AddBkCommand)sut).BkName.Should().Be("bazinga");
        }

        [Test]
        public void RemoveBkTest()
        {
            var sut = CommandDetector.Parse("/removebk: bazinga");
            sut.Should().BeOfType(typeof(RemoveBkCommand));
            ((RemoveBkCommand)sut).BkName.Should().Be("bazinga");
        }

        [Test]
        public void ViewBkTest()
        {
            var sut = CommandDetector.Parse("/viewbk");
            sut.Should().BeOfType(typeof(ViewBkCommand));
        }

        [Test]
        public void ViewWaiterTest()
        {
            var sut = CommandDetector.Parse("/viewwaiters");
            sut.Should().BeOfType(typeof(ViewWaitersCommand));
        }

        [Test]
        public void AddInactiveChatExceptionsTest()
        {
            var sut = CommandDetector.Parse("/addonetimechat: bazinga");
            sut.Should().BeOfType(typeof(AddOnetimeChatCommand));
            ((AddOnetimeChatCommand)sut).ChatName.Should().Be("bazinga");
        }

        [Test]
        public void RemoveInactiveChatExceptionsTest()
        {
            var sut = CommandDetector.Parse("/removeonetimechat: bazinga");
            sut.Should().BeOfType(typeof(RemoveOnetimeChatCommand));
            ((RemoveOnetimeChatCommand)sut).ChatName.Should().Be("bazinga");
        }

        [Test]
        public void ViewInactiveChatExceptionsTest()
        {
            var sut = CommandDetector.Parse("/viewonetimechats");
            sut.Should().BeOfType(typeof(ViewOneTimeChatsCommand));
        }

        [Test]
        public void ViewInactiveChatsTest()
        {
            var sut = CommandDetector.Parse("/viewinactivechats");
            sut.Should().BeOfType(typeof(ViewInactiveChatsCommand));
        }

        [Test]
        public void WrongCommandTest()
        {
            Assert.Throws<InvalidCastException>(() => CommandDetector.Parse("WrongCommand"));
        }
    }
}
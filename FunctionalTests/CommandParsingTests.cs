using System;
using FluentAssertions;
using NUnit.Framework;
using TelegramBotTry1;
using TelegramBotTry1.Dto;
using TelegramBotTry1.Enums;

namespace FunctionalTests
{
    [TestFixture]
    public class CommandParsingTests
    {
        [Test]
        public void HistoryofTest()
        {
            var sut = new HistoryCommand("/historyof: 118274261 28.03.2020 3");
            sut.Begin.Should().BeSameDateAs(new DateTime(2020, 3, 28));
            sut.End.Should().BeSameDateAs(new DateTime(2020, 3, 31));
            sut.NameOrId.Should().BeEquivalentTo("118274261");
            sut.Type.Should().Be(HistoryCommandType.SingleUser);
        }

        [Test]
        public void HistoryallTest()
        {
            var sut = new HistoryCommand("/historyall: 28.03.2020 3");
            sut.Begin.Should().BeSameDateAs(new DateTime(2020, 3, 28));
            sut.End.Should().BeSameDateAs(new DateTime(2020, 3, 31));
            sut.Type.Should().Be(HistoryCommandType.AllChats);
        }

        [Test]
        public void HistoryTest()
        {
            var sut = new HistoryCommand("/history: bazinga 28.03.2020 3");
            sut.Begin.Should().BeSameDateAs(new DateTime(2020, 3, 28));
            sut.End.Should().BeSameDateAs(new DateTime(2020, 3, 31));
            sut.NameOrId.Should().BeEquivalentTo("bazinga");
            sut.Type.Should().Be(HistoryCommandType.SingleChat);
        }

        [Test]
        public void AddAdminTest()
        {
            var sut = CommandDetector.Parse("/addadmin: bazinga", 0, "");
            sut.Should().BeOfType(typeof(AddAdminCommand));
            sut.EntityType.Should().Be(EntityType.Admin);
            ((AddAdminCommand)sut).AdminName.Should().Be("bazinga");
        }

        [Test]
        public void RemoveAdminTest()
        {
            var sut = CommandDetector.Parse("/removeadmin: bazinga", 0, "");
            sut.Should().BeOfType(typeof(RemoveAdminCommand));
            sut.EntityType.Should().Be(EntityType.Admin);
            ((RemoveAdminCommand)sut).AdminName.Should().Be("bazinga");
        }

        [Test]
        public void ViewAdminsTest()
        {
            var sut = CommandDetector.Parse("/viewadmins");
            sut.Should().BeOfType(typeof(ViewAdminsCommand));
            sut.EntityType.Should().Be(EntityType.Admin);
        }

        [Test]
        public void AddBkTest()
        {
            var sut = CommandDetector.Parse("/addbk: bazinga");
            sut.Should().BeOfType(typeof(AddBkCommand));
            sut.EntityType.Should().Be(EntityType.Bookkeeper);
            ((AddBkCommand)sut).BkName.Should().Be("bazinga");
        }

        [Test]
        public void RemoveBkTest()
        {
            var sut = CommandDetector.Parse("/removebk: bazinga");
            sut.Should().BeOfType(typeof(RemoveBkCommand));
            sut.EntityType.Should().Be(EntityType.Bookkeeper);
            ((RemoveBkCommand)sut).BkName.Should().Be("bazinga");
        }

        [Test]
        public void ViewBkTest()
        {
            var sut = CommandDetector.Parse("/viewbk");
            sut.Should().BeOfType(typeof(ViewBkCommand));
            sut.EntityType.Should().Be(EntityType.Bookkeeper);
        }

        [Test]
        public void ViewWaiterTest()
        {
            var sut = CommandDetector.Parse("/viewwaiters");
            sut.Should().BeOfType(typeof(ViewWaitersCommand));
            sut.EntityType.Should().Be(EntityType.Waiter);
        }

        [Test]
        public void AddInactiveChatExceptionsTest()
        {
            var sut = CommandDetector.Parse("/addonetimechat: bazinga");
            sut.Should().BeOfType(typeof(AddOnetimeChatCommand));
            sut.EntityType.Should().Be(EntityType.InactiveChatException);
            ((AddOnetimeChatCommand)sut).ChatName.Should().Be("bazinga");
        }

        [Test]
        public void RemoveInactiveChatExceptionsTest()
        {
            var sut = CommandDetector.Parse("/removeonetimechat: bazinga");
            sut.Should().BeOfType(typeof(RemoveOnetimeChatCommand));
            sut.EntityType.Should().Be(EntityType.InactiveChatException);
            ((RemoveOnetimeChatCommand)sut).ChatName.Should().Be("bazinga");
        }

        [Test]
        public void ViewInactiveChatExceptionsTest()
        {
            var sut = CommandDetector.Parse("/viewonetimechats");
            sut.Should().BeOfType(typeof(ViewOneTimeChatsCommand));
            sut.EntityType.Should().Be(EntityType.InactiveChatException);
        }

        [Test]
        public void ViewInactiveChatsTest()
        {
            var sut = CommandDetector.Parse("/viewinactivechats");
            sut.Should().BeOfType(typeof(ViewInactiveChatsCommand));
            sut.EntityType.Should().Be(EntityType.InactiveChat);
        }

        [Test]
        public void WrongCommandTest()
        {
            Assert.Throws<InvalidCastException>(() => CommandDetector.Parse("WrongCommand"));
        }
    }
}
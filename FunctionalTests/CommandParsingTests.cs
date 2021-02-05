using System;
using FluentAssertions;
using NUnit.Framework;
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
            sut.Begin.ShouldBeEquivalentTo(new DateTime(2020, 3, 28));
            sut.End.ShouldBeEquivalentTo(new DateTime(2020, 3, 31));
            sut.NameOrId.ShouldBeEquivalentTo("118274261");
            sut.Type.Should().Be(HistoryCommandType.SingleUser);
        }

        [Test]
        public void HistoryallTest()
        {
            var sut = new HistoryCommand("/historyall: 28.03.2020 3");
            sut.Begin.ShouldBeEquivalentTo(new DateTime(2020, 3, 28));
            sut.End.ShouldBeEquivalentTo(new DateTime(2020, 3, 31));
            sut.Type.Should().Be(HistoryCommandType.AllChats);
        }

        [Test]
        public void HistoryTest()
        {
            var sut = new HistoryCommand("/history: bazinga 28.03.2020 3");
            sut.Begin.ShouldBeEquivalentTo(new DateTime(2020, 3, 28));
            sut.End.ShouldBeEquivalentTo(new DateTime(2020, 3, 31));
            sut.NameOrId.ShouldBeEquivalentTo("bazinga");
            sut.Type.Should().Be(HistoryCommandType.SingleChat);
        }

        [Test]
        public void AddAdminTest()
        {
            var sut = new ManagingCommand("/addadmin: bazinga");
            sut.ManagingType.Should().Be(ManagingType.Add);
            sut.EntityType.Should().Be(EntityType.Admin);
            sut.EntityName.Should().Be("bazinga");
        }

        [Test]
        public void RemoveAdminTest()
        {
            var sut = new ManagingCommand("/removeadmin: bazinga");
            sut.ManagingType.Should().Be(ManagingType.Remove);
            sut.EntityType.Should().Be(EntityType.Admin);
            sut.EntityName.Should().Be("bazinga");
        }

        [Test]
        public void ViewAdminsTest()
        {
            var sut = new ManagingCommand("/viewadmins");
            sut.ManagingType.Should().Be(ManagingType.ViewList);
            sut.EntityType.Should().Be(EntityType.Admin);
            sut.EntityName.Should().Be(string.Empty);
        }

        [Test]
        public void AddBkTest()
        {
            var sut = new ManagingCommand("/addbk: bazinga");
            sut.ManagingType.Should().Be(ManagingType.Add);
            sut.EntityType.Should().Be(EntityType.Bookkeeper);
            sut.EntityName.Should().Be("bazinga");
        }

        [Test]
        public void RemoveBkTest()
        {
            var sut = new ManagingCommand("/removebk: bazinga");
            sut.ManagingType.Should().Be(ManagingType.Remove);
            sut.EntityType.Should().Be(EntityType.Bookkeeper);
            sut.EntityName.Should().Be("bazinga");
        }

        [Test]
        public void ViewBkTest()
        {
            var sut = new ManagingCommand("/viewbk");
            sut.EntityType.Should().Be(EntityType.Bookkeeper);
            sut.ManagingType.Should().Be(ManagingType.ViewList);
            sut.EntityName.Should().Be(string.Empty);
        }

        [Test]
        public void ViewWaiterTest()
        {
            var sut = new ManagingCommand("/viewwaiters");
            sut.ManagingType.Should().Be(ManagingType.ViewList);
            sut.EntityType.Should().Be(EntityType.Waiter);
            sut.EntityName.Should().Be(string.Empty);
        }

        [Test]
        public void AddInactiveChatExceptionsTest()
        {
            var sut = new ManagingCommand("/addonetimechat: bazinga");
            sut.ManagingType.Should().Be(ManagingType.Add);
            sut.EntityType.Should().Be(EntityType.InactiveChatException);
            sut.EntityName.Should().Be("bazinga");
        }

        [Test]
        public void RemoveInactiveChatExceptionsTest()
        {
            var sut = new ManagingCommand("/removeonetimechat: bazinga");
            sut.ManagingType.Should().Be(ManagingType.Remove);
            sut.EntityType.Should().Be(EntityType.InactiveChatException);
            sut.EntityName.Should().Be("bazinga");
        }

        [Test]
        public void ViewInactiveChatExceptionsTest()
        {
            var sut = new ManagingCommand("/viewonetimechats");
            sut.ManagingType.Should().Be(ManagingType.ViewList);
            sut.EntityType.Should().Be(EntityType.InactiveChatException);
            sut.EntityName.Should().Be(string.Empty);
        }

        [Test]
        public void ViewInactiveChatsTest()
        {
            var sut = new ManagingCommand("/viewinactivechats");
            sut.ManagingType.Should().Be(ManagingType.ViewList);
            sut.EntityType.Should().Be(EntityType.InactiveChat);
            sut.EntityName.Should().Be(string.Empty);
        }
    }
}
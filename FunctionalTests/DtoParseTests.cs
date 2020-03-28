using System;
using FluentAssertions;
using NUnit.Framework;
using TelegramBotTry1.Dto;
using TelegramBotTry1.Enums;

namespace FunctionalTests
{
    [TestFixture]
    public class DtoParseTests
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
            var sut = new UserManagingCommand("/addadmin: bazinga");
            sut.ManagingType.Should().Be(ManagingType.Add);
            sut.UserUserName.Should().Be("bazinga");
        }

        [Test]
        public void RemoveAdminTest()
        {
            var sut = new UserManagingCommand("/removeadmin: bazinga");
            sut.ManagingType.Should().Be(ManagingType.Remove);
            sut.UserUserName.Should().Be("bazinga");
        }

        [Test]
        public void ViewAdminsTest()
        {
            var sut = new UserViewCommand("/viewadmins");
            sut.ManagingType.Should().Be(ManagingType.ViewList);
        }

        [Test]
        public void AddBkTest()
        {
            var sut = new UserManagingCommand("/addbk: bazinga");
            sut.ManagingType.Should().Be(ManagingType.Add);
            sut.UserUserName.Should().Be("bazinga");
        }

        [Test]
        public void RemoveBkTest()
        {
            var sut = new UserManagingCommand("/removebk: bazinga");
            sut.ManagingType.Should().Be(ManagingType.Remove);
            sut.UserUserName.Should().Be("bazinga");
        }

        [Test]
        public void ViewBkTest()
        {
            var sut = new UserViewCommand("/viewbk");
            sut.ManagingType.Should().Be(ManagingType.ViewList);
        }
    }
}
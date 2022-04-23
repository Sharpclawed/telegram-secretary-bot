using System;
using System.Collections.Generic;
using System.Linq;
using TrunkRings.DAL;
using TrunkRings.DAL.Models;
using Microsoft.EntityFrameworkCore;
using TrunkRings.Domain.Models;

namespace TrunkRings.Domain.Services
{
    class AdminService : IAdminService
    {
        public bool TryMake(string userName, Admin addedBy, out string message)
        {
            using var context = new SecretaryContext();
            var adminDataSets = context.AdminDataSets;
            var messageDataSets = context.MessageDataSets.AsNoTracking();
            var user = messageDataSets.FindUserByUserName(userName);
            if (user == null)
            {
                message = "Пользователь не найден";
                return false;
            }

            if (adminDataSets.Any(x => x.UserId == user.UserId && x.DeleteTime == null))
            {
                message = "Пользователь уже в списке";
                return false;
            }

            adminDataSets.Add(new AdminDataSet
            {
                AddTime = DateTime.UtcNow,
                AddedUserId = addedBy.UserId,
                AddedUserName = addedBy.UserName,
                UserId = user.UserId,
                UserName = user.Name
            });
            context.SaveChanges();
            message = "Пользователь добавлен";
            return true;
        }

        public bool TryUnmake(string userName, Admin removedBy, out string message)
        {
            using var context = new SecretaryContext();
            var adminDataSets = context.AdminDataSets;
            var messageDataSets = context.MessageDataSets.AsNoTracking();
            var user = messageDataSets.FindUserByUserName(userName);
            if (user == null)
            {
                message = "Пользователь не найден";
                return false;
            }
            var adminToRemove = adminDataSets.FirstOrDefault(x => x.UserId == user.UserId && x.DeleteTime == null);
            if (adminToRemove == null)
            {
                message = "Пользователь отсутствует в списке";
                return false;
            }

            adminToRemove.DeleteTime = DateTime.UtcNow;
            adminToRemove.DeletedUserId = removedBy.UserId;
            adminToRemove.DeletedUserName = removedBy.UserName;
            context.SaveChanges();
            message = "Пользователь успешно удален";
            return true;
        }

        public bool IfAdmin(long userId)
        {
            using var context = new SecretaryContext();
            return context.AdminDataSets.AsNoTracking()
                .Where(x => x.UserId == userId)
                .SingleOrDefault(x => x.DeleteTime == null) != null;
        }

        public IEnumerable<(Admin, Admin)> GetAllActiveWithAddedBy()
        {
            using var context = new SecretaryContext();
            var records = context.AdminDataSets.AsNoTracking()
                .Where(x => x.DeleteTime == null).ToList();
            return records.Select(admin =>
                (new Admin {UserId = admin.UserId, UserName = admin.UserName}
                    , new Admin {UserId = admin.AddedUserId, UserName = admin.AddedUserName}));
        }
    }

    public interface IAdminService
    {
        bool TryMake(string userName, Admin addedBy, out string message);
        bool TryUnmake(string userName, Admin removedBy, out string message);
        bool IfAdmin(long userId);
        IEnumerable<(Admin, Admin)> GetAllActiveWithAddedBy();
    }
}
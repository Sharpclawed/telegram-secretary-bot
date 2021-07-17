using System;
using System.Collections.Generic;
using System.Linq;
using DAL;
using DAL.Models;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Domain.Services
{
    public class AdminService : IAdminService
    {
        public void Make(string userName, Admin addedBy)
        {
            using var context = new SecretaryContext();
            var adminDataSets = context.AdminDataSets;
            var messageDataSets = context.MessageDataSets.AsNoTracking();
            var user = messageDataSets.GetUserByUserName(userName);
            if (!adminDataSets.Any(x => x.UserId == user.UserId && x.DeleteTime == null))
            {
                adminDataSets.Add(new AdminDataSet
                {
                    AddTime = DateTime.UtcNow,
                    AddedUserId = addedBy.UserId,
                    AddedUserName = addedBy.UserName,
                    UserId = user.UserId,
                    UserName = user.Name
                });

                context.SaveChanges();
            }
        }

        public bool Unmake(string userName, Admin removedBy)
        {
            using var context = new SecretaryContext();
            var adminDataSets = context.AdminDataSets;
            var messageDataSets = context.MessageDataSets.AsNoTracking();
            var user = messageDataSets.GetUserByUserName(userName);
            var adminToRemove = adminDataSets.FirstOrDefault(x => x.UserId == user.UserId && x.DeleteTime == null);
            if (adminToRemove != null)
            {
                adminToRemove.DeleteTime = DateTime.UtcNow;
                adminToRemove.DeletedUserId = removedBy.UserId;
                adminToRemove.DeletedUserName = removedBy.UserName;
                context.SaveChanges();
                return true;
            }

            return false;
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
            return records.Select(z =>
                (new Admin {UserId = z.UserId, UserName = z.UserName}
                    , new Admin {UserId = z.AddedUserId, UserName = z.AddedUserName}));
        }
    }

    public interface IAdminService
    {
        void Make(string userName, Admin addedBy);
        bool Unmake(string userName, Admin removedBy);
        bool IfAdmin(long userId);
        IEnumerable<(Admin, Admin)> GetAllActiveWithAddedBy();
    }
}

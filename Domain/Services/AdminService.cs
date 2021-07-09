using System;
using System.Linq;
using DAL;
using DAL.Models;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Domain.Services
{
    public class AdminService
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
    }
}

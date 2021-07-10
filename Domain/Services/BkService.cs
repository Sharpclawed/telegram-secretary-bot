using System.Collections.Generic;
using System.Linq;
using DAL;
using DAL.Models;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Domain.Services
{
    public class BkService : IBkService
    {
        public void Make(string userName)
        {
            using var context = new SecretaryContext();
            var bkDataSets = context.BookkeeperDataSets;
            var messageDataSets = context.MessageDataSets.AsNoTracking();
            var user = messageDataSets.GetUserByUserName(userName);
            if (!bkDataSets.Any(x => x.UserId == user.UserId))
            {
                bkDataSets.Add(new BookkeeperDataSet
                {
                    UserId = user.UserId,
                    UserName = user.Username,
                    UserFirstName = user.Name,
                    UserLastName = user.Surname
                });
                context.SaveChanges();
            }
        }

        public bool Unmake(string userName)
        {
            using var context = new SecretaryContext();
            var bkDataSets = context.BookkeeperDataSets;
            var messageDataSets = context.MessageDataSets.AsNoTracking();
            var user = messageDataSets.GetUserByUserName(userName);
            var bkToRemove = bkDataSets.FirstOrDefault(x => x.UserId == user.UserId);
            if (bkToRemove != null)
            {
                bkDataSets.Remove(bkToRemove);
                context.SaveChanges();
                return true;
            }

            return false;
        }

        public IEnumerable<User> GetAll()
        {
            using var context = new SecretaryContext();
            return context.BookkeeperDataSets.AsNoTracking()
                .Select(z => new User
                {
                    UserId = z.UserId,
                    Name = z.UserFirstName,
                    Surname = z.UserLastName,
                    Username = z.UserName
                });
        }
    }

    public interface IBkService
    {
        void Make(string userName);
        bool Unmake(string userName);
        IEnumerable<User> GetAll();
    }
}
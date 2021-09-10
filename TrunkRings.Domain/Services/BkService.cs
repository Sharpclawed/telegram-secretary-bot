using System.Collections.Generic;
using System.Linq;
using TrunkRings.DAL;
using TrunkRings.DAL.Models;
using Microsoft.EntityFrameworkCore;
using TrunkRings.Domain.Models;

namespace TrunkRings.Domain.Services
{
    public class BkService : IBkService
    {
        public bool TryMake(string userName, out string message)
        {
            using var context = new SecretaryContext();
            var bkDataSets = context.BookkeeperDataSets;
            var messageDataSets = context.MessageDataSets.AsNoTracking();
            var user = messageDataSets.FindUserByUserName(userName);
            if (user == null)
            {
                message = "Пользователь не найден";
                return false;
            }

            if (bkDataSets.Any(x => x.UserId == user.UserId))
            {
                message = "Пользователь уже в списке";
                return false;
            }

            bkDataSets.Add(new BookkeeperDataSet
            {
                UserId = user.UserId,
                UserName = user.Username,
                UserFirstName = user.Name,
                UserLastName = user.Surname
            });
            context.SaveChanges();
            message = "Пользователь добавлен";
            return true;
        }

        public bool TryUnmake(string userName, out string message)
        {
            using var context = new SecretaryContext();
            var bkDataSets = context.BookkeeperDataSets;
            var messageDataSets = context.MessageDataSets.AsNoTracking();
            var user = messageDataSets.FindUserByUserName(userName);
            if (user == null)
            {
                message = "Пользователь не найден";
                return false;
            }
            var bkToRemove = bkDataSets.FirstOrDefault(x => x.UserId == user.UserId);
            if (bkToRemove == null)
            {
                message = "Пользователь отсутствует в списке";
                return false;
            }

            bkDataSets.Remove(bkToRemove);
            context.SaveChanges();
            message = "Пользователь успешно удален";
            return true;
        }

        public IEnumerable<User> GetAll()
        {
            using var context = new SecretaryContext();
            return context.BookkeeperDataSets.AsNoTracking().ToList()
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
        bool TryMake(string userName, out string message);
        bool TryUnmake(string userName, out string message);
        IEnumerable<User> GetAll();
    }
}
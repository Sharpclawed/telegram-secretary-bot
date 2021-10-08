using Microsoft.EntityFrameworkCore;
using TrunkRings.DAL;
using TrunkRings.Domain.Services;

namespace TrunkRings.Domain
{
    public class DomainServices
    {
        public DomainServices()
        {
            using var context = new SecretaryContext();
            context.Database.Migrate();
        }

        public IMessageService GetMessageService()
        {
            return new MessageService();
        }

        public IAdminService GetAdminService()
        {
            return new AdminService();
        }

        public IBkService GetBookkeeperService()
        {
            return new BkService();
        }

        public IOneTimeChatService GetOneTimeChatService()
        {
            return new OneTimeChatService();
        }
    }
}

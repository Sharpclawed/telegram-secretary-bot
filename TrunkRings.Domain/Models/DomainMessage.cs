using System;

namespace TrunkRings.Domain.Models
{
    public class DomainMessage
    {
        public long MessageId { get; set; }
        public DateTime Date { get; set; }
        public string UserName { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public long UserId { get; set; }
        public long ChatId { get; set; }
        public string ChatName { get; set; }
        public string Message { get; set; }
    }
}

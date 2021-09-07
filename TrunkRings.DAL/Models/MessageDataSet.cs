using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrunkRings.DAL.Models
{
    public class MessageDataSet
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid MessageDataSetId { get; set; }
        public long MessageId { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public string UserName { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public long UserId { get; set; }
        public long ChatId { get; set; }
        public string ChatName { get; set; }

        public override string ToString()
        {
            return ChatName + " " +
                    UserFirstName + " " +
                    UserLastName + " " +
                    UserName + "| " +
                    Date + " " +
                    Message;
        }
    }
}
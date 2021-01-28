using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TelegramBotTry1.Domain
{
    public class OnetimeChatDataSet
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid OnetimeChatDataSetId { get; set; }
        public long ChatId { get; set; }
        public string ChatName { get; set; }
        public long AddedUserId { get; set; }
        public string AddedUserName { get; set; }
        public DateTime AddTime { get; set; }
        public long? DeletedUserId { get; set; }
        public string DeletedUserName { get; set; }
        public DateTime? DeleteTime { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TelegramBotTry1.Domain
{
    public class AdminDataSet
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AdminDataSetId { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public long AddedUserId { get; set; }
        public string AddedUserName { get; set; }
        public DateTime AddTime { get; set; }
        public long DeletedUserId { get; set; }
        public string DeletedUserName { get; set; }
        public DateTime DeleteTime { get; set; }
    }
}
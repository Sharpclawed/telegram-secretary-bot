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
    }
}
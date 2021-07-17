using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class OnetimeChatDataSet
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid OnetimeChatDataSetId { get; set; }
        public long ChatId { get; set; }
        public string ChatName { get; set; }
    }
}
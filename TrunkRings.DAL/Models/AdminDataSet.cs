using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrunkRings.DAL.Models
{
    public class AdminDataSet
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AdminDataSetId { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public long AddedUserId { get; set; }
        public string AddedUserName { get; set; }
        public DateTimeOffset AddTime { get; set; }
        public long? DeletedUserId { get; set; }
        public string DeletedUserName { get; set; }
        public DateTimeOffset? DeleteTime { get; set; }
    }
}
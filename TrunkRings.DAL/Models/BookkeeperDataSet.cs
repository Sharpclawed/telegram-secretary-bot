﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrunkRings.DAL.Models
{
    public class BookkeeperDataSet
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid BookkeeperDataSetId { get; set; }
        public string UserName { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public long UserId { get; set; }
    }
}
﻿using Dapper.Contrib.Extensions;

namespace xUnit.Rop.Dapper.ContribEx9.Data
{
    [Table("Stuff")]
    public class Stuff
    {
        [Key]
        public short TheId { get; set; }
        public string Name { get; set; }
        public DateTime? Created { get; set; }
    }
}
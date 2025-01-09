using Dapper.Contrib.Extensions;

namespace xUnit.Rop.Dapper.ContribEx9.Data
{
    [Table("ObjectZ")]
    public class ObjectZ
    {
        [ExplicitKey]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
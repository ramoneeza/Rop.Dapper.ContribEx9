using Dapper.Contrib.Extensions;

namespace xUnit.Rop.Dapper.ContribEx9.Data
{
    [Table("ObjectY")]
    public class ObjectY
    {
        [ExplicitKey]
        public int ObjectYId { get; set; }
        public string Name { get; set; }
    }
}
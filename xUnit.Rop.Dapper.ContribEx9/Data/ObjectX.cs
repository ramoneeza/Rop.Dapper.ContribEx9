using Dapper.Contrib.Extensions;

namespace xUnit.Rop.Dapper.ContribEx9.Data
{
     [Table("ObjectX")]
    public class ObjectX
    {
        [ExplicitKey]
        public string ObjectXId { get; set; }
        public string Name { get; set; }
    }
}

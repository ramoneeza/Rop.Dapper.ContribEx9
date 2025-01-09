using Dapper.Contrib.Extensions;

namespace xUnit.Rop.Dapper.ContribEx9.Data
{
    [Table("GenericType")]
    public class GenericType<T>
    {
        [ExplicitKey]
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
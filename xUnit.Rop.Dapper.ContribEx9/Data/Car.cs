using Dapper.Contrib.Extensions;
using Rop.Dapper.ContribEx9;

namespace xUnit.Rop.Dapper.ContribEx9.Data
{
    [Table("Automobiles")]
    public class Car
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [LeftJoin(typeof(Maniobra))]
        [Computed]
        public Maniobra[] Maniobras { get; set; } = [];
    }
}
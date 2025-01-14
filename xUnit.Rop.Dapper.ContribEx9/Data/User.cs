using Dapper.Contrib.Extensions;

namespace xUnit.Rop.Dapper.ContribEx9.Data
{
    public class User : IUser
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
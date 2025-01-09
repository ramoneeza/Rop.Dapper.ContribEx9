using Dapper.Contrib.Extensions;

namespace xUnit.Rop.Dapper.ContribEx9.Data
{
    public interface INullableDate
    {
        [Key]
        int Id { get; set; }
        DateTime? DateValue { get; set; }
    }
}
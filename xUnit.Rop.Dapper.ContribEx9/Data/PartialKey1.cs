using Dapper.Contrib.Extensions;
using Rop.Dapper.ContribEx9;

namespace xUnit.Rop.Dapper.ContribEx9.Data
{
    public class PartialKey1
    {
        [PartialKey]
        public string PKey { get; set; }
        public string Key2 { get; set; }
        public string Data { get; set; }
    }
    public class Maniobra
    {
        [PartialKey]
        public int PKey { get; set; }
        public int Key2 { get; set; }
        public string Grupo { get; set; }
    }
}

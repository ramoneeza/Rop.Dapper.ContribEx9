using Dapper.Contrib.Extensions;
using Rop.Dapper.ContribEx9;

namespace xUnit.Rop.Dapper.ContribEx9.Data
{
    public class PartialKey1
    {
        [PartialKey(0)]
        public string PKey { get; set; }
        [PartialKey(1)]
        public string Key2 { get; set; }
        public string Data { get; set; }
    }
    public class Maniobra
    {
        [PartialKey(0)]
        public int PKey { get; set; }
        [PartialKey(1)]
        public int Key2 { get; set; }
        public string Grupo { get; set; }
    }
}

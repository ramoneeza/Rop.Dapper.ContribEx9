using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using Rop.Dapper.ContribEx9;
using xUnit.Rop.Dapper.ContribEx9.Data;

namespace xUnit.Rop.Dapper.ContribEx9
{
    public class PartialKeyTest
    {
        // We can change TestSuite to other DB Engines
        private static readonly ITestSuite TestSuite = new SQLiteTestSuite();
        private IDbConnection GetOpenConnection()
        {
            var connection = GetConnection();
            connection.Open();
            TestSuite.CreateMemoryTables(connection);
            return connection;
        }
        private IDbConnection GetConnection()
        {
            return TestSuite.GetConnection();
        }
        private static (Car, Car, Car) _createCars()
        {
            var c1 = new Car()
            {
                Id = 1,
                Name = "Ford",
                Computed = "America"
            };
            var c2 = new Car()
            {
                Id = 2,
                Name = "Ford",
                Computed = "Europe"
            };
            var c3 = new Car()
            {
                Id = 3,
                Name = "Renault",
                Computed = "Europe"
            };
            return (c1, c2, c3);
        }
        private static (Maniobra,Maniobra,Maniobra) _createPartials()
        {
            var c1 = new Maniobra()
            {
                PKey = 1,
                Key2 = 1,
                Grupo = "Pepe"
            };
            var c2 = new Maniobra()
            {
                PKey = 1,
                Key2 = 2,
                Grupo = "Antonio"
            };
            var c3 = new Maniobra()
            {
                PKey = 2, Key2 = 3, Grupo = "Juan"
            };
            return (c1, c2, c3);
        }

        private void _insertCar(IDbConnection conn)
        {
            var (item1,item2,item3) = _createCars();
            conn.Insert(item1);
            conn.Insert(item2);
            conn.Insert(item3);
        }

        private void _insertPartials(IDbConnection conn)
        {
           var (item1, item2, item3) = _createPartials();
            conn.Insert(item1);
            conn.Insert(item2);
            conn.Insert(item3);
        }
        [Fact]
        public void DeletePartialTest()
        {
            using (var conn = GetOpenConnection())
            {
                _insertCar(conn);
                _insertPartials(conn);
                var r1 = conn.DeleteByPartialKey<Maniobra>(1);
                Assert.Equal(2,r1);
                var r2 = conn.DeleteByPartialKey<Maniobra>(2);
                Assert.Equal(1,r2);
                // Already deleted
                r1 = conn.DeleteByPartialKey<Maniobra>(1);
                Assert.Equal(0,r1);
                r2 = conn.DeleteByPartialKey<Maniobra>(2);
                Assert.Equal(0,r2);
            }
        }

        [Fact]
        public void GetPartial()
        {
            using var conn = GetOpenConnection();
            var r= _createPartials();
            _insertPartials(conn);
            var result = conn.GetPartial<Maniobra>(1).OrderBy(m=>m.PKey).ThenBy(m=>m.Key2).ToList();
            Assert.Equal(2, result.Count());
            Assert.Equal(r.Item1.PKey, result[0].PKey);
            Assert.Equal(r.Item1.Key2,result[0].Key2);
            Assert.Equal(r.Item1.Grupo,result[0].Grupo);
            Assert.Equal(r.Item2.PKey, result[1].PKey);
            Assert.Equal(r.Item2.Key2, result[1].Key2);
            Assert.Equal(r.Item2.Grupo,result[1].Grupo);
        }

        [Fact]
        public void LeftJoinTest()
        {
            using var conn = GetOpenConnection();
            _insertCar(conn);
            _insertPartials(conn);
            var (car, maniobras) = conn.GetLeftJoin<Car,Maniobra>(1);
            Assert.Equal(1,car.Id);
            Assert.Equal(2, maniobras.Length);
            Assert.Equal(1, maniobras[0].PKey);
            Assert.Equal(1, maniobras[1].PKey);
        }
    }

}

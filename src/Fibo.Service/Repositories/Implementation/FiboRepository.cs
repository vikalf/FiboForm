using Dapper;
using Fibo.Service.Repositories.Definition;
using Npgsql;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fibo.Service.Repositories.Implementation
{
    public class FiboRepository : IFiboRepository
    {
        public async Task CreateVisitValuesTable()
        {
            using (var cnn = GetConnection())
            {
                await cnn.OpenAsync();
                await cnn.ExecuteAsync("CREATE TABLE IF NOT EXISTS VisitedValues(FiboIndex INT)");
            }
        }

        public async Task<List<int>> GetVisitedValuesFromDb()
        {
            using (var cnn = GetConnection())
            {
                await cnn.OpenAsync();
                var result = await cnn.QueryAsync<int>("SELECT FiboIndex FROM VisitedValues");
                return result.ToList();
            }
        }

        public async Task<List<int>> InsertValue(int index)
        {
            using (var cnn = GetConnection())
            {
                await cnn.OpenAsync();
                var result = await cnn.QueryAsync<int>("INSERT INTO FiboIndex(FiboIndex) VALUES(@index)", param: new { index });
                return result.ToList();
            }
        }

        private NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection("Host=192.168.99.100:5432;Username=initdb;Password=password01;Database=initdb");
        }
    }
}

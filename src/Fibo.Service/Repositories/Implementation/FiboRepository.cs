using Dapper;
using Fibo.Service.Repositories.Definition;
using FiboForm.Common;
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
                var result = await cnn.QueryAsync<int>("INSERT INTO VisitedValues(FiboIndex) VALUES(@index)", param: new { index });
                return result.ToList();
            }
        }

        private NpgsqlConnection GetConnection()
        {
            var server = EnvironmentSettings.GetEnvironmentVariable("PG_SERVER");
            var port = EnvironmentSettings.GetEnvironmentVariable("PG_PORT");
            var db = EnvironmentSettings.GetEnvironmentVariable("PG_DATABASE");
            var userId = EnvironmentSettings.GetEnvironmentVariable("PG_USER");
            var password = EnvironmentSettings.GetEnvironmentVariable("PG_PASSWORD");

            return new NpgsqlConnection($"Server={server};Port={port};Database={db};User Id={userId};Password={password};");
        }
    }
}

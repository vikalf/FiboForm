using Dapper;
using FiboForm.Api.Repositories.Definition;
using FiboForm.Common;
using FiboForm.Common.Components.Definition;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiboForm.Api.Repositories.Implementation
{
    public class FiboRepository : IFiboRepository
    {
        private readonly IEnvironmentSettings _environmentSettings;
        private readonly ILogger<FiboRepository> _logger;

        public FiboRepository(IEnvironmentSettings environmentSettings, ILogger<FiboRepository> logger)
        {
            _environmentSettings = environmentSettings;
            _logger = logger;
        }

        public async Task CreateVisitValuesTable()
        {
            using (var cnn = GetConnection())
            {
                try
                {
                    await cnn.OpenAsync();
                    await cnn.ExecuteAsync("CREATE TABLE IF NOT EXISTS VisitedValues(FiboIndex INT)");
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, "CreateVisitValuesTable()");
                    throw;
                }
            }
        }

        public async Task<List<int>> GetVisitedValuesFromDb()
        {
            await CreateVisitValuesTable();
            using (var cnn = GetConnection())
            {
                try
                {
                    await cnn.OpenAsync();
                    var result = await cnn.QueryAsync<int>("SELECT FiboIndex FROM VisitedValues");
                    return result.ToList();
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, "GetVisitedValuesFromDb()");
                    throw;
                }
            }
        }

        public async Task<List<int>> InsertValue(int index)
        {
            await CreateVisitValuesTable();
            using (var cnn = GetConnection())
            {
                try
                {
                    await cnn.OpenAsync();
                    var result = await cnn.QueryAsync<int>("INSERT INTO VisitedValues(FiboIndex) VALUES(@index)", param: new { index });
                    return result.ToList();
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, $"GetVisitedValuesFromDb({index})");
                    throw;
                }
            }
        }

        private NpgsqlConnection GetConnection()
        {
            try
            {
                var server = _environmentSettings.GetEnvironmentVariable("PG_SERVER");
                var port = _environmentSettings.GetEnvironmentVariable("PG_PORT");
                var db = _environmentSettings.GetEnvironmentVariable("PG_DATABASE");
                var userId = _environmentSettings.GetEnvironmentVariable("PG_USER");
                var password = _environmentSettings.GetEnvironmentVariable("PG_PASSWORD");

                return new NpgsqlConnection($"Server={server};Port={port};Database={db};User Id={userId};Password={password};");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Unable to Get Postgres Connection");
                throw;
            }
        }
    }
}

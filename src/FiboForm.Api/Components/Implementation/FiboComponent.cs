using FiboForm.Api.Components.Definition;
using FiboForm.Api.Models;
using FiboForm.Api.Repositories.Definition;
using FiboForm.Common;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiboForm.Api.Components.Implementation
{
    public class FiboComponent : IFiboComponent
    {
        private readonly ILogger<FiboComponent> _logger;
        private readonly IDistributedCache _distributedCache;
        private readonly IFiboRepository _fiboRepository;

        public FiboComponent(ILogger<FiboComponent> logger, IDistributedCache distributedCache, IFiboRepository fiboRepository)
        {
            _logger = logger;
            _distributedCache = distributedCache;
            _fiboRepository = fiboRepository;
        }

        public async Task<Payload> GetFiboPayload()
        {
            Payload payload = new Payload();
            var result = await GetVisitedValues();
            payload.VisitedIndexes = result.Indexes.ToList();
            payload.VisitedValues = result.VisitedValues.Select(e => new VisitedValues { Index = e.Key, Value = e.Value }).ToList();
            return payload;
        }

        public async Task<Payload> SearchFiboNumber(int index)
        {
            // Get Fibo Number
            var fiboNumber = GetFiboNumeralByIndex(index);

            // Save in Redis and Postgres
            var saveRedisReply = await SaveFiboValueRedis(index, fiboNumber);
            var savePostgresReply = await SaveFiboIndexPostgres(index);

            if (savePostgresReply && saveRedisReply)
            {
                Payload result = await GetFiboPayload();
                return result;
            }
            else
                throw new System.OperationCanceledException("");


        }


        #region Methods

        private int GetFiboNumeralByIndex(int index)
        {
            List<int> fibonacciList = GetFibonacciList(index);
            var fiboNumeral = fibonacciList[index];
            return fiboNumeral;
        }

        private async Task<VisitedValuesModel> GetVisitedValues()
        {

            VisitedValuesModel result = new VisitedValuesModel
            {
                Indexes = await _fiboRepository.GetVisitedValuesFromDb(),
                VisitedValues = await GetVisitedValuesFromRedis()
            };

            return result;

        }

        private async Task<Dictionary<int, int>> GetVisitedValuesFromRedis()
        {
            var cacheKey = "fibo_visited_values";
            var visitedValuesBytes = await _distributedCache.GetAsync(cacheKey);
            var values = new Dictionary<int, int>();
            if (visitedValuesBytes != null)
            {
                var valuesBytes = await _distributedCache.GetAsync(cacheKey);
                values = Helpers.FromByteArray<Dictionary<int, int>>(valuesBytes);
            }

            return values;
        }

        private async Task<bool> SaveFiboValueRedis(int index, int fiboNumeral)
        {
            try
            {
                var cacheKey = "fibo_visited_values";
                var visitedValuesBytes = await _distributedCache.GetAsync(cacheKey);
                var values = new Dictionary<int, int>();
                if (visitedValuesBytes == null)
                {
                    // if Empty, save first key-value pair
                    values.Add(index, fiboNumeral);
                    var valuesBytes = Helpers.ToByteArray<Dictionary<int, int>>(values);
                    await _distributedCache.SetAsync(cacheKey, valuesBytes);
                }
                else
                {
                    var valuesBytes = await _distributedCache.GetAsync(cacheKey);
                    values = Helpers.FromByteArray<Dictionary<int, int>>(valuesBytes);

                    if (!values.Any(e => e.Key == index))
                    {
                        values.Add(index, fiboNumeral);
                        valuesBytes = Helpers.ToByteArray<Dictionary<int, int>>(values);
                        await _distributedCache.SetAsync(cacheKey, valuesBytes);
                    }
                }

                return true;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "SaveFiboValueRedis({index}, {fiboNumeral})", index, fiboNumeral);
                return false;
            }

        }

        private async Task<bool> SaveFiboIndexPostgres(int fiboIndex)
        {
            try
            {
                await _fiboRepository.InsertValue(fiboIndex);
                return true;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "SaveFiboIndexPostgres({fiboIndex})", fiboIndex);
                return false;
            }
        }


        private List<int> GetFibonacciList(int index)
        {
            List<int> result = new List<int>();
            var itemsCount = index + 1;

            for (int i = 0; i < itemsCount; i++)
            {
                var count = result.Count;
                if (count == itemsCount)
                    break;
                else if (count < 2)
                    result.Add(1);
                else
                {
                    var a = result[count - 2];
                    var b = result[count - 1];
                    result.Add(a + b);
                }
            }

            return result;

        }


        #endregion

    }
}

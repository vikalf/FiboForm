using Fibo.Service.Components.Definition;
using Fibo.Service.Models;
using Fibo.Service.Repositories.Definition;
using FiboForm.Common;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fibo.Service.Components.Implementation
{
    public class FiboComponent : IFiboComponent
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<FiboComponent> _logger;
        private readonly IFiboRepository _fiboRepository;

        public FiboComponent(IDistributedCache distributedCache, ILogger<FiboComponent> logger, IFiboRepository fiboRepository)
        {
            _distributedCache = distributedCache;
            _logger = logger;
            _fiboRepository = fiboRepository;
        }

        public async Task<FiboResultModel> GetFiboNumeralByIndex(int index)
        {
            await Task.Delay(1);
            List<int> fibonacciList = GetFibonacciList(index);
            var fiboNumeral = fibonacciList[index];
            var result = new FiboResultModel { FiboNumeral = fiboNumeral };
            result.VisitedValues = await GetVisitedValuesFromRedis(index, fiboNumeral);

            await _fiboRepository.CreateVisitValuesTable();
            await _fiboRepository.InsertValue(index);

            return result;
        }

        public async Task<List<int>> GetVisitedIndexes()
        {
            return await _fiboRepository.GetVisitedValuesFromDb();
        }

        private async Task<Dictionary<int, int>> GetVisitedValuesFromRedis(int index, int fiboNumeral)
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

            return values;

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


    }
}

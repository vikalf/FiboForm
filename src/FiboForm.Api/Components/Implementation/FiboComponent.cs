using FiboForm.Api.Components.Definition;
using FiboForm.Api.Models;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using static Fibo.Definition.Fibo;

namespace FiboForm.Api.Components.Implementation
{
    public class FiboComponent : IFiboComponent
    {
        private readonly FiboClient _fiboClient;
        private readonly ILogger<FiboComponent> _logger;

        public FiboComponent(FiboClient fiboClient, ILogger<FiboComponent> logger)
        {
            _fiboClient = fiboClient;
            _logger = logger;
        }

        public async Task<Payload> GetFiboPayload()
        {
            Payload payload = new Payload();
            var result = await _fiboClient.GetVisitedValuesAsync(new Fibo.Definition.EmptyRequest());
            payload.VisitedIndexes = result.Indexes.ToList();
            payload.VisitedValues = result.VisitedValues.Select(e => new VisitedValues { Index = e.Index, Value = e.Value }).ToList();
            return payload;
        }

        public async Task<Payload> SearchFiboNumber(int index)
        {
            // Get Fibo Number
            var reply = await _fiboClient.GetFiboNumberByIndexAsync(new Fibo.Definition.FiboNumberByIndexRequest { Index = index });
            var fiboNumber = reply.FiboNumber;

            // Save in Redis and Postgres
            var saveRedisReply = await _fiboClient.SaveFiboNumberRedisAsync(new Fibo.Definition.VisitedValue { Index = index, Value = fiboNumber });
            var savePostgresReply = await _fiboClient.SaveFiboIndexPostgresAsync(new Fibo.Definition.SaveFiboIndexPostgresRequest { FiboIndex = index });

            if (savePostgresReply.Success && saveRedisReply.Success)
            {
                Payload result = await GetFiboPayload();
                return result;
            }
            else
                throw new System.OperationCanceledException("");

        }
    }
}

using Fibo.Definition;
using Fibo.Service.Components.Definition;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Fibo.Service.Implementation
{
    public class FiboServiceImplementation : Fibo.Definition.Fibo.FiboBase
    {
        private readonly IServiceProvider _provider;
        private readonly IFiboComponent _fiboComponent;
        private readonly ILogger<FiboServiceImplementation> _logger;

        public FiboServiceImplementation(IServiceProvider provider)
        {
            var scoped = provider.CreateScope();
            _provider = provider;
            _logger = scoped.ServiceProvider.GetRequiredService<ILogger<FiboServiceImplementation>>();
            _fiboComponent = scoped.ServiceProvider.GetRequiredService<IFiboComponent>();
        }

        public async override Task<VisitedValuesReply> GetVisitedValues(EmptyRequest request, ServerCallContext context)
        {
            try
            {
                var result = await _fiboComponent.GetVisitedValues();
                var reply = new VisitedValuesReply();
                reply.Indexes.AddRange(result.Indexes.ToList());

                foreach (var item in result.VisitedValues)
                    reply.VisitedValues.Add(new VisitedValue { Index = item.Key, Value = item.Value });

                return reply;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Grpc Error GetVisitedIndexes()");
                throw new RpcException(new Status(StatusCode.Internal, "Grpc Error GetVisitedIndexes()"));
            }
        }

        public async override Task<FiboNumberByIndexReply> GetFiboNumberByIndex(FiboNumberByIndexRequest request, ServerCallContext context)
        {
            if (request.Index < 0)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Index must be greater than zero"));

            if (request.Index > 40)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Index must be less than 40"));

            try
            {
                await Task.Delay(1);
                var result = _fiboComponent.GetFiboNumeralByIndex(request.Index);
                var reply = new FiboNumberByIndexReply { FiboNumber = result };

                return reply;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Grpc Error GetFiboNumberByIndex({index})", request.Index);
                throw new RpcException(new Status(StatusCode.Internal, "Grpc Error GetFiboNumberByIndex({index})"));
            }
        }

        public async override Task<SaveFiboNumberReply> SaveFiboIndexPostgres(SaveFiboIndexPostgresRequest request, ServerCallContext context)
        {

            if (request.FiboIndex < 0)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Index must be greater than zero"));

            try
            {
                bool result = await _fiboComponent.SaveFiboIndexPostgres(request.FiboIndex);
                return new SaveFiboNumberReply { Success = result };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Grpc Error SaveFiboIndexPostgres({FiboIndex})", request.FiboIndex);
                throw new RpcException(new Status(StatusCode.Internal, "Grpc Error SaveFiboIndexPostgres({FiboIndex})"));
            }
        }

        public async override Task<SaveFiboNumberReply> SaveFiboNumberRedis(VisitedValue request, ServerCallContext context)
        {
            if (request.Index <= 0)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Index must be greater than zero"));

            if (request.Value <= 0)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Value must be greater than zero"));

            try
            {
                bool result = await _fiboComponent.SaveFiboValueRedis(request.Index, request.Value);
                return new SaveFiboNumberReply { Success = result };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Grpc Error SaveFiboIndexPostgres({Index}, {Value})", request.Index, request.Value);
                throw new RpcException(new Status(StatusCode.Internal, "Grpc Error SaveFiboIndexPostgres({Index}, {Value})"));
            }
        }

    }
}

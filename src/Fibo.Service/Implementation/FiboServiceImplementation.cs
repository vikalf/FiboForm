using Fibo.Definition;
using Fibo.Service.Components.Definition;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
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

        public async override Task<FiboNumberByIndexReply> GetFiboNumberByIndex(FiboNumberByIndexRequest request, ServerCallContext context)
        {
            if (request.Index < 0)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Index must be greater than zero"));

            if (request.Index > 40)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Index must be less than 40"));

            try
            {
                var result = await _fiboComponent.GetFiboNumeralByIndex(request.Index);
                var reply = new FiboNumberByIndexReply { FiboNumber = result.FiboNumeral };

                foreach (var item in result.VisitedValues)
                {
                    reply.VisitedValues.Add(new VisitedValue { Index = item.Key, Value = item.Value });
                }

                return reply;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Grpc Error GetFiboNumberByIndex({index})", request.Index);
                throw new RpcException(new Status(StatusCode.Internal, "Grpc Error GetFiboNumberByIndex({index})"));
            }
        }
    }
}

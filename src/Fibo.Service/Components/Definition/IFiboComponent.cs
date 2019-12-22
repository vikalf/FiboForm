using Fibo.Service.Models;
using System.Threading.Tasks;

namespace Fibo.Service.Components.Definition
{
    public interface IFiboComponent
    {
        int GetFiboNumeralByIndex(int index);
        Task<VisitedValuesModel> GetVisitedValues();
        Task<bool> SaveFiboValueRedis(int index, int fiboNumeral);
        Task<bool> SaveFiboIndexPostgres(int fiboIndex);
    }
}

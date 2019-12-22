using Fibo.Service.Models;
using System.Threading.Tasks;

namespace Fibo.Service.Components.Definition
{
    public interface IFiboComponent
    {
        Task<FiboResultModel> GetFiboNumeralByIndex(int index);
    }
}

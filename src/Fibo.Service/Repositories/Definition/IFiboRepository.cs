
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fibo.Service.Repositories.Definition
{
    interface IFiboRepository
    {
        Task CreateVisitValuesTable();
        Task<List<int>> GetVisitedValuesFromDb();
    }
}

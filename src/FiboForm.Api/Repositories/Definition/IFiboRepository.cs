
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FiboForm.Api.Repositories.Definition
{
    public interface IFiboRepository
    {
        Task CreateVisitValuesTable();
        Task<List<int>> GetVisitedValuesFromDb();
        Task<List<int>> InsertValue(int index);
    }
}

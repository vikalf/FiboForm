using FiboForm.Web.Models;
using System.Threading.Tasks;

namespace FiboForm.Web.Repositories.Definition
{
    public interface IFiboFormRepository
    {
        Task<FiboModel> GetPayload();
        Task<FiboModel> SearchIndex(int index);
    }
}

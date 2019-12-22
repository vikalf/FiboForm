using FiboForm.Api.Models;
using System.Threading.Tasks;

namespace FiboForm.Api.Components.Definition
{
    public interface IFiboComponent
    {
        Task<Payload> GetFiboPayload();
        Task<Payload> SearchFiboNumber(int index);
    }
}

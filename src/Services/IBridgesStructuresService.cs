using bridges_structures_service.Models;
using System.Threading.Tasks;

namespace bridges_structures_service.Services
{
    public interface IBridgesStructuresService
    {
        Task<string> CreateCase(BridgesStructuresReport formData);
    }
}

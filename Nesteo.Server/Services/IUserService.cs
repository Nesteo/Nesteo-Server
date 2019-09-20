using System.Threading.Tasks;
using Nesteo.Server.Models;

namespace Nesteo.Server.Services
{
    public interface IUserService
    {
        Task<User> FindUserByIdAsync(string id);
    }
}

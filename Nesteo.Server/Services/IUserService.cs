using System.Collections.Generic;
using System.Threading.Tasks;
using Nesteo.Server.Models;

namespace Nesteo.Server.Services
{
    public interface IUserService
    {
        Task<ICollection<User>> GetAllUsersAsync();

        Task<User> FindUserByIdAsync(string id);
    }
}

using System.Threading;
using System.Threading.Tasks;
using Nesteo.Server.Models;

namespace Nesteo.Server.Services
{
    public interface IUserService : ICrudService<User, string> { }
}

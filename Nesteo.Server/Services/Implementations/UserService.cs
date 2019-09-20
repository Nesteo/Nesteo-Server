using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Nesteo.Server.Data.Identity;
using Nesteo.Server.Models;

namespace Nesteo.Server.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<NesteoUser> _userManager;
        private readonly IMapper _mapper;

        public UserService(UserManager<NesteoUser> userManager, IMapper mapper)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<User> FindUserByIdAsync(string id)
        {
            NesteoUser identityUser = await _userManager.FindByIdAsync(id).ConfigureAwait(false);
            return _mapper.Map<User>(identityUser);
        }
    }
}

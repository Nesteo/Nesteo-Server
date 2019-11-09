using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nesteo.Server.Data.Entities.Identity;
using Nesteo.Server.Models;

namespace Nesteo.Server.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IMapper _mapper;

        public UserService(UserManager<UserEntity> userManager, IMapper mapper)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public IAsyncEnumerable<User> GetAllAsync()
        {
            return _userManager.Users.ProjectTo<User>(_mapper.ConfigurationProvider).AsAsyncEnumerable();
        }

        public async Task<User> FindByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            return await _userManager.Users.ProjectTo<User>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(u => u.Id == id, cancellationToken).ConfigureAwait(false);
        }

        public Task<bool> ExistsIdAsync(string id, CancellationToken cancellationToken = default)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            return _userManager.Users.AnyAsync(u => u.Id == id, cancellationToken);
        }

        public Task<User> InsertOrUpdateAsync(User entry, CancellationToken cancellationToken = default) => throw new NotImplementedException();

        public Task DeleteAsync(string id, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    }
}

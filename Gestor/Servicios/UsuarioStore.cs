using Microsoft.AspNetCore.Identity;
using Gestor.Models;

namespace Gestor.Servicios
{
    public class UsuarioStore : IUserStore<TipoUsuarios>, IUserEmailStore<TipoUsuarios>,
    IUserPasswordStore<TipoUsuarios>
    {
        private readonly IRepositorioUsuarios repositorioUsuarios;
        public UsuarioStore(IRepositorioUsuarios repositorioUsuarios)
        {
            this.repositorioUsuarios = repositorioUsuarios;

        }
        public async Task<IdentityResult> CreateAsync(TipoUsuarios user, CancellationToken cancellationToken)
        {
            user.IdUser = await repositorioUsuarios.CrearUsuario(user);
            return IdentityResult.Success;
        }

        public Task<IdentityResult> DeleteAsync(TipoUsuarios user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            
        }

        public async Task<TipoUsuarios> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            return await repositorioUsuarios.BuscarUsuarioPorEmail(normalizedEmail);
        }

        public Task<TipoUsuarios> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<TipoUsuarios> FindByNameAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
              return await repositorioUsuarios.BuscarUsuarioPorEmail(normalizedEmail);
        }

        public Task<string> GetEmailAsync(TipoUsuarios user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(TipoUsuarios user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedEmailAsync(TipoUsuarios user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedUserNameAsync(TipoUsuarios user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPasswordHashAsync(TipoUsuarios user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Password);
        }

        public Task<string> GetUserIdAsync(TipoUsuarios user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.IdUser.ToString());
        }

        public Task<string> GetUserNameAsync(TipoUsuarios user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> HasPasswordAsync(TipoUsuarios user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailAsync(TipoUsuarios user, string email, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(TipoUsuarios user, bool confirmed, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedEmailAsync(TipoUsuarios user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.EmailNormalizado = normalizedEmail;
            return Task.CompletedTask;
        }

        public Task SetNormalizedUserNameAsync(TipoUsuarios user, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(TipoUsuarios user, string passwordHash, CancellationToken cancellationToken)
        {
            user.Password = passwordHash;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(TipoUsuarios user, string userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(TipoUsuarios user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<TipoUsuarios?> IUserStore<TipoUsuarios>.FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }


        Task<TipoUsuarios> IUserStore<TipoUsuarios>.FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

    }
}
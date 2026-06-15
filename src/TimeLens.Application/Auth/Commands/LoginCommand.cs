using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TimeLens.Application.Common.Interfaces;
using TimeLens.Domain.Interfaces;

namespace TimeLens.Application.Auth.Commands
{
    public record LoginCommand(string Email, string Password) : IRequest<AuthResult>;
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;

        public LoginCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        public async Task<AuthResult> Handle(LoginCommand request, CancellationToken ct)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email, ct);

            if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                return new AuthResult(false, null, "Email hoac mat khau khong dung");
            }

            if (!user.IsActive)
            {
                return new AuthResult(false, null, "Tai khoan da bi khoa");
            }

            user.RecordLogin();
            await _userRepository.UpdateAsync(user, ct);
            await _userRepository.SaveChangesAsync(ct);

            var token = _jwtService.GenerateToken(user);
            return new AuthResult(true, token, null);
        }
    }
}

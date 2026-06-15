using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TimeLens.Application.Common.Interfaces;
using TimeLens.Domain.Entities;
using TimeLens.Domain.Interfaces;

namespace TimeLens.Application.Auth.Commands
{
    public record RegisterCommand(string Email, string FullName, string Password) : IRequest<AuthResult>; 
    
    public record AuthResult(bool Success, string? Token, string? ErrorMessage);
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;

        public RegisterCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        public async Task<AuthResult> Handle(RegisterCommand request, CancellationToken ct)
        {
            if (await _userRepository.ExistsAsync(request.Email, ct))
            {
                return new AuthResult(false, null, "Email da duoc su dung.");
            }

            var passwordHash = _passwordHasher.Hash(request.Password);
            var user = User.Create(request.Email, request.FullName, passwordHash);

            await _userRepository.AddAsync(user, ct);
            await _userRepository.SaveChangesAsync(ct);

            var token = _jwtService.GenerateToken(user);
            return new AuthResult(true, token, null);
        }
    }
}

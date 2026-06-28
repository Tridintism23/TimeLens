using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TimeLens.Application.Common.Interfaces;
using TimeLens.Domain.Interfaces;
using TimeLens.Domain.Entities;

namespace TimeLens.Application.Users.Commands
{
    public record UpdatePhilosophyCommand(PhilosophyType Philosophy) : IRequest<bool>;

    public class UpdatePhilosophyCommandHandler : IRequestHandler<UpdatePhilosophyCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUser _currentUser;

        public UpdatePhilosophyCommandHandler(IUserRepository userRepository, ICurrentUser currentUser)
        {
            _userRepository = userRepository;
            _currentUser = currentUser;
        }

        public async Task<bool> Handle(UpdatePhilosophyCommand request, CancellationToken ct)
        {
            var user = await _userRepository.GetByIdAsync(_currentUser.Id, ct);

            if (user is null)
            {
                return false;
            }

            user.SetPreferredPhilosophy(request.Philosophy);
            await _userRepository.UpdateAsync(user, ct);
            await _userRepository.SaveChangesAsync(ct);

            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TimeLens.Application.Common.Interfaces
{
    public interface ICurrentUser
    {
        string Id { get; }
        string Email { get; }
        bool IsAuthenticated { get; }
    }
}

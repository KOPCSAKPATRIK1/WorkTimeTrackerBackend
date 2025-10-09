using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkTimeTracker.Core.Models.Domain;

namespace WorkTimeTracker.Core.Interfaces.Business
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
    }
}

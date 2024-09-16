using Lib.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Consumer.BusinessLogic
{
    public interface IRuleApplication
    {
        Task<IRuleApplicationResult> Apply(MessageBody body);
    }
}

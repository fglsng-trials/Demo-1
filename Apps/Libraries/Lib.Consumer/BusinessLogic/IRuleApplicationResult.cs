using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Consumer.BusinessLogic
{
    public interface IRuleApplicationResult
    {
        int RequirementMatch { get; set; }
        bool Success { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Consumer.BusinessLogic
{
    public class RuleApplicationResult : IRuleApplicationResult
    {
        public int RequirementMatch { get; set; }
        public bool Success { get; set; }
    }
}

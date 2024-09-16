using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Shared
{
    public class MessageBody
    {
        public long Counter { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}

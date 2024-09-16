using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Consumer.Data.Model
{
    /// <summary>
    /// Data class to store the entity data.
    /// </summary>
    public class Entity
    {
        public int Id { get; init; }
        public long Counter { get; set; }
        public DateTime Timestamp { get; set; }

        public Entity() { }

        public Entity(long counter, DateTime timestamp)
        {
            Counter = counter;
            Timestamp = timestamp;
        }
    }
}

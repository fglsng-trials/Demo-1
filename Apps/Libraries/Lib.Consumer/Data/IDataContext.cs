using Lib.Consumer.Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Consumer.Data
{
    internal interface IDataContext
    {
        DbSet<Entity> Entities { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions.DataStorage
{
    public interface IDataStorageManager<T>
    {
        Task<bool> InsertAsync(T obj);
    }
}

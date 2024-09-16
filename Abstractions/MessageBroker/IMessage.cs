using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions.MessageBroker
{
    public interface IMessage
    {
        Guid Id { get; init; }
        DateTime? Created { get; init; }

        string? Type { get; set; }
        string? Content { get; set; }

        T? TryGetBody<T>();
    }
}

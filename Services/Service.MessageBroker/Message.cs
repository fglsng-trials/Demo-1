using Abstractions.MessageBroker;
using System.Text.Json;

namespace MessageBroker
{
    public record Message : IMessage
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public DateTime? Created { get; init; } = DateTime.UtcNow;

        public string? Type { get; set; }
        public string? Content { get; set; }

        public Message() { }
        public Message(object content)
        {
            Type = content.GetType().Name;
            Content = JsonSerializer.Serialize(content);
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }


        public T? TryGetBody<T>()
        {
            if (Content is null)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(Content);
        }
    }
}

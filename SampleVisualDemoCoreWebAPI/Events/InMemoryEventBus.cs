using Microsoft.Extensions.DependencyInjection;

namespace SampleVisualDemoCoreWebAPI.Events
{
    public class InMemoryEventBus : IEventBus
    {
        private readonly IServiceProvider _serviceProvider;

        public InMemoryEventBus(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent
        {
            var handlers = _serviceProvider.GetServices<IEventHandler<TEvent>>();

            foreach (var handler in handlers)
            {
                try
                {
                    await handler.HandleAsync(@event);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Error in handler {handler.GetType().Name}: {ex.Message}");
                    // You can replace this with proper logging later
                }
            }
        }
    }
}

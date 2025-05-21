namespace SampleVisualDemoCoreWebAPI.Events
{
    public interface IEventBus
    {
        Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent;
    }
}

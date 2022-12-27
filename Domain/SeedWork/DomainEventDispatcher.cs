class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DomainEventDispatcher> _logger;

    public DomainEventDispatcher(IServiceProvider serviceProvider, ILogger<DomainEventDispatcher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task Dispatch<T>(T domainEvent) where T : IDomainEvent
    {
        var listeners = _serviceProvider.GetServices<IDomainEventListener<T>>();
        _logger.LogInformation("Dispatching {eventName} - {number} listenners", domainEvent.GetType().Name, listeners.Count());

        foreach(var listener in listeners)
            await listener.HandleEvent(domainEvent);
    }
}

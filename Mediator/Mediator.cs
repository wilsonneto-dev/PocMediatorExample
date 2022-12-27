namespace PocMediatorExample.Mediator;

public class Mediator : IMediator
{
    private readonly Func<Type, object> _getService;
    private readonly Dictionary<Type, Type> _handlerDictionary;

    public Mediator(Func<Type, object> getService, Dictionary<Type, Type> handlerDictionary)
    {
        _getService = getService;
        _handlerDictionary = handlerDictionary;
    }

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request)
    {
        var requestType = request.GetType();
        if(!_handlerDictionary.ContainsKey(requestType))
            throw new Exception($"Handler not found for {requestType}");
        
        var handlerType = _handlerDictionary.GetValueOrDefault(requestType);
        if(handlerType is null) throw new NullReferenceException($"Handler not found (null returned) for {requestType}");

        var handler = _getService(handlerType);

        if(handler is null) 
            throw new NullReferenceException($"Handler not found (null returned) for {requestType}");

        // above line will throw...
        // return await ((IHandler<IRequest<TResponse>, TResponse>)handler).Handle(request);

        return await (Task<TResponse>) handler.GetType().GetMethod("Handle").Invoke(handler, new[] { request });
    }
}

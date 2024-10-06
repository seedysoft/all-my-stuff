using GoogleMapsLibrary.Maps;

namespace GoogleMapsLibrary.Core;

/// <summary>
/// Base class implementing KVO.
/// The MVCObject constructor is guaranteed to be an empty function, and so you may inherit from MVCObject by writing MySubclass.prototype = new google.maps.MVCObject();.
/// Unless otherwise noted, this is not true of other classes in the API, and inheriting from other classes in the API is not supported.
/// </summary>
/// <see href="https://developers.google.com/maps/documentation/javascript/reference/event#MVCObject"/>
public abstract class MVCObject(JsObjectRef jsObjectRef) : IDisposable
{
    protected JsObjectRef _jsObjectRef { get; private set; } = jsObjectRef;
    private readonly Dictionary<string, List<MapEventListener>> EventListeners = [];
    private bool _isDisposed;

    private void AddEvent(string eventName, MapEventListener listener)
    {
        if (!EventListeners.TryGetValue(eventName, out List<MapEventListener>? collection))
        {
            collection = [];
            EventListeners.Add(eventName, collection);
        }

        collection.Add(listener);
    }

    public async Task<MapEventListener> AddListener(string eventName, Action handler)
    {
        JsObjectRef listenerRef = await _jsObjectRef.InvokeWithReturnedObjectRefAsync(
            "addListener", eventName, handler);

        var eventListener = new MapEventListener(listenerRef);
        AddEvent(eventName, eventListener);
        return eventListener;
    }

    public async Task<MapEventListener> AddListener<T>(string eventName, Action<T> handler)
    {
        JsObjectRef listenerRef = await _jsObjectRef.InvokeWithReturnedObjectRefAsync(
            "addListener", eventName, handler);

        var eventListener = new MapEventListener(listenerRef);
        AddEvent(eventName, eventListener);
        return eventListener;
    }

    //Note: Might want to wrap the handler with our own handler to make sure that we dispose the event after trigger?
    public async Task<MapEventListener> AddListenerOnce(string eventName, Action handler)
    {
        JsObjectRef listenerRef = await _jsObjectRef.InvokeWithReturnedObjectRefAsync(
            "addListenerOnce", eventName, handler);

        var eventListener = new MapEventListener(listenerRef);
        AddEvent(eventName, eventListener);
        return eventListener;
    }

    public async Task<MapEventListener> AddListenerOnce<T>(string eventName, Action<T> handler)
    {
        JsObjectRef listenerRef = await _jsObjectRef.InvokeWithReturnedObjectRefAsync(
            "addListenerOnce", eventName, handler);

        var eventListener = new MapEventListener(listenerRef);
        AddEvent(eventName, eventListener);
        return eventListener;
    }

    public async Task ClearListeners(string eventName)
    {
        if (EventListeners.TryGetValue(eventName, out List<MapEventListener>? listeners))
        {
            foreach (MapEventListener? listener in listeners.Where(listener => !listener.IsRemoved))
                await listener.RemoveAsync();

            //IMHO is better preserving the knowledge that Marker had some EventListeners attached to "eventName" in the past
            //so, instead to clear the list and remove the key from dictionary, I prefer to leave the key with an empty list
            EventListeners[eventName].Clear();
            //EventListeners.Remove(eventName);
        }
    }

    public virtual async ValueTask DisposeAsync()
    {
        // Perform async cleanup.
        await DisposeAsyncCore();

        // Dispose of unmanaged resources.
        Dispose(false);

        // Suppress finalization.
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// This method takes care of disposing the possible event listeners that were added.
    /// It also dispose the JsObjectRef and uses it to remove listeners
    /// </summary>
    protected virtual async ValueTask DisposeAsyncCore()
    {
        foreach (MapEventListener? eventListener in EventListeners.SelectMany(listener => listener.Value))
        {
            if (eventListener.IsRemoved)
            {
                continue;
            }

            await eventListener.DisposeAsync();
        }

        EventListeners.Clear();
        _ = await _jsObjectRef.DisposeAsync();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            //_jsObjectRef.Dispose();

            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _isDisposed = true;
        }
    }

    public virtual void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

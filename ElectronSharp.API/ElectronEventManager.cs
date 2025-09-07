using System;
using ElectronSharp.API.Entities;
// ReSharper disable PossibleNullReferenceException
// ReSharper disable AccessToModifiedClosure

namespace ElectronSharp.API;

internal static class ElectronEventManager
{
    internal static void AddEvent(string eventName, object id, Action callback, Action value, string suffix = "", bool emit = true)
    {
        if (callback == null)
        {
            BridgeConnector.On(eventName + id, () => { callback(); });
            if (emit) BridgeConnector.Emit($"register-{eventName}{suffix}", id);
            callback += value;
        }
    }
    
    internal static void AddEventWithSuffix(string eventName, object id, Action callback, Action value)
    {
        AddEvent(eventName, id, callback, value, "-event");
    }
    
    internal static void AddEvent<T>(string eventName, object id, Action<T> callback, Action<T> value, string suffix = "", bool emit = true)
    {
        if (callback == null)
        {
            BridgeConnector.On<T>(eventName + id, (args) => { callback(args); });
            if (emit) BridgeConnector.Emit($"register-{eventName}{suffix}", id);
            callback += value;
        }
    }
    
    internal static void AddEventWithSuffix<T>(string eventName, object id, Action<T> callback, Action<T> value)
    {
        AddEvent(eventName, id, callback, value, "-event");
    }

    internal static void AddEventNoEmit<T>(string eventName, object id, Action<T> callback, Action<T> value)
    {
        AddEvent(eventName, id, callback, value, emit: false);
    }
    
    internal static void RemoveEvent(string eventName, object id, Action callback, Action value)
    {
        callback -= value;

        if (callback == null) BridgeConnector.Off(eventName + id);
    }
    
    internal static void RemoveEvent<T>(string eventName, object id, Action<T> callback, Action<T> value)
    {
        callback -= value;
        if (callback == null) BridgeConnector.Off(eventName + id);
    }

    internal static void AddTrayEvent(string eventName, object id, Action<TrayClickEventArgs, Rectangle> callback, Action<TrayClickEventArgs, Rectangle> value)
    {
        if (callback == null)
        {
            BridgeConnector.On<TrayClickEventResponse>(eventName + id, (result) => { callback(result.eventArgs, result.bounds); });
            BridgeConnector.Emit($"register-{eventName}", id);
            callback += value;
        }
    }
    
    internal static void RemoveTrayEvent(string eventName, object id, Action<TrayClickEventArgs, Rectangle> callback, Action<TrayClickEventArgs, Rectangle> value)
    {
        callback -= value;
        if (callback == null) BridgeConnector.Off(eventName + id);
    }

    internal static void AddScreenEvent(string eventName, object id, Action<Display, string[]> callback, Action<Display, string[]> value)
    {
        if (callback == null)
        {
            BridgeConnector.On<DisplayChanged>(eventName + id, (args) => { callback(args.display, args.metrics); });
            BridgeConnector.Emit($"register-{eventName}", id);
            callback += value;
        }
    }
    
    internal static void RemoveScreenEvent(string eventName, object id, Action<Display, string[]> callback, Action<Display, string[]> value)
    {
        callback -= value;
        if (callback == null) BridgeConnector.Off(eventName + id);
    }

}
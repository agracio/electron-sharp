using System;
using ElectronSharp.API.Entities;
// ReSharper disable PossibleNullReferenceException
// ReSharper disable AccessToModifiedClosure

namespace ElectronSharp.API;

internal static class ElectronEventManager
{
    internal static void AddEvent(string eventName, object id, Action callback, Action value)
    {
        if (callback == null)
        {
            BridgeConnector.On(eventName + id, () => { callback(); });
            BridgeConnector.Emit($"register-{eventName}", id);
            callback += value;
        }
    }

    internal static void RemoveEvent(string eventName, object id, Action callback, Action value)
    {
        callback -= value;

        if (callback == null)
        {
            BridgeConnector.Off(eventName + id);
        }
    }
    
    internal static void AddEvent<T>(string eventName, object id, Action<T> callback, Action<T> value)
    {
        if (callback == null)
        {
            BridgeConnector.On<T>(eventName + id, (args) =>
            {
                callback(args);
            });
            BridgeConnector.Emit($"register-{eventName}", id);
            callback += value;
        }
    }
    
    internal static void RemoveEvent<T>(string eventName, object id, Action<T> callback, Action<T> value)
    {
        callback -= value;
        if (callback == null)
            BridgeConnector.Off(eventName + id);
    }

    internal static void AddTrayEvent<TEventArgs, TEntity>(string eventName, object id, Action<TrayClickEventArgs, Rectangle> callback, Action<TrayClickEventArgs, Rectangle> value)
        where TEventArgs : TrayClickEventArgs 
        where TEntity : Rectangle
    {
        if (callback == null)
        {
            BridgeConnector.On<TrayClickEventResponse>(eventName + id, (result) =>
            {
                callback(result.eventArgs, result.bounds);
            });
            BridgeConnector.Emit($"register-{eventName}", id);
            callback += value;
        }
    }
    
    internal static void RemoveTrayEvent<TEventArgs, TEntity>(string eventName, object id, Action<TrayClickEventArgs, Rectangle> callback, Action<TrayClickEventArgs, Rectangle> value) where TEventArgs : TrayClickEventArgs where TEntity : Rectangle
    {
        callback -= value;
        if (callback == null)
            BridgeConnector.Off(eventName + id);
    }

}
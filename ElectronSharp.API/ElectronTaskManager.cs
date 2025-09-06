using System;
using System.Threading.Tasks;

namespace ElectronSharp.API;

internal class TaskReturnArguments<T>(string guid, TaskCompletionSource<T> taskCompletionSource)
{
    internal string Guid { get; } = guid;
    internal TaskCompletionSource<T> TaskCompletionSource { get; } = taskCompletionSource;
}

internal static class ElectronTaskManager
{
    internal static Task CreateTask(string eventName, object id, object args = null)
    {
        var taskReturnArguments = CreateNullTask(eventName);
        if (args != null)
        {
            BridgeConnector.Emit(eventName, id, args, taskReturnArguments.Guid);
        }
        else
        {
            BridgeConnector.Emit(eventName, id, taskReturnArguments.Guid);              
        }
        return taskReturnArguments.TaskCompletionSource.Task;
    }
    
    internal static Task<T> CreateTask<T>(string eventName, object id, bool noGuid = false)
    {
        var taskReturnArguments = CreateResultTask<T>(eventName);
        if (noGuid)
        {
            BridgeConnector.Emit(eventName, id);
        }
        else
        {
            BridgeConnector.Emit(eventName, id, taskReturnArguments.Guid);
        }
        return taskReturnArguments.TaskCompletionSource.Task;
    }

    internal static Task<T> CreateTask<T>(string eventName, object id, object arg1, object arg2 = null)
    {
        var taskReturnArguments = CreateResultTask<T>(eventName);
        var argument2 = arg2 ?? taskReturnArguments.Guid;
        BridgeConnector.Emit(eventName, id, arg1, argument2);
        return taskReturnArguments.TaskCompletionSource.Task;
    }

    // internal static Task<T> CreateTask<T>(string eventName, object arg1, object arg2 = null)
    // {
    //     var taskReturnArguments = CreateResultTask<T>(eventName, true);
    //     if (arg2 == null)
    //     {
    //         BridgeConnector.Emit(eventName, arg1);
    //     }
    //     else
    //     {
    //         BridgeConnector.Emit(eventName, arg1, arg2);
    //     }
    //
    //     return taskReturnArguments.TaskCompletionSource.Task;
    // }
    
    internal static Task<string> CreateStringTask(string eventName, object id, object args = null)
    {
        var taskReturnArguments = CreateResultTask(eventName);
        if (args != null)
        {
            BridgeConnector.Emit(eventName, id, args, taskReturnArguments.Guid);
        }
        else
        {
            BridgeConnector.Emit(eventName, id, taskReturnArguments.Guid);              
        }

        return taskReturnArguments.TaskCompletionSource.Task;
    }

    private static TaskReturnArguments<object> CreateNullTask(string eventName)
    {
        var taskCompletionSource = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
        var guid = Guid.NewGuid().ToString();
        BridgeConnector.On($"{eventName}-completed" + guid, () =>
        {
            BridgeConnector.Off($"{eventName}-completed" + guid);
            taskCompletionSource.SetResult(null);
        });
        return new TaskReturnArguments<object>(guid, taskCompletionSource);
    }
    
    private static TaskReturnArguments<T> CreateResultTask<T>(string eventName, bool noGuid = false)
    {
        var taskCompletionSource = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        var guid = noGuid ? null : Guid.NewGuid().ToString();
        BridgeConnector.On<T>($"{eventName}-completed" + guid, (result) =>
        {
            BridgeConnector.Off($"{eventName}-completed" + guid);
            taskCompletionSource.SetResult(result);
        });

        return new TaskReturnArguments<T>(guid, taskCompletionSource);
    }
    
    private static TaskReturnArguments<string> CreateResultTask(string eventName, bool noGuid = false)
    {
        var taskCompletionSource = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        var guid = noGuid ? null : Guid.NewGuid().ToString();
        BridgeConnector.On<string>($"{eventName}-completed" + guid, (result) =>
        {
            BridgeConnector.Off($"{eventName}-completed" + guid);
            taskCompletionSource.SetResult(result.ToString());
        });

        return new TaskReturnArguments<string>(guid, taskCompletionSource);
    }
    

}
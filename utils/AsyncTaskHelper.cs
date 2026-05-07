using Godot;
using System;
using System.Threading.Tasks;

public static class AsyncTaskHelper
{
    public static void FireAndForget(Task task, string context)
    {
        if (task == null)
        {
            return;
        }

        _ = ObserveAsync(task, context);
    }

    public static Task RunLogged(Task task, string context)
    {
        if (task == null)
        {
            return Task.CompletedTask;
        }

        return LogAndRethrowAsync(task, context);
    }

    private static async Task ObserveAsync(Task task, string context)
    {
        try
        {
            await task;
        }
        catch (OperationCanceledException)
        {
            // Cancellation is an expected control flow in cooperative async operations.
        }
        catch (Exception ex)
        {
            GD.PushError($"[AsyncTaskHelper] Unhandled fire-and-forget task exception in '{context}': {ex}");
        }
    }

    private static async Task LogAndRethrowAsync(Task task, string context)
    {
        try
        {
            await task;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            GD.PushError($"[AsyncTaskHelper] Task exception in '{context}': {ex}");
            throw;
        }
    }
}

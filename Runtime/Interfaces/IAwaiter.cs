using System.Runtime.CompilerServices;

namespace Lab5Games.ScheduleKit
{
    public interface IAwaiter : INotifyCompletion
    {
        bool IsCompleted { get; }
        void GetResult();
    }

    public interface IAwaiter<TResult> : INotifyCompletion
    {
        bool IsCompleted { get; }
        TResult GetResult();
    }
}


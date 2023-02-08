using System;

namespace Lab5Games.ScheduleKit
{
    public abstract class Schedule 
    {
        public enum States
        {
            NotStarted,
            InProgress,
            Completed,
            Canceled
        }

        public States state { get; protected set; } = States.NotStarted;

        public bool paused { get; private set; } = false;

        public delegate void ScheduleDelegate(Schedule schedule);
        public event ScheduleDelegate onComplete;
        public event ScheduleDelegate onCancel;


        public void Start()
        {
            if(state != States.NotStarted)
            {
                throw new System.Exception("Failed to start the schedule, state must be 'NotStarted'");
            }

            if (ScheduleSystem.RegisterSchedule(this))
            {
                OnStart();
                state = States.InProgress;
            }
            else
            {
                throw new System.Exception("Failed to start the schedule");
            }
        }

        protected virtual void OnStart() { }

        public virtual void Complete()
        {
            state = States.Completed;
        }

        internal void Complete_Internal()
        {
            onComplete?.Invoke(this);
        }

        public virtual void Cancel()
        {
            state = States.Canceled;
        }

        internal void Cancel_Internal()
        {
            onCancel?.Invoke(this);
        }

        public virtual void Pause()
        {
            paused = true;
        }

        public virtual void Unpause()
        {
            paused = false;
        }

        public virtual void Tick(float deltaTime) { }
    }

    public class ScheduleTask : IAwaiter<Schedule>, IAwaitable<ScheduleTask, Schedule>
    {
        Schedule m_Schedule;

        public ScheduleTask(Schedule schedule)
        {
            m_Schedule = schedule;
        }

        public bool IsCompleted => m_Schedule.state == Schedule.States.Completed || m_Schedule.state == Schedule.States.Canceled;

        public Schedule GetResult() => m_Schedule;

        public void OnCompleted(Action continuation)
        {
            m_Schedule.onComplete += x => continuation();
            m_Schedule.onCancel += x => continuation();
        }

        public ScheduleTask GetAwaiter() => this;
    }
}

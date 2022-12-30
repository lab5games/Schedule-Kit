
namespace Lab5Games.Schedules
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


        public virtual void Start()
        {
            if(state != States.NotStarted)
            {
                throw new System.Exception("Failed to start the schedule, state must be 'NotStarted'");
            }

            if (ScheduleSystem.RegisterSchedule(this))
            {
                state = States.InProgress;
            }
            else
            {
                throw new System.Exception("Failed to start the schedule");
            }
        }

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
}

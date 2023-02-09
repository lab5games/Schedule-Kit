using System;

namespace Lab5Games.ScheduleKit
{
    public class WaitAllSchedule : Schedule, IAwaiter<ScheduleReport>, IAwaitable<WaitAllSchedule, ScheduleReport>
    {
        public static WaitAllSchedule Create(Schedule[] schedules, bool autoStart = true)
        {
            WaitAllSchedule schedule = new WaitAllSchedule(schedules);

            if (autoStart)
                schedule.Start();

            return schedule;
        }


        Schedule[] m_Schedules;

        private WaitAllSchedule(Schedule[] schedules)
        {
            if(schedules == null)
                throw new NullReferenceException(nameof(schedules));

            m_Schedules = new Schedule[schedules.Length];
            Array.Copy(schedules, m_Schedules, schedules.Length);
        }

        public override void Tick(float deltaTime)
        {
            foreach(var schedule in m_Schedules)
            {
                if(schedule.state == States.Completed ||
                    schedule.state == States.Canceled)
                {
                    continue;
                }
                else
                {
                    return;
                }
            }

            Complete();
        }

        public override void Cancel()
        {
            base.Cancel();

            foreach(var schedule in m_Schedules)
            {
                if (schedule.state == States.InProgress)
                    schedule.Cancel();
            }
        }

        public bool IsCompleted => state == States.Canceled || state == States.Completed;

        public ScheduleReport GetResult()
        {
            return new ScheduleReport(this);
        }

        public void OnCompleted(Action continuation)
        {
            onCancel += x => continuation();
            onComplete += x => continuation();
        }

        public WaitAllSchedule GetAwaiter()
        {
            return this;
        }
    }
}

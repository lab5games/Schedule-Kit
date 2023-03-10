using System;

namespace Lab5Games.ScheduleKit
{
    public class TimeSchedule : Schedule, IAwaiter<ScheduleReport>, IAwaitable<TimeSchedule, ScheduleReport>
    {
        public static TimeSchedule Create(float seconds, bool autoStart = true)
        {
            TimeSchedule schedule = new TimeSchedule(seconds);

            if (autoStart)
                schedule.Start();

            return schedule;
        }

        float m_time;
        public float remainingTime => m_time;

        private TimeSchedule(float time)
        {
            m_time = time;
        }

        public override void Tick(float deltaTime)
        {
            m_time -= deltaTime;

            if (m_time <= 0)
                Complete();
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

        public TimeSchedule GetAwaiter()
        {
            return this;
        }
    }
}

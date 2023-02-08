using System;

namespace Lab5Games.ScheduleKit
{
    public class TimeSchedule : Schedule
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

        private ScheduleTask m_Task;
        public ScheduleTask Task
        {
            get
            {
                if (m_Task == null)
                    m_Task = new ScheduleTask(this);

                return m_Task;
            }
        }

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
    }
}

using System;
using System.Collections;
using UnityEngine;

namespace Lab5Games.ScheduleKit
{
    public class CoroutineSchedule : Schedule
    {
        public static CoroutineSchedule Create(IEnumerator routine, bool autoStart = true)
        {
            CoroutineSchedule schedule = new CoroutineSchedule(routine);

            if (autoStart)
                schedule.Start();

            return schedule;
        }

        IEnumerator m_routine;
        Coroutine m_coroutine;

        WaitForEndOfFrame m_waitForEndOfFrame = new WaitForEndOfFrame();

        public bool running { get; private set; } = false;

        private CoroutineSchedule(IEnumerator routine)
        {
            m_routine = routine;
        }

        protected override void OnStart()
        {
            m_coroutine = ScheduleSystem.ProxyStartCoroutine(Routine());
        }

        public override void Complete()
        {
            Debug.LogWarning("[CoroutineSchedule] The operation is invalid!");
        }

        public override void Cancel()
        {
            base.Cancel();

            if(running)
            {
                running = false;
                ScheduleSystem.ProxyStopCoroutine(m_coroutine);
            }                
        }

        IEnumerator Routine()
        {
            running = true;
            
            while(running)
            {
                if(paused)
                {
                    yield return m_waitForEndOfFrame;
                }
                else
                {
                    if(m_routine.MoveNext())
                    {
                        yield return m_waitForEndOfFrame;
                    }
                    else
                    {
                        running = false;
                    }
                }
            }

            state = States.Completed;
        }
    }
}

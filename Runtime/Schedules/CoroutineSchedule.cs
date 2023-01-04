using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.AI;

namespace Lab5Games.Schedules
{
    public class CoroutineSchedule : Schedule, IAwaiter, IAwaitable<CoroutineSchedule>
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
            Debug.LogWarning("[CoroutineSchedule] This operation is invalid!");
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

        public bool IsCompleted => state == States.Completed || state == States.Canceled;

        public void GetResult()
        {
        }

        public void OnCompleted(Action continuation)
        {
            onCancel += (x) => { continuation(); };
            onComplete += (x) => { continuation(); };
        }

        public CoroutineSchedule GetAwaiter()
        {
            return this;
        }
    }
}

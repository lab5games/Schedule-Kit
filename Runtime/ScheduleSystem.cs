using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lab5Games.ScheduleKit
{
    public class ScheduleSystem : MonoBehaviour
    {
        private static ScheduleSystem m_instance = null;

        static ScheduleSystem current
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = FindObjectOfType<ScheduleSystem>();

                    if (m_instance == null)
                    {
                        GameObject go = new GameObject("[ScheduleSystem]");
                        m_instance = go.AddComponent<ScheduleSystem>();

                        Debug.LogWarning("[ScheduleSystem] The system has been created automatically");
                    }

                    if (m_instance == null)
                    {
                        throw new Exception("[ScheduleSystem] Failed to create the system");
                    }
                }

                return m_instance;
            }
        }

        public static void CancelScheduleAll()
        {
            current?.CancelScheduleAll_Internal();
        }

        public static bool RegisterSchedule(Schedule schedule)
        {
            return (current?.RegisterSchedule_Internal(schedule)).Value;
        }

        public static Coroutine ProxyStartCoroutine(IEnumerator routine)
        {
            return current?.StartCoroutine_Internal(routine);
        }

        public static void ProxyStopCoroutine(Coroutine routine)
        {
            current?.StopCoroutine_Internal(routine);
        }

        float m_deltaTime;
        List<Schedule> m_schedules = new List<Schedule>();

        private void CancelScheduleAll_Internal()
        {
            for(int i=m_schedules.Count-1; i>=0; i--)
            {
                m_schedules[i].Cancel();

                if (m_schedules[i].state == Schedule.States.Canceled)
                    m_schedules.RemoveAt(i);
            }

            Debug.Log("[ScheduleSystem] Cancel schedule all", this);
        }

        private bool RegisterSchedule_Internal(Schedule schedule)
        {
            if (schedule == null)
                throw new ArgumentNullException("schedule is null");

            if (m_schedules.Contains(schedule))
                return false;

            m_schedules.Add(schedule);
            return true;
        }

        private Coroutine StartCoroutine_Internal(IEnumerator routine)
        {
            return StartCoroutine(routine);
        }

        private void StopCoroutine_Internal(Coroutine routine)
        {
            StopCoroutine(routine);
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            m_instance = null;
            Debug.LogWarning("[ScheduleSystem] The system has been destroyed");
        }

        private void FixedUpdate()
        {
            m_deltaTime = Time.deltaTime;

            // tick schedules
            for(int i=m_schedules.Count-1; i>=0; i--)
            {
                Schedule schedule = m_schedules[i];

                if (schedule.paused)
                    continue;

                switch(schedule.state)
                {
                    case Schedule.States.InProgress:
                        schedule.Tick(m_deltaTime);
                        break;

                    case Schedule.States.Completed:
                        schedule.Complete_Internal();
                        m_schedules.RemoveAt(i);
                        break;

                    case Schedule.States.Canceled:
                        schedule.Cancel_Internal();
                        m_schedules.RemoveAt(i);
                        break;
                }
            }
        }

        
    }
}

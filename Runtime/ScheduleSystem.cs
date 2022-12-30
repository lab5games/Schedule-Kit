using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace Lab5Games.Schedules
{
    public class ScheduleSystem : MonoBehaviour
    {
        private static ScheduleSystem m_instance = null;

        internal static ScheduleSystem GetInstance()
        {
            if(m_instance == null)
            {
                m_instance = FindObjectOfType<ScheduleSystem>();


                if(m_instance == null)
                {
                    GameObject go = new GameObject("[ScheduleSystem]");
                    DontDestroyOnLoad(go);
                    
                    m_instance = go.AddComponent<ScheduleSystem>();

                    Debug.LogWarning("[ScheduleSystem] The system has been created automatically");
                }

                if (m_instance == null)
                {
                    Debug.LogError("[ScheduleSystem] Failed to create the system");
                }
            }

            return m_instance;
        }

        public static void CancelScheduleAll()
        {
            GetInstance()?.CancelScheduleAll_Internal();
        }

        public static bool RegisterSchedule(Schedule schedule)
        {
            return (GetInstance()?.RegisterSchedule_Internal(schedule)).Value;
        }

        public static Coroutine ProxyStartCoroutine(IEnumerator routine)
        {
            return GetInstance()?.StartCoroutine_Internal(routine);
        }

        public static void ProxyStopCoroutine(Coroutine routine)
        {
            GetInstance()?.StopCoroutine_Internal(routine);
        }

        public static void RegisterTickModule(ITickModule module)
        {
            GetInstance()?.RegisterTickModule_Internal(module);
        }

        public static void UnregisterTickModule(ITickModule module)
        {
            GetInstance().UnregisterTickModule_Internal(module);
        }

        float m_deltaTime;
        List<Schedule> m_schedules = new List<Schedule>();
        List<ITickModule> m_tickModules = new List<ITickModule>();

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

        private void RegisterTickModule_Internal(ITickModule module)
        {
            m_tickModules.Add(module);
            m_tickModules.Sort((a, b) => a.order.CompareTo(b.order));
        }

        private void UnregisterTickModule_Internal(ITickModule module)
        {
            m_tickModules.Remove(module);
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

            for(int i=0; i<m_tickModules.Count; i++)
            {
                m_tickModules[i].Tick(m_deltaTime);
            }
        }

        
    }
}

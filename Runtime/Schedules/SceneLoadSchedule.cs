using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lab5Games.ScheduleKit
{
    public class SceneLoadSchedule : Schedule
    {
        public static SceneLoadSchedule Create(string sceneName, bool isAdditive, bool autoStart = true)
        {
            SceneLoadSchedule schedule = new SceneLoadSchedule(sceneName, isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);

            if(autoStart)
                schedule.Start();

            return schedule;
        }

        LoadSceneMode m_LoadMode;
        AsyncOperation m_Async;

        string m_LoadSceneName;
        public string sceneName => m_LoadSceneName;

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

        public bool isLoaded => state == States.Completed;
        
        public float progress { get; private set; }

        private SceneLoadSchedule(string sceneName, LoadSceneMode loadMode)
        {
            m_LoadSceneName = sceneName;
            m_LoadMode = loadMode;
        }

        protected override void OnStart()
        {
            m_Async = SceneManager.LoadSceneAsync(m_LoadSceneName, m_LoadMode);
            m_Async.allowSceneActivation = false;
        }

        public override void Tick(float deltaTime)
        {
            if(m_Async != null)
            {
                if(m_Async.progress < 0.9f)
                {
                    progress = m_Async.progress;
                }
                else
                {
                    progress = 1f;
                    state = States.Completed;
                    Debug.Log($"[SceneLoadSchedule] The scene is loaded: {sceneName}");
                }
            }
        }

        public void Activeate()
        {
            if(!isLoaded)
            {
                Debug.LogWarning("[SceneLoadSchedule] The loading is still in porgress");
                return;
            }

            m_Async.allowSceneActivation = true;
            Debug.Log($"[SceneLoadSchedule] The scene is activated: {sceneName}");
        }

        public override void Complete()
        {
            Debug.LogWarning("[SceneLoadSchedule] The operation is invalid!");
        }

        public override void Cancel()
        {
            Debug.LogWarning("[SceneLoadSchedule] The operation is invalid!");
        }
    }
}

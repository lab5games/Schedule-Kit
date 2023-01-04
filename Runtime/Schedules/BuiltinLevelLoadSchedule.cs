using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lab5Games.Schedules
{
    public class BuiltinLevelLoadSchedule : Schedule, IAwaiter<Scene>, IAwaitable<BuiltinLevelLoadSchedule, Scene>
    {
        public static BuiltinLevelLoadSchedule Create(string path, LoadSceneMode loadMode, bool visibleOnLoaded, bool autoStart = true)
        {
            BuiltinLevelLoadSchedule schedule = new BuiltinLevelLoadSchedule(path, loadMode, visibleOnLoaded);

            if (autoStart)
                schedule.Start();

            return schedule;
        }

        public readonly string path;
        public readonly LoadSceneMode loadMode;
        public readonly bool visibleOnLoaded;

        private bool m_levelLoadCompleted = false;

        private AsyncOperation m_asyncOp;
        public AsyncOperation asyncOperation => m_asyncOp;

        public Scene scene { get; private set; }
        public float progress => m_asyncOp == null ? 0f : Mathf.Clamp01(m_asyncOp.progress / FullProgress);

        public event ScheduleDelegate onLoadUpdate;
        public event ScheduleDelegate onLoadCompleted;

        const float FullProgress = .9f;

        private BuiltinLevelLoadSchedule(string path, LoadSceneMode loadMode, bool visibleOnLoaded)
        {
            this.path = path;
            this.loadMode = loadMode;
            this.visibleOnLoaded = visibleOnLoaded;
        }

        public void ActivateLevel()
        {
            if(m_asyncOp == null || !m_levelLoadCompleted)
            {
                Debug.LogWarning($"[BuiltinLevelLoadSchedule] The level has not been loaded yet, state= {state}");
            }

            m_asyncOp.allowSceneActivation = true;
        }

        protected override void OnStart()
        {
            try
            {
                m_asyncOp = SceneManager.LoadSceneAsync(path, loadMode);
                m_asyncOp.allowSceneActivation = false;
                m_asyncOp.completed += x => Complete();
            }
            catch(Exception ex)
            {
                Debug.LogError($"[BuiltinLevelLoadSchedule] Loading failed, path= {path}");
                Debug.LogException(ex);
            }
        }

        public override void Tick(float deltaTime)
        {
            if (m_levelLoadCompleted)
                return;

            if (!m_asyncOp.isDone)
            {
                onLoadUpdate?.Invoke(this);

                if (m_asyncOp.progress >= FullProgress)
                {
                    m_levelLoadCompleted = true;
                    onLoadCompleted?.Invoke(this);

                    if (visibleOnLoaded)
                        ActivateLevel();
                }
            }
        }

        public override void Complete()
        {
            base.Complete();

            scene = SceneManager.GetSceneByPath(path);
        }

        public override void Cancel()
        {
            Debug.LogWarning("[BuiltinLevelLoadSchedule] This operation is invalid!");
        }

        public override void Pause()
        {
            Debug.LogWarning("[BuiltinLevelLoadSchedule] This operation is invalid!");
        }

        public override void Unpause()
        {
            Debug.LogWarning("[BuiltinLevelLoadSchedule] This operation is invalid!");
        }

        public bool IsCompleted => state == States.Completed;

        public Scene GetResult() => scene;

        public void OnCompleted(Action continuation)
        {
            onComplete += x => continuation();
        }

        public BuiltinLevelLoadSchedule GetAwaiter()
        {
            return this;
        }
    }
}

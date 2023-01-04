using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Lab5Games.Schedules
{
    // references
    // https://github.com/JohannesMP/unity-scene-reference/blob/master/unity-scene-reference/Assets/source/SceneReference.cs

    [System.Serializable]
    public class BuiltinLevelReference : ISerializationCallbackReceiver
    {
        [SerializeField]
        private string m_scenePath;
        public string path => m_scenePath;

        [System.Serializable]
        public struct LoadOptions
        {
            public bool additive;
            public bool visibleOnLoaded;

            public LoadSceneMode loadSceneMode => additive ? LoadSceneMode.Additive : LoadSceneMode.Single;
        }

        public LoadOptions loadOptions;

        private BuiltinLevelLoadSchedule m_loadSchedule;

        public BuiltinLevelLoadSchedule LoadAsync()
        {
            return BuiltinLevelLoadSchedule.Create(path, loadOptions.loadSceneMode, loadOptions.visibleOnLoaded);
        }

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            HandleBeforeSerialize();
#endif
        }

        public void OnAfterDeserialize()
        {
#if UNITY_EDITOR
            EditorApplication.update += HandleAfterSerialize;
#endif
        }


#if UNITY_EDITOR
        [SerializeField]
        private SceneAsset m_sceneAsset;

        public bool IsValidSceneAsset => m_sceneAsset != null;


        private SceneAsset GetSceneAssetFromPath()
        {
            return string.IsNullOrEmpty(m_scenePath) ? null : AssetDatabase.LoadAssetAtPath<SceneAsset>(m_scenePath);
        }

        private string GetScenePathFromAsset()
        {
            return IsValidSceneAsset ? AssetDatabase.GetAssetPath(m_sceneAsset) : string.Empty;
        }

        private void HandleBeforeSerialize()
        {
            if (IsValidSceneAsset == false && string.IsNullOrEmpty(m_scenePath) == false)
            {
                m_sceneAsset = GetSceneAssetFromPath();

                if (m_sceneAsset == null)
                    m_scenePath = string.Empty;

                EditorSceneManager.MarkAllScenesDirty();
            }
            else
            {
                m_scenePath = GetScenePathFromAsset();
            }
        }

        private void HandleAfterSerialize()
        {
            EditorApplication.update -= HandleAfterSerialize;

            if (IsValidSceneAsset)
                return;

            if (string.IsNullOrEmpty(m_scenePath))
                return;

            m_sceneAsset = GetSceneAssetFromPath();

            if (m_sceneAsset == null)
                m_scenePath = string.Empty;

            if (!EditorApplication.isPlaying)
                EditorSceneManager.MarkAllScenesDirty();
        }
#endif
    }
}

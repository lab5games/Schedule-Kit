using UnityEngine;
using UnityEditor;

namespace Lab5Games.Schedules.LevelManagement.Ediotr
{
    // https://github.com/JohannesMP/unity-scene-reference/blob/master/unity-scene-reference/Assets/source/SceneReference.cs

    [CustomPropertyDrawer(typeof(BuiltinLevelReference))]
    public class BuiltinLevelReferencePropertyDrawer : PropertyDrawer
    {

        private static readonly RectOffset boxPadding = EditorStyles.helpBox.padding;


        // Made these two const btw
        private const float PAD_SIZE = 2f;
        private const float FOOTER_HEIGHT = 10f;

        private static readonly float lineHeight = EditorGUIUtility.singleLineHeight;
        private static readonly float paddedLine = lineHeight + PAD_SIZE;

        private const float EDIT_BUTTON_WIDTH = 60;

        private static readonly Color32 errorColor = new Color32(255, 55, 0, 255);


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, GUIContent.none, property);
            {
                var sceneAssetProperty = GetSceneAssetProperty(property);
                var optionsProperty = property.FindPropertyRelative("loadOptions");
                var buildScene = GetBuildScene(sceneAssetProperty.objectReferenceValue);

                Color guiColor = GUI.color;
                if (!buildScene.IsValid()) GUI.color = errorColor;

                position.height -= lineHeight;
                position.y += lineHeight;
                
                // draw the Box Background
                position.height -= FOOTER_HEIGHT;
                GUI.Box(EditorGUI.IndentedRect(position), GUIContent.none, EditorStyles.helpBox);
                position = boxPadding.Remove(position);
                position.height = lineHeight;

                // draw label
                Rect rect = GetLabelRect(position);
                EditorGUI.LabelField(rect, new GUIContent(label.text), EditorStyles.boldLabel);

                // draw edit button (open BuildSettings Window)
                GUI.color = guiColor;
                rect.x = position.width - (EDIT_BUTTON_WIDTH / 1.5f);
                rect.width = EDIT_BUTTON_WIDTH;
                
                if(GUI.Button(rect, new GUIContent("Edit")))
                {
                    EditorWindow.GetWindow(typeof(BuildPlayerWindow));
                }

                EditorGUI.indentLevel++;

                //var sceneControlID = GUIUtility.GetControlID(FocusType.Passive);
                if (!buildScene.IsValid()) GUI.color = errorColor;
                EditorGUI.BeginChangeCheck();
                {
                    position.y += paddedLine;
   
                    sceneAssetProperty.objectReferenceValue = EditorGUI.ObjectField(position, sceneAssetProperty.objectReferenceValue, typeof(SceneAsset), false);
                }
                
                if(EditorGUI.EndChangeCheck())
                {
                    if (buildScene.scene == null) GetScenePathProperty(property).stringValue = string.Empty;
                }

                position.y += paddedLine;
                EditorGUI.PropertyField(position, optionsProperty.FindPropertyRelative("additive"), new GUIContent("Additive"));
                position.y += paddedLine;
                EditorGUI.PropertyField(position, optionsProperty.FindPropertyRelative("visibleOnLoaded"), new GUIContent("Visible On Loaded"));

                GUI.color = guiColor;
                EditorGUI.indentLevel--;
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var sceneAssetProperty = GetSceneAssetProperty(property);
            // Add an additional line and check if property.isExpanded
            var lines = 5;
            // If this oneliner is confusing you - it does the same as
            //var line = 3; // Fully expanded and with info
            //if(sceneAssetProperty.objectReferenceValue == null) line = 2;
            //if(!property.isExpanded) line = 1;

            return boxPadding.vertical + lineHeight * lines + PAD_SIZE * (lines - 1) + FOOTER_HEIGHT;
        }

        static SerializedProperty GetSceneAssetProperty(SerializedProperty property)
        {
            return property.FindPropertyRelative("m_sceneAsset");
        }

        static SerializedProperty GetScenePathProperty(SerializedProperty property)
        {
            return property.FindPropertyRelative("m_scenePath");
        }

        static Rect GetFieldRect(Rect position)
        {
            position.width -= EditorGUIUtility.labelWidth;
            position.x += EditorGUIUtility.labelWidth;
            return position;
        }

        static Rect GetLabelRect(Rect position)
        {
            position.width = EditorGUIUtility.labelWidth - PAD_SIZE;
            return position;
        }

        static BuildScene GetBuildScene(Object sceneObj)
        {
            BuildScene entry = new BuildScene()
            {
                buildIndex = -1,
                assetGUID = new GUID(string.Empty)
            };

            if (sceneObj as SceneAsset == null)
                return entry;

            entry.assetPath = AssetDatabase.GetAssetPath(sceneObj);
            entry.assetGUID = new GUID(AssetDatabase.AssetPathToGUID(entry.assetPath));

            var scenes = EditorBuildSettings.scenes;
            for(int sceneIndx = 0; sceneIndx < scenes.Length; sceneIndx++)
            {
                if (entry.assetGUID.Equals(scenes[sceneIndx].guid))
                {
                    entry.scene = scenes[sceneIndx];
                    entry.buildIndex = sceneIndx;

                    return entry;
                }
            }

            return entry;
        }

        public struct BuildScene
        {
            public int buildIndex;
            public GUID assetGUID;
            public string assetPath;
            public EditorBuildSettingsScene scene;

            public bool IsValid()
            {
                return buildIndex != -1;
            }
        }
    }
}

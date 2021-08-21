using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Tile
{
    [CustomEditor(typeof(TileTester))]
    [CanEditMultipleObjects]
    public class TileTestEditor : Editor
    {
        TileTester tiletester;
        SerializedProperty index;

        private void OnEnable()
        {
            tiletester = (TileTester)target;

            index = serializedObject.FindProperty("getIndex");
        }
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("clear All Tile Debug Text")) TileHelper.ClearAllDebugText();

            serializedObject.Update();
            EditorGUILayout.PropertyField(index);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
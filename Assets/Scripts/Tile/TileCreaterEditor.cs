using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Tile
{
    [CustomEditor(typeof(TileCreater))]
    [CanEditMultipleObjects]
    public class TileCreaterEditor : Editor
    {
        private TileCreater tileCreater;
        readonly SerializedProperty[] serializeProperty = new SerializedProperty[PropertyStrings.Length];
        
        // NOTE : 추가하고 싶은 프로퍼티는 여기에 해당 멤버변수명을 추가
        static string[] PropertyStrings = new string[]
        {
            "tilePrefab",
            "tileRange",
            "hexGrid",
            "hexMesh",
            "gridCanvas",
            "cellLabelPrefab",
            "defaultColor",
            "touchedColor",
            "numOfMaxContinentTiles",
            "numOfLeastContinentTiles",
            "influenceOfContinent",
        };

        private void OnEnable()
        {
            tileCreater = (TileCreater)target;
            for (int i = 0; i < PropertyStrings.Length; ++i) {
                serializeProperty[i] = serializedObject.FindProperty(PropertyStrings[i]);
            }
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("create tile map")) tileCreater.CreateFromEditor();
            if (GUILayout.Button("delete all tiles")) TileHelper.ClearAllTiles();
            serializedObject.Update();
            foreach(var property in serializeProperty) {
                EditorGUILayout.PropertyField(property);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
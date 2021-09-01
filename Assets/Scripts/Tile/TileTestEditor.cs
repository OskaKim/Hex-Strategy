using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Tile
{
    [CustomEditor(typeof(TileTester))]
    [CanEditMultipleObjects]
    public class TileTestEditor : Editor
    {
        private TileTester tileTester;
        readonly SerializedProperty[] serializeProperty = new SerializedProperty[PropertyStrings.Length];

        // NOTE : 추가하고 싶은 프로퍼티는 여기에 해당 멤버변수명을 추가
        static string[] PropertyStrings = new string[]
        {
            "findPathStart",
            "findPathEnd",
            "path",
        };

        private void OnEnable()
        {
            tileTester = (TileTester)target;

            for (int i = 0; i < PropertyStrings.Length; ++i)
            {
                serializeProperty[i] = serializedObject.FindProperty(PropertyStrings[i]);
            }
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("path finding"))
            {
                var startTile = TileHelper.GetTile(tileTester.findPathStart);
                var endTile = TileHelper.GetTile(tileTester.findPathEnd);
                tileTester.path = PathFinding.FindPath(startTile, endTile);
            }

            serializedObject.Update();
            foreach (var property in serializeProperty)
            {
                EditorGUILayout.PropertyField(property);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
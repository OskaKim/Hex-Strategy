using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Lovatto.SceneLoader;
using UnityEditorInternal;

[CustomEditor(typeof(bl_SceneLoaderManager))]
public class bl_SceneLoaderManagerEditor : Editor
{
    ReorderableList scenesList, tipsList;


    /// <summary>
    /// 
    /// </summary>
    private void OnEnable()
    {
        scenesList = new ReorderableList(serializedObject, serializedObject.FindProperty("sceneList"), true, true, true, true);
        tipsList = new ReorderableList(serializedObject, serializedObject.FindProperty("tipsList"), true, true, true, true);
        scenesList.drawElementCallback = DrawSceneList;
        scenesList.drawHeaderCallback = (rect) => { EditorGUI.LabelField(rect, "All Scenes"); };
        scenesList.elementHeightCallback = SceneHeight;

        tipsList.drawElementCallback = DrawTips;
        tipsList.drawHeaderCallback = (rect) => { EditorGUI.LabelField(rect, "Tips"); };
        tipsList.elementHeightCallback = TipsHeight;
    }

    /// <summary>
    /// 
    /// </summary>
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        serializedObject.Update();

        scenesList.DoLayoutList();
        GUILayout.Space(10);
        tipsList.DoLayoutList();


        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }
    }

    void DrawSceneList(Rect rect, int index, bool isActive, bool isFocused)
    {
        DrawList("sceneList", rect, index);
    }

    private float SceneHeight(int index)
    {
        return GetPropHeight("sceneList", index);
    }

    void DrawTips(Rect rect, int index, bool isActive, bool isFocused)
    {
        var lw = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 100;
        DrawList("tipsList", rect, index);
        EditorGUIUtility.labelWidth = lw;
    }
    private float TipsHeight(int index) => GetPropHeight("tipsList", index);

    void DrawList(string propName, Rect rect, int index)
    {
        EditorGUI.indentLevel++;
        var element = serializedObject.FindProperty(propName).GetArrayElementAtIndex(index);
        EditorGUI.PropertyField(rect, element, true);
        EditorGUI.indentLevel--;
    }

    private float GetPropHeight(string propName, int index)
    {
        var element = serializedObject.FindProperty(propName).GetArrayElementAtIndex(index);
        if (element.isExpanded)
        {
            return EditorGUI.GetPropertyHeight(element);
        }
        else return EditorGUIUtility.singleLineHeight;
    }
}
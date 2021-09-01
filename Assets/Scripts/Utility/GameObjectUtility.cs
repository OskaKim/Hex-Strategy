using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectUtility : MonoBehaviour {
    public static void DeleteGameObject(GameObject deleteGameObject) {
        if (!deleteGameObject) return;

#if UNITY_EDITOR
        DestroyImmediate(deleteGameObject);
#else
                Destroy(deleteGameObject);
#endif
    }

    public static void ClearComponentListAndDelete<T>(List<T> list) where T : Component {
        if (list.Count == 0) return;

        list.Where(x => x != null)
            .ToList()
            .ForEach(x => DeleteGameObject(x.gameObject));
        list.Clear();
    }

    public static void DeleteGameObjectsFromTags(string[] tags) {
        tags.ForEach(tag => DeleteGameObjectsFromTag(tag));
    }
    public static void DeleteGameObjectsFromTag(string tag) {
        GameObject.FindGameObjectsWithTag(tag).ForEach(x => DeleteGameObject(x));
    }
}
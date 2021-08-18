using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lovatto.SceneLoader
{
    public class bl_SceneLoaderManager : ScriptableObject
    {

        [Header("Scene Manager")]
        public List<bl_SceneLoaderInfo> sceneList = new List<bl_SceneLoaderInfo>();
        public List<string> tipsList = new List<string>();

        public bl_SceneLoaderInfo GetSceneInfo(string scene)
        {
            foreach(bl_SceneLoaderInfo info in sceneList)
            {
                if(info.SceneName == scene)
                {
                    return info;
                }
            }
            
            Debug.Log("Not found any scene with this name: " + scene);
            return null;           
        }

        public bool HasTips
        {
            get
            {
                return (tipsList != null && tipsList.Count > 0);
            }
        }

        public string[] GetSceneNames()
        {
            return sceneList.Select(x => x.SceneName).ToArray();
        }

        public static IEnumerator AsyncLoadData()
        {
            if (_instance == null)
            {
                ResourceRequest rr = Resources.LoadAsync("SceneLoaderManager", typeof(bl_SceneLoaderManager));
                while (!rr.isDone) { yield return null; }
                _instance = rr.asset as bl_SceneLoaderManager;
            }
        }

        public static bl_SceneLoaderManager _instance;
        public static bl_SceneLoaderManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<bl_SceneLoaderManager>("SceneLoaderManager") as bl_SceneLoaderManager;
                }
                return _instance;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            for (int i = 0; i < sceneList.Count; i++)
            {
                if(sceneList[i].SceneAsset != null)
                {
                    sceneList[i].SceneName = sceneList[i].SceneAsset.name;
                }
            }
        }
#endif
    }
}
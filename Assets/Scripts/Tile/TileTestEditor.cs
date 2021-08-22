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
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("clear All Tile Debug Text")) TileHelper.ClearAllDebugText();
            if (GUILayout.Button("show tile continent influence"))
            {
                TileModel.tiles.Where(x => x.ContinentInfluence != 0).All(x =>
                {
                    x.CustomDebugText(x.ContinentInfluence.ToString());
                    return true;
                });
            }
            if (GUILayout.Button("show tile index"))
            {
                TileModel.tiles.All(x =>
                {
                    x.CustomDebugText($"{x.IndexPair.x}/{x.IndexPair.y}");
                    return true;
                });
            }
        }
    }
}
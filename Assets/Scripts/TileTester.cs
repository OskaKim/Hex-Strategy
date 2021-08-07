using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tile
{
    // TODO : 유니티 디버그 기능으로 확인할 수 있도록 개수
    public class TileTester : MonoBehaviour
    {
        [SerializeField] IndexPair getIndex;

        private List<Tile> cacheTiles = new List<Tile>();

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.F))
            {
                foreach(var cacheTile in cacheTiles) {
                    cacheTile.DebugTextToIndex();
                }
                cacheTiles.Clear();

                var tile = TileHelper.GetTile(getIndex);
                tile.CustomDebugText("target");
                cacheTiles.Add(tile);

                Debug.Log("======================");
                foreach (var nearTile in TileHelper.GetNearTiles(tile))
                {
                    Debug.Log($"{nearTile.IndexPair.x} / {nearTile.IndexPair.y})");
                    nearTile.CustomDebugText("near tile");
                    cacheTiles.Add(nearTile);
                }
                Debug.Log("======================");
            }
        }
    }
}
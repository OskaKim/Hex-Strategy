using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Tile;

public class TileData : MonoBehaviour {
    public static void SetTileTerrainData() {
        var allTiles = TileModel.tiles;
        var terrainDic = allTiles.Select(x => new { x.TerrainType, x.IndexPair });

        terrainDic.All(x => {
            Debug.Log($"{x.TerrainType} : {x.IndexPair.X}, {x.IndexPair.Y}");
            return true;
        });
    }
}

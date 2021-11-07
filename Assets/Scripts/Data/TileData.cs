using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Tile;
using System.IO;

[System.Serializable]
public class TileJsonData {
    [System.Serializable]
    public class TileJsonDataElement {
        public int terrainType;
        public int x;
        public int y;
    }

    public List<TileJsonDataElement> tileDataList;
}

public class TileData : MonoBehaviour {
    public static void SetTileTerrainData() {
        var data = new TileJsonData();
        var allTiles = TileModel.tiles;
        data.tileDataList = allTiles.Select(x => new TileJsonData.TileJsonDataElement() { terrainType = (int)x.TerrainType, x = x.IndexPair.X, y = x.IndexPair.Y }).ToList();
        var json = JsonUtility.ToJson(data, true);
        var saveFile = Application.dataPath + "/terrainData.json";
        File.WriteAllText(saveFile, json);
    }
}

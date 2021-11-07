using System.Collections.Generic;
using UnityEngine;
using System.Linq;
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

public class TileDataJsonConverter : MonoBehaviour {

    // NOTE : 타일의 지형 데이터를 json형식으로 컨버트
    public static void SetTileTerrainData(List<Tile.Tile> tiles) {
        var data = new TileJsonData();
        data.tileDataList = tiles.Select(x => new TileJsonData.TileJsonDataElement() { terrainType = (int)x.TerrainType, x = x.IndexPair.X, y = x.IndexPair.Y }).ToList();
        var json = JsonUtility.ToJson(data, true);
        var saveFile = Application.dataPath + "/terrainData.json";
        File.WriteAllText(saveFile, json);
    }
}

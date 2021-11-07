using System.Collections.Generic;
using System.Linq;

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

public class TileDataJsonConverter : JsonConverterBase {

    // NOTE : Ÿ���� ���� �����͸� json�������� ����Ʈ
    public static void SetTileTerrainData(List<Tile.Tile> tiles) {
        var data = new TileJsonData();
        data.tileDataList = tiles.Select(x => new TileJsonData.TileJsonDataElement() { terrainType = (int)x.TerrainType, x = x.IndexPair.X, y = x.IndexPair.Y }).ToList();
        ExportToJson(data, "TileData", "TerrainData.json");
    }
}

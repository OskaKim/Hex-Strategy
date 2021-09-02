using UnityEngine;
using System;

namespace Tile
{
    // NOTE : 기본지형.
    // 기본 지형은 타일이 생성됐을때의 고정치.
    [Serializable]
    public enum TerrainType 
    {
        Field,
        Mountain,
        Ocean,
        NumTerrainType
    }

    // NOTE : 지형특성.
    // 지형 특성은 바뀔 수 있음. 중복의 특성은 가지지 않음.
    [Serializable]
    public enum FeatureType
    {
        Ice,
        Woods,
        Oasis,
        NumFeatureType
    }

    [Serializable]
    public enum ResourceType
    {
        None,
        Food,
        ManPower,
        Science,
        Culture
    }
    struct TileResourceInfo
    {
        public static string[] TileResourcesPath = new string[(int)TerrainType.NumTerrainType] {
            "Tile/hex_field",
            "Tile/hex_mountain",
            "Tile/hex_ocean"
        };
    }
}
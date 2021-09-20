using UnityEngine;
using System;
using System.Collections.Generic;

namespace Tile {
    // NOTE : 기본지형.
    // 기본 지형은 타일이 생성됐을때의 고정치.
    [Serializable]
    public enum TerrainType {
        Field,
        Mountain,
        Ocean,
        NumTerrainType
    }

    // NOTE : 지형특성.
    // 지형 특성은 바뀔 수 있음. 중복의 특성은 가지지 않음.
    [Serializable]
    public enum FeatureType {
        None = -1,
        Ice,
        Jungle,
        Desert,
        Grass,
        NumFeatureType
    }
    // NOTE : 기후.
    [Serializable]
    public enum ClimateType {
        Polar,
        Subarctic,
        Temperate,
        Tropical,
        NumClimateType
    }

    struct FeatureInfo {
        public static Dictionary<FeatureType, List<ClimateType>> climateConditions = new Dictionary<FeatureType, List<ClimateType>>() {
            { FeatureType.Ice, new List<ClimateType>(){ ClimateType.Polar } },
            { FeatureType.Jungle, new List<ClimateType>(){ ClimateType.Tropical } },
            { FeatureType.Desert, new List<ClimateType>(){ ClimateType.Subarctic, ClimateType.Temperate } },
        };
        public static Dictionary<FeatureType, List<TerrainType>> terrainConditions = new Dictionary<FeatureType, List<TerrainType>>() {
            { FeatureType.Jungle, new List<TerrainType>(){ TerrainType.Field } },
            { FeatureType.Desert, new List<TerrainType>(){ TerrainType.Field } },
            { FeatureType.Grass, new List<TerrainType>() { TerrainType.Field } }
        };
    }

    [Serializable]
    public enum ResourceType {
        None,
        Food,
        ManPower,
        Science,
        Culture
    }
    struct TileResourceInfo {
        public static string[] TileResourcesPath = new string[(int)TerrainType.NumTerrainType] {
            "Tile/hex_field",
            "Tile/hex_mountain",
            "Tile/hex_ocean"
        };
    }

    struct TilePropertyInfo {
        public static string[] ContinentNames = new string[] {
        "아프리카",
        "유럽",
        "아시아",
        "북아메리카",
        "남아메리카",
        "오세아니아"
    };
    }

}
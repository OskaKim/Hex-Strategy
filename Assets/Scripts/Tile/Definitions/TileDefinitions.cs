using UnityEngine;
using System;
using System.Collections.Generic;

namespace Tile {
    // NOTE : �⺻����.
    // �⺻ ������ Ÿ���� ������������ ����ġ.
    [Serializable]
    public enum TerrainType {
        Field,
        Mountain,
        Ocean,
        NumTerrainType
    }

    // NOTE : ����Ư��.
    // ���� Ư���� �ٲ� �� ����. �ߺ��� Ư���� ������ ����.
    [Serializable]
    public enum FeatureType {
        None = -1,
        Ice,
        Jungle,
        Desert,
        Grass,
        NumFeatureType
    }
    // NOTE : ����.
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
        "������ī",
        "����",
        "�ƽþ�",
        "�ϾƸ޸�ī",
        "���Ƹ޸�ī",
        "�����ƴϾ�"
    };
    }

}
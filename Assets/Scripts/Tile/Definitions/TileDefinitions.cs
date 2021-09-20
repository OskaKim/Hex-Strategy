using UnityEngine;
using System;

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

    // NOTE : ����.
    [Serializable]
    public enum ClimateType {
        Polar,
        Subarctic,
        Temperate,
        Tropical
    }

}
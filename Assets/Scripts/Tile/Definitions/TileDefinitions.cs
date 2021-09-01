using UnityEngine;
using System;

namespace Tile
{
    // NOTE : Ÿ�� �������� x, y �ε����� ����
    [Serializable]
    public struct IndexPair
    {
        [SerializeField] private int x, y;

        public int X { get { return x; } }
        public int Y { get { return y; } }
        public IndexPair(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static bool operator ==(IndexPair indexPair1, IndexPair indexPair2) =>
            indexPair1.x == indexPair2.x && indexPair1.y == indexPair2.y;
        public static bool operator !=(IndexPair indexPair1, IndexPair indexPair2) =>
            indexPair1.x != indexPair2.x || indexPair1.y != indexPair2.y;

        public static IndexPair operator /(IndexPair indexPair1, IndexPair indexPair2)
        {
            if (indexPair2.x == 0 || indexPair2.y == 0) throw new DivideByZeroException();
            return new IndexPair(indexPair1.x / indexPair2.x, indexPair2.y / indexPair2.y);
        }
        
        public static IndexPair operator /(IndexPair indexPair, int num)
        {
            if (num == 0) throw new DivideByZeroException();
            return new IndexPair(indexPair.x / num, indexPair.y / num);
        }
    }

    // NOTE : �⺻����.
    // �⺻ ������ Ÿ���� ������������ ����ġ.
    [Serializable]
    public enum TerrainType 
    {
        Field,
        Mountain,
        Ocean,
        NumTerrainType
    }

    // NOTE : ����Ư��.
    // ���� Ư���� �ٲ� �� ����. �ߺ��� Ư���� ������ ����.
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
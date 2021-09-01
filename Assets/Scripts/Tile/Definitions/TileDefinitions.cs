using UnityEngine;
using System;

namespace Tile
{
    // NOTE : 타일 생성시의 x, y 인덱스를 가짐
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
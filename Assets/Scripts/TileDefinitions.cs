using System;

namespace Tile
{
    [Serializable]
    public struct IndexPair
    {
        public int x, y;
        public IndexPair(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static bool operator ==(IndexPair indexPair1, IndexPair indexPair2) =>
            indexPair1.x == indexPair2.x && indexPair1.y == indexPair2.y;
        public static bool operator !=(IndexPair indexPair1, IndexPair indexPair2) =>
            indexPair1.x != indexPair2.x || indexPair1.y != indexPair2.y;
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
}
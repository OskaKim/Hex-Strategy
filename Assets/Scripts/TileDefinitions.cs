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
}
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

}
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Tile
{
    public static class TileHelper
    {
        public static int maxIndexX { get; private set; }
        public static int maxIndexY { get; private set; }
        public static IndexPair maxIndex
        {
            get
            {
                return new IndexPair(maxIndexX, maxIndexY);
            }
            set
            {
                maxIndexX = value.x;
                maxIndexY = value.y;
            }
        }

        public static Tile GetTile(IndexPair indexPair)
        {
            return TileCreater.tiles.FirstOrDefault(x => x.IndexPair == indexPair);
        }

        public static Tile[] GetNearTiles(Tile tile)
        {
            var x = tile.IndexPair.x;
            var y = tile.IndexPair.y;
            bool isEven = y % 2 == 0;

            // NOTE : 육각타일의 특성상 홀수 짝수에서 인덱스가 달라지기 때문에 특수한 방식으로 타일을 습득
            if (isEven)
            {
                return new Tile[]
                {
                    GetTile(new IndexPair(x - 1, y - 1)),
                    GetTile(new IndexPair(x - 1, y)),
                    GetTile(new IndexPair(x - 1, y + 1)),
                    GetTile(new IndexPair(x, y - 1)),
                    GetTile(new IndexPair(x, y + 1)),
                    GetTile(new IndexPair(x + 1, y))
                }
                .Where(x => x != null).ToArray();
            }

            return new Tile[]
            {
                GetTile(new IndexPair(x - 1, y)),
                GetTile(new IndexPair(x, y - 1)),
                GetTile(new IndexPair(x, y + 1)),
                GetTile(new IndexPair(x + 1, y - 1)),
                GetTile(new IndexPair(x + 1, y)),
                GetTile(new IndexPair(x + 1, y + 1))
            }
            .Where(x => x != null).ToArray();
        }

        // TODO : 필요 없을 수도 있는데 혹시 몰라서 주석처리
        //private static int getNearTileCount(IndexPair index)
        //{
        //    var x = index.x;
        //    var y = index.y;

        //    // NOTE : 기본적으론 인접타일은 6개지만, 가장자리인 경우엔 따로 정의해야 함.
        //    int size = 6;
        //    if (x == 0 && y == 0) size = 2;                      // left under
        //    else if (x == 0 && y == maxIndexY) size = 3;         // left top
        //    else if (x == 0 && y != 0) size = 3;                 // left
        //    else if (x != 0 && y == 0) size = 4;                 // under
        //    else if (x == maxIndexX && y == maxIndexY) size = 2; // right top
        //    else if (x != maxIndexX && y == maxIndexY) size = 4; // top
        //    else if (x == maxIndexX) size = 3;                   // right, right under

        //    return size;
        //}

    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace Tile
{
    public class TileModel : MonoBehaviour
    {
        public static readonly List<Tile> tiles = new List<Tile>();
        public static readonly List<Text> tileLabels = new List<Text>();
        public static HexMesh hexMesh;
        public static void ClearAll()
        {
            var tiles = TileModel.tiles;
            if (tiles.Count == 0) return;

            tiles.Where(x => x != null)
                .ToList()
#if UNITY_EDITOR
                .ForEach(x => DestroyImmediate(x.gameObject));
#else
                .ForEach(x => Destroy(x.gameObject));
#endif
            tiles.Clear();

            var tileLabels = TileModel.tileLabels;
            if (tileLabels.Count == 0) return;

            tileLabels.Where(x => x != null)
                .ToList()
#if UNITY_EDITOR
                .ForEach(x => DestroyImmediate(x.gameObject));
#else
                .ForEach(x => Destroy(x.gameObject));
#endif
            tileLabels.Clear();
            if (hexMesh != null) hexMesh.Clear();
        }
    }

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

        public static Tile GetTile(HexCoordinates coordinates)
        {
            return TileModel.tiles.FirstOrDefault(x => x.coordinates == coordinates);
        }

        public static Tile GetTile(IndexPair indexPair)
        {
            return TileModel.tiles.FirstOrDefault(x => x.IndexPair == indexPair);
        }

        public static Tile[] GetNearTiles(Tile tile)
        {
            var x = tile.coordinates.X;
            var z = tile.coordinates.Z;

            return new Tile[]
            {
               GetTile(new HexCoordinates(x,    z + 1)),
               GetTile(new HexCoordinates(x,    z - 1)),
               GetTile(new HexCoordinates(x - 1,z    )),
               GetTile(new HexCoordinates(x - 1,z + 1)),
               GetTile(new HexCoordinates(x + 1,z - 1)),
               GetTile(new HexCoordinates(x + 1,z    )),
            }.Where(x => x != null).ToArray();
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
        
        public static void ClearAllTiles()
        {
            TileModel.ClearAll();
        }

#if UNITY_EDITOR
        public static void ClearAllDebugText()
        {
            var tiles = TileModel.tiles;

            tiles.ForEach(x => x.CustomDebugText(""));
        }
#endif
    }
}
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

        public static Tile GetTile(IndexPair indexPair)
        {
            return TileModel.tiles.FirstOrDefault(x => x.IndexPair == indexPair);
        }

        public static Tile[] GetNearTiles(Tile tile)
        {
            var x = tile.IndexPair.x;
            var y = tile.IndexPair.y;
            bool isEven = y % 2 == 0;

            // NOTE : ����Ÿ���� Ư���� Ȧ�� ¦������ �ε����� �޶����� ������ Ư���� ������� Ÿ���� ����
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

        // TODO : �ʿ� ���� ���� �ִµ� Ȥ�� ���� �ּ�ó��
        //private static int getNearTileCount(IndexPair index)
        //{
        //    var x = index.x;
        //    var y = index.y;

        //    // NOTE : �⺻������ ����Ÿ���� 6������, �����ڸ��� ��쿣 ���� �����ؾ� ��.
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
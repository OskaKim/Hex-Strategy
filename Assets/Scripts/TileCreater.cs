using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        private static Tile GetTile(IndexPair indexPair)
        {
            return TileCreater.tiles.FirstOrDefault(x => x.IndexPair == indexPair);
        }

        private static List<Tile> GetNearTiles(Tile tile)
        {
            var x = tile.IndexPair.x;
            var y = tile.IndexPair.y;

            List<Tile> nearTiles = new List<Tile>(getNearTileCount(tile.IndexPair));
            bool isEven = y % 2 == 0;

            // NOTE : ����Ÿ���� Ư���� Ȧ�� ¦������ �ε����� �޶����� ������ Ư���� ������� Ÿ���� ����
            if (isEven)
            {
                nearTiles.Add(GetTile(new IndexPair(x - 1, y)));
                nearTiles.Add(GetTile(new IndexPair(x, y - 1)));
                nearTiles.Add(GetTile(new IndexPair(x, y + 1)));
                nearTiles.Add(GetTile(new IndexPair(x + 1, y - 1)));
                nearTiles.Add(GetTile(new IndexPair(x + 1, y)));
                nearTiles.Add(GetTile(new IndexPair(x + 1, y + 1)));
            }
            else
            {
                nearTiles.Add(GetTile(new IndexPair(x - 1, y - 1)));
                nearTiles.Add(GetTile(new IndexPair(x - 1, y)));
                nearTiles.Add(GetTile(new IndexPair(x - 1, y + 1)));
                nearTiles.Add(GetTile(new IndexPair(x, y - 1)));
                nearTiles.Add(GetTile(new IndexPair(x, y + 1)));
                nearTiles.Add(GetTile(new IndexPair(x + 1, y)));
            }

            return nearTiles;
        }

        private static int getNearTileCount(IndexPair index)
        {
            var x = index.x;
            var y = index.y;

            // NOTE : �⺻������ ����Ÿ���� 6������, �����ڸ��� ��쿣 ���� �����ؾ� ��.
            int size = 6;
            if (x == 0 && y == 0) size = 2;                      // left under
            else if (x == 0 && y == maxIndexY) size = 3;         // left top
            else if (x == 0 && y != 0) size = 3;                 // left
            else if (x != 0 && y == 0) size = 4;                 // under
            else if (x == maxIndexX && y == maxIndexY) size = 2; // right top
            else if (x != maxIndexX && y == maxIndexY) size = 4; // top
            else if (x == maxIndexX) size = 3;                   // right, right under

            return size;
        }

    }
    public class TileCreater : MonoBehaviour
    {
        [SerializeField] private IndexPair tileRange = new IndexPair(1, 1);
        [SerializeField] private Tile tilePrefab;

        // TODO : Ÿ�� ����Ʈ�� ��� �δ°� ������
        public static readonly List<Tile> tiles = new List<Tile>();

        private void Create(IndexPair indexRange)
        {
            TileHelper.maxIndex = new IndexPair(indexRange.x, indexRange.y);

            for (int y = 0; y < indexRange.y; ++y)
            {
                for (int x = 0; x < indexRange.x; ++x)
                {
                    var tile = Instantiate(tilePrefab);
                    
                    tile.Setup(new IndexPair(x, y), new Vector2(x * Tile.SIZE_X, y * Tile.SIZE_Y));
                    tiles.Add(tile);
                }
            }
        }

        private void OnEnable()
        {
            Create(tileRange);
        }
        private void OnDisable()
        {
            ClearAll();
        }

        private static void ClearAll()
        {
            if (tiles.Count == 0) return;

            tiles.Where(x => x != null)
                .ToList()
                .ForEach(x => Destroy(x.gameObject));

            tiles.Clear();
        }
    }
}
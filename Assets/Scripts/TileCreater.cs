using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Tile
{
    public class TileCreater : MonoBehaviour
    {
        [SerializeField] private IndexPair tileRange = new IndexPair(1, 1);
        [SerializeField] private Tile tilePrefab;

        // TODO : 타일 리스트를 어디에 두는게 좋을지
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
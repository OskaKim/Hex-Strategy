using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tile
{
    public class TileCreater : MonoBehaviour
    {
        [SerializeField]
        private Tile tilePrefab;
        private readonly List<Tile> tiles = new List<Tile>();

        private void Create(IndexPair indexRange)
        {
            tiles.Capacity = indexRange.x * indexRange.y;

            for (int y = 0; y < indexRange.y; ++y)
            {
                for (int x = 0; x < indexRange.x; ++x)
                {
                    var t = Instantiate<Tile>(tilePrefab);
                    t.Setup(new IndexPair(x, y), new Vector2(x, y));
                    tiles.Add(t);
                }
            }
        }

        private void Start()
        {
            Create(new IndexPair(10, 5));
        }
    }
}
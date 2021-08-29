using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using System;

namespace Tile
{
    public class TileCreater : MonoBehaviour
    {
        [SerializeField] private IndexPair tileRange = new IndexPair(75, 50);
        [SerializeField] private Tile tilePrefab;

        // NOTE : ���Ÿ�� ������ �ʿ��� �Ķ����.
        [SerializeField] private int   numOfMaxContinentTiles = 1000;        // NOTE : ���Ÿ�� �ִ� ������. TODO : �� ������κ��� ����
        [SerializeField] private int   numOfLeastContinentTiles = 100;       // NOTE : ������ ���Ÿ���� �ּ� ����. �� ���ں��� ������ �߰� ������. ��� Ÿ���� �ִ� ������� Ŀ�� �� ����
        [SerializeField] private float influenceOfContinent = 0.6f;          // NOTE : ���Ÿ�� ������ ��� ����� ����(0 ~ 1). ù Ÿ���� ������� Ÿ�� ������ x ����� ������ ����.

        private Transform tilesRoot = null;
        private struct ContinentTile
        {
            public Tile tile;
            public int influence;
            public bool isClosed;
            public ContinentTile(Tile tile, int influence)
            {
                this.tile = tile;
                this.influence = influence;
                isClosed = false;
            }
        }

        public void Create(IndexPair indexRange)
        {
            TileHelper.ClearAllTiles();
            TileHelper.maxIndex = new IndexPair(indexRange.x, indexRange.y);

            if (tilesRoot == null) tilesRoot = new GameObject("tilesRoot").transform;

            for (int y = 0; y < indexRange.y; ++y)
            {
                for (int x = 0; x < indexRange.x; ++x)
                {
                    var tile = Instantiate(tilePrefab, tilesRoot);
                    
                    tile.Setup(new IndexPair(x, y), new Vector2(x * Tile.SIZE_X, y * Tile.SIZE_Y));
                    tile.name = $"{x},{y}";
                    TileModel.tiles.Add(tile);
                }
            }

            // NOTE : Ÿ�� ���� ������
            var defaultPoint = TileHelper.maxIndex / 2;
            // NOTE : Ÿ�� ���� �������� �Ź� �ٸ��� �ϱ� ���� �ణ ����
            var firstContinentTileIndex = defaultPoint;

            var continentTiles = CreateContinentTilePhase(firstContinentTileIndex);
            
            foreach(var tile in TileModel.tiles)
            {
                if(continentTiles.Contains(tile)) {
                    tile.setupType(TerrainType.Field, 0);
                    continue;
                }

                tile.setupType(TerrainType.Ocean, 0);
            }
        }

        // NOTE : ��� Ÿ�� ���� ������
        private List<Tile> CreateContinentTilePhase(IndexPair firstContinentTileIndex)
        {
            var continentTiles = new List<ContinentTile>();

            // NOTE : �ۼ�Ʈ�� ����ϱ� ���� �⺻ ����. ��ü Ÿ���� 1%�� �ش�
            int percentBasicUnit = numOfMaxContinentTiles / 100;

            // NOTE : ��� �����. ������� �������� ����Ÿ���� ����� Ȯ���� ����
            int influence = (int)(numOfMaxContinentTiles * Mathf.Clamp(influenceOfContinent, 0, 1));

            // ù ���Ÿ���� �������� ����� ����.
            var firstContinentTile = new ContinentTile(TileHelper.GetTile(firstContinentTileIndex), --influence / percentBasicUnit);
            continentTiles.Add(firstContinentTile);

            do {
                CreateContinentTilesFromNearTiles(continentTiles, influence, percentBasicUnit);
            } while (Mathf.Clamp(numOfLeastContinentTiles, 0, numOfMaxContinentTiles) > continentTiles.Count);

#if UNITY_EDITOR
            foreach (var tile in TileModel.tiles)
            {
                tile.CustomDebugText("");
            }

            foreach (var continentTile in continentTiles)
            {
                continentTile.tile.CustomDebugText($"{continentTile.influence}");
            }
#endif
            Debug.Log($"{continentTiles.Count}continentTiles have created");
            return continentTiles.Select(x => x.tile).ToList();
        }

        private void CreateContinentTilesFromNearTiles(List<ContinentTile> continentTiles, int influence, int percentBasicUnit)
        {
            bool isEnd = false;
            for (int i = 0; i < continentTiles.Count && !isEnd; ++i)
            {
                var currentTile = continentTiles[i];
                if (currentTile.isClosed) continue;
                currentTile.isClosed = true;

                foreach (var nearTile in TileHelper.GetNearTiles(currentTile.tile))
                {
                    if (!continentTiles.Select(x => x.tile).Contains(nearTile))
                    {
                        // NOTE : ��ü Ÿ�� ������ ��� �� Ÿ���� ��������� ���� ����� �ۼ�Ʈ
                        int influencePercent = influence / percentBasicUnit;

                        // NOTE : ������ Ÿ�� ���� Ÿ�ֿ̹� ���Ÿ�� ���ڰ� �ּҿ� ��ġ�� ���ϸ� ������ ���Ÿ�� ����
                        bool lastInfluenceTile = influence == 1;
                        bool isForceContinentTile = lastInfluenceTile && numOfLeastContinentTiles > continentTiles.Count;

                        if (!isForceContinentTile)
                        {
                            // NOTE : ���� Ȯ��. ���� Ȯ������ ���Ÿ�� Ȯ���� ������ �������� �ʴ°ɷ� ��
                            int randomResult = UnityEngine.Random.Range(0, 100);
                            if (randomResult > influencePercent) continue;
                        }

                        var newContinentTile = nearTile;
                        newContinentTile.ContinentInfluence = influence;

                        continentTiles.Add(new ContinentTile(newContinentTile, influencePercent));

                        if (!isForceContinentTile) --influence;
                        if (influence <= 0 || numOfMaxContinentTiles <= continentTiles.Count)
                        {
                            isEnd = true;
                            break;
                        }
                    }
                }
            }
        }

#if UNITY_EDITOR
        public void CreateFromEditor()
        {
            Create(tileRange);
        }
#endif

        private void OnDisable()
        {
            TileHelper.ClearAllTiles();
        }

    }
}
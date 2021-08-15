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
        [SerializeField] private IndexPair tileRange = new IndexPair(1, 1);
        [SerializeField] private Tile tilePrefab;

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

        private void Start()
        {
            var clickStream = Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.R))
            .Subscribe(_ =>
                Create(tileRange));
        }

        private void Create(IndexPair indexRange)
        {
            TileHelper.ClearAllTiles();
            TileHelper.maxIndex = new IndexPair(indexRange.x, indexRange.y);

            for (int y = 0; y < indexRange.y; ++y)
            {
                for (int x = 0; x < indexRange.x; ++x)
                {
                    var tile = Instantiate(tilePrefab);
                    
                    tile.Setup(new IndexPair(x, y), new Vector2(x * Tile.SIZE_X, y * Tile.SIZE_Y));
                    tile.name = $"{x},{y}";
                    TileModel.tiles.Add(tile);
                }
            }

            var continentTiles = CreateContinentTilePhase();
            
            foreach(var tile in TileModel.tiles)
            {
                if(continentTiles.Contains(tile)) {
                    tile.attachResource((int)TerrainType.Field, 0);
                    continue;
                }

                tile.attachResource((int)TerrainType.Ocean, 0);
            }
        }

        // NOTE : ��� Ÿ�� ���� ������
        private List<Tile> CreateContinentTilePhase()
        {
            var continentTiles = new List<ContinentTile>();

            // TODO : �� ������κ��� ����
            int numOfContinentTiles = 500;
            // NOTE : ��� �����. ������� �������� ����Ÿ���� ����� Ȯ���� ����
            int influence = numOfContinentTiles;

            // ù ���Ÿ���� �������� ����� ����. ù ���Ÿ���� ���� �߾�
            var firstContinentTile = new ContinentTile(TileHelper.GetTile(TileHelper.maxIndex / 2), --influence);
            continentTiles.Add(firstContinentTile);

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
                        const int MAX_PERCENT = 100;
                        const int MIN_PERCENT = 10;

                        // NOTE : ���Ÿ������ �ٲ����� ���� Ȯ��
                        int percent = Mathf.Clamp(influence / (numOfContinentTiles / MAX_PERCENT), MIN_PERCENT, MAX_PERCENT);
                        if (UnityEngine.Random.Range(0, MAX_PERCENT) > percent) continue;

                        continentTiles.Add(new ContinentTile(nearTile, --influence));
                        if (influence <= 0)
                        {
                            isEnd = true;
                            break;
                        }
                    }
                }
            }

            // NOTE : �������� ���� ��� Ÿ���� ������ �߰� ����
            continentTiles
                .SelectMany(x => TileHelper.GetNearTiles(x.tile))
                .Distinct()
                .Where(x => !continentTiles.Select(continentTile => continentTile.tile).Contains(x))
                .OrderBy(g => Guid.NewGuid())
                .Take(influence)
                .All(x => {
                    continentTiles.Add(new ContinentTile(x, --influence));
                    return true;
                });

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

        private void OnEnable()
        {
            Create(tileRange);
        }
        private void OnDisable()
        {
            TileHelper.ClearAllTiles();
        }

    }
}
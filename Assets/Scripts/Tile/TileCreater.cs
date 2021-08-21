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
        [SerializeField] private int numOfContinentTiles = 1000;             // NOTE : ���Ÿ�� ������ TODO : �� ������κ��� ����
        [SerializeField] private float influenceOfContinent = 0.6f;          // NOTE : ���Ÿ�� ������ ��� ����� ����(0 ~ 1)
        [SerializeField] private int maxPercentToChangeToContinent = 70;     // NOTE : ���Ÿ������ �ٲ����� ���� �ִ� Ȯ��
        [SerializeField] private int minPercentToChangeToContinent = 10;     // NOTE : ���Ÿ������ �ٲ����� ���� �ּ� Ȯ��
        [SerializeField] private bool isCreateContinentTilesPerfect = false; // NOTE : ù �����ÿ� ���Ÿ���� ���� �������� �ʾҴٸ� ���� ����ŭ ��� Ÿ���� �߰��� ���ΰ�
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

            // NOTE : ��� �����. ������� �������� ����Ÿ���� ����� Ȯ���� ����
            int influence = (int)(numOfContinentTiles * influenceOfContinent);

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
                        // NOTE : ���Ÿ������ �ٲ����� ���� Ȯ��
                        int percent = Mathf.Clamp(influence / (numOfContinentTiles / maxPercentToChangeToContinent), minPercentToChangeToContinent, maxPercentToChangeToContinent);
                        if (UnityEngine.Random.Range(0, maxPercentToChangeToContinent) > percent) continue;

                        var newContinentTile = nearTile;
                        newContinentTile.ContinentInfluence = influence;
                        continentTiles.Add(new ContinentTile(newContinentTile, --influence));
                        if (influence <= 0)
                        {
                            isEnd = true;
                            break;
                        }
                    }
                }
            }

            if (isCreateContinentTilesPerfect)
            {
                // NOTE : �������� ���� ��� Ÿ���� ������ �߰� ����
                continentTiles
                    .SelectMany(x => TileHelper.GetNearTiles(x.tile))
                    .Distinct()
                    .Where(x => !continentTiles.Select(continentTile => continentTile.tile).Contains(x))
                    .OrderBy(g => Guid.NewGuid())
                    .Take(influence)
                    .All(x =>
                    {
                        x.ContinentInfluence = influence;
                        continentTiles.Add(new ContinentTile(x, --influence));
                        return true;
                    });
            }

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
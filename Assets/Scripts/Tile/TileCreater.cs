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
        [SerializeField] private int numOfContinentTiles = 1000;             // NOTE : 대륙타일 사이즈 TODO : 맵 사이즈로부터 결정
        [SerializeField] private float influenceOfContinent = 0.6f;          // NOTE : 대륙타일 사이즈 대비 영향력 지수(0 ~ 1)
        [SerializeField] private int maxPercentToChangeToContinent = 70;     // NOTE : 대륙타일으로 바꿀지에 대한 최대 확률
        [SerializeField] private int minPercentToChangeToContinent = 10;     // NOTE : 대륙타일으로 바꿀지에 대한 최소 확률
        [SerializeField] private bool isCreateContinentTilesPerfect = false; // NOTE : 첫 생성시에 대륙타일이 전부 생성되지 않았다면 남은 수만큼 대륙 타일을 추가할 것인가
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

        // NOTE : 대륙 타일 설정 페이즈
        private List<Tile> CreateContinentTilePhase()
        {
            var continentTiles = new List<ContinentTile>();

            // NOTE : 대륙 영향력. 영향력이 높을수록 인접타일이 대륙일 확률이 높음
            int influence = (int)(numOfContinentTiles * influenceOfContinent);

            // 첫 대륙타일을 기준으로 대륙을 생성. 첫 대륙타일은 맵의 중앙
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
                        // NOTE : 대륙타일으로 바꿀지에 대한 확률
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
                // NOTE : 생성되지 않은 대륙 타일이 있을때 추가 생성
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
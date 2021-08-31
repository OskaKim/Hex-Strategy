using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

namespace Tile
{
    public class TileCreater : MonoBehaviour
    {
        [SerializeField] private IndexPair  tileRange = new IndexPair(75, 50);
        [SerializeField] private Tile       tilePrefab;
        [SerializeField] private HexGrid    hexGrid;
        [SerializeField] private Canvas     gridCanvas;
        [SerializeField] private Text       cellLabelPrefab;
        [SerializeField] private HexMesh    hexMesh;
        [SerializeField] private Color      defaultColor = Color.white;
        [SerializeField] private Color      touchedColor = Color.magenta;

        // NOTE : 대륙타일 생성시 필요한 파라미터.
        [SerializeField] private int   numOfMaxContinentTiles = 1000;        // NOTE : 대륙타일 최대 사이즈. TODO : 맵 사이즈로부터 결정
        [SerializeField] private int   numOfLeastContinentTiles = 100;       // NOTE : 생성할 대륙타일의 최소 숫자. 이 숫자보다 적으면 추가 생성함. 대륙 타일의 최대 사이즈보다 커질 수 없음
        [SerializeField] private float influenceOfContinent = 0.6f;          // NOTE : 대륙타일 사이즈 대비 영향력 지수(0 ~ 1). 첫 타일의 영향력은 타일 사이즈 x 영향력 지수로 계산됨.

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

            for (int y = 0, i = 0; y < indexRange.y; y++)
            {
                for (int x = 0; x < indexRange.x; x++)
                {
                    var tile = Instantiate(tilePrefab, hexGrid.transform);
                    // NOTE : 실제 위치 설정
                    var pos = tile.transform.localPosition = new Vector3(
                        (x + y * 0.5f - y / 2) * (HexMetrics.innerRadius * 2f),
                        0f,
                        y * (HexMetrics.outerRadius * 1.5f));

                    tile.Setup(new IndexPair(x, y));
                    TileModel.tiles.Add(tile);
                    
                    // NOTE : 좌표계 설정
                    tile.coordinates = HexCoordinates.FromOffsetCoordinates(x, y);
                    tile.color = defaultColor;

                    Text label = Instantiate<Text>(cellLabelPrefab);
                    label.rectTransform.SetParent(gridCanvas.transform, false);
                    label.rectTransform.anchoredPosition = new Vector2(pos.x, pos.z);
                    label.text = tile.coordinates.ToStringOnSeparateLines();
                    TileModel.tileLabels.Add(label);
                }
            }

            Debug.Log(TileModel.tiles.Count);

            // TODO : 타일 환경설정은 새로운 좌표계에 맞춰서 리팩토링

            //// NOTE : 타일 생성 기준점
            var defaultPoint = TileHelper.maxIndex / 2;
            //// NOTE : 타일 생성 기준점을 매번 다르게 하기 위해 약간 조절
            var firstContinentTileIndex = defaultPoint;

            var continentTiles = CreateContinentTilePhase(firstContinentTileIndex);

            continentTiles.All(x => { x.color = Color.green; return x; });

            // NOTE : 메쉬 생성
            hexMesh.Triangulate();

            //foreach(var tile in TileModel.tiles)
            //{
            //    if(continentTiles.Contains(tile)) {
            //        tile.setupType(TerrainType.Field, 0);
            //        continue;
            //    }

            //    tile.setupType(TerrainType.Ocean, 0);
            //}
        }

        // NOTE : 대륙 타일 설정 페이즈
        private List<Tile> CreateContinentTilePhase(IndexPair firstContinentTileIndex)
        {
            var continentTiles = new List<ContinentTile>();

            // NOTE : 퍼센트를 계산하기 위한 기본 단위. 전체 타일의 1%에 해당
            int percentBasicUnit = numOfMaxContinentTiles / 100;

            // NOTE : 대륙 영향력. 영향력이 높을수록 인접타일이 대륙일 확률이 높음
            int influence = (int)(numOfMaxContinentTiles * Mathf.Clamp(influenceOfContinent, 0, 1));

            // 첫 대륙타일을 기준으로 대륙을 생성.
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
                        // NOTE : 전체 타일 사이즈 대비 각 타일의 영향력으로 계산된 영향력 퍼센트
                        int influencePercent = influence / percentBasicUnit;

                        // NOTE : 마지막 타일 생성 타이밍에 대륙타일 숫자가 최소에 미치지 못하면 강제로 대륙타일 생성
                        bool lastInfluenceTile = influence == 1;
                        bool isForceContinentTile = lastInfluenceTile && numOfLeastContinentTiles > continentTiles.Count;

                        if (!isForceContinentTile)
                        {
                            // NOTE : 랜덤 확률. 랜덤 확률보다 대륙타일 확률이 낮으면 생성하지 않는걸로 함
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
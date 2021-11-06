using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

namespace Tile
{
    // TODO : 최적화
    public class TileCreater : MonoBehaviour {
        #region singletone
        private static TileCreater instance;
        public static TileCreater GetInstance() {
            if (!instance) {
                instance = GameObject.FindObjectOfType<TileCreater>();
            }

            return instance;
        }
        #endregion
        [SerializeField] private Tile tilePrefab;
        [SerializeField] private IndexPair tileRange = new IndexPair(75, 50);
        [SerializeField] private Transform tilesRoot;
        [SerializeField] private HexMesh hexMesh;
        [SerializeField] private Canvas gridCanvas;
        [SerializeField] private Text cellLabelPrefab;
        [SerializeField] private Color defaultColor = Color.white;
        [SerializeField] private IndexPair[] firstContinentTileIndexs;

        // NOTE : 대륙타일 생성시 필요한 파라미터.
        [SerializeField] private int numOfMaxContinentTiles = 1000;        // NOTE : 대륙타일 최대 사이즈. TODO : 맵 사이즈로부터 결정
        [SerializeField] private int numOfLeastContinentTiles = 100;       // NOTE : 생성할 대륙타일의 최소 숫자. 이 숫자보다 적으면 추가 생성함. 대륙 타일의 최대 사이즈보다 커질 수 없음
        [SerializeField, Range(0, 1)] private float influenceOfContinent = 0.6f;          // NOTE : 대륙타일 사이즈 대비 영향력 지수(0 ~ 1). 첫 타일의 영향력은 타일 사이즈 x 영향력 지수로 계산됨.
        [SerializeField, Range(0, 1)] private float moutainRatioOfContinentTiles = 0.1f;  // NOTE : 대륙 중에서 산 타일의 비율(0 ~ 1).

        // NOTE : 작성된 대륙 타일리스트가 타입별로 배열에 할당됨. 생성되지 않은 대륙은 비어있음
        private List<Tile>[] allContinentTiles = new List<Tile>[TilePropertyInfo.ContinentNames.Length];

        private struct ContinentTile {
            public Tile tile;
            public int influence;
            public bool isClosed;
            public ContinentTile(Tile tile, int influence) {
                this.tile = tile;
                this.influence = influence;
                isClosed = false;
            }
        }

        public void Create(IndexPair indexRange) {
            TileHelper.ClearAllTiles();
            TileHelper.maxIndex = new IndexPair(indexRange.X, indexRange.Y);

            for (int y = 0; y < indexRange.Y; y++) {
                for (int x = 0; x < indexRange.X; x++) {
                    var tile = Instantiate(tilePrefab, tilesRoot);
                    // NOTE : 실제 위치 설정
                    var pos = tile.transform.localPosition = new Vector3(
                        (x + y * 0.5f - y / 2) * (HexMetrics.innerRadius * 2f),
                        0f,
                        y * (HexMetrics.outerRadius * 1.5f));

                    // NOTE : 좌표계 설정
                    tile.Setup(new IndexPair(x, y));

                    TileModel.tiles.Add(tile);

                    // NOTE : 라벨 설정
                    Text label = Instantiate<Text>(cellLabelPrefab);
                    label.rectTransform.SetParent(gridCanvas.transform, false);
                    label.rectTransform.anchoredPosition = new Vector2(pos.x, pos.z);
                    label.text = tile.Coordinates.ToStringOnSeparateLines();
                    label.tag = "TileUI";
                    TileModel.tileLabels.Add(label);
                }
            }

            Debug.Log(TileModel.tiles.Count);

            // TODO : 타일 환경설정은 새로운 좌표계에 맞춰서 리팩토링

            for (int i = 0; i < allContinentTiles.Length; ++i) {
                allContinentTiles[i] = new List<Tile>();
            }

            foreach (var firstContinentTileIndex in firstContinentTileIndexs) {
                CreateRandomContinent(firstContinentTileIndex);
            }

            SetupTerrainType();
            SetupClimateType();
            SetupFeatureType();

            TileData.SetTileTerrainData();

            for (int i = 0; i < allContinentTiles.Length; ++i) {
                var currentContinentName = TilePropertyInfo.ContinentNames[i];
                var currentContinent = allContinentTiles[i];
                Debug.Log($"{currentContinentName} : {currentContinent.Count}");
            }
        }

        private void SetupTerrainType() {

            var randomSortedContinentTiles = allContinentTiles.SelectMany(x => x).OrderBy(g => Guid.NewGuid());
            var numOfContinentTiles = randomSortedContinentTiles.Count();
            var numOfMountainTiles = (int)(numOfContinentTiles * moutainRatioOfContinentTiles);
            var mountainTiles = randomSortedContinentTiles
                                .Take(numOfMountainTiles)
                                .ToList();

            foreach (var tile in TileModel.tiles) {
                if (mountainTiles.Any(x => x == tile)) {
                    tile.SetupTerrainType(TerrainType.Mountain);
                    continue;
                }

                if (randomSortedContinentTiles.Any(x => x == tile)) {
                    tile.SetupTerrainType(TerrainType.Field);
                    continue;
                }

                tile.SetupTerrainType(TerrainType.Ocean);
            }
        }

        // NOTE : 타일 인덱스로부터 기후를 계산
        private ClimateType GetClimateTypeFromIndexPair(IndexPair indexPair) {
            var yPercentRatio = (indexPair.Y / (float)tileRange.Y) * 100.0f;

            if (yPercentRatio <= 10 || 90 <= yPercentRatio) {
                return ClimateType.Polar;
            }
            else if (yPercentRatio <= 20 || 80 <= yPercentRatio) {
                return ClimateType.Subarctic;
            }
            else if (yPercentRatio <= 40 || 60 <= yPercentRatio) {
                return ClimateType.Temperate;
            }
            else {
                return ClimateType.Tropical;
            }
        }

        // NOTE : 기후 설정
        private void SetupClimateType() {
            TileModel.tiles.ForEach(x => {
                x.ClimateType = (int)GetClimateTypeFromIndexPair(x.IndexPair);
            });
        }

        private void SetupFeatureType() {
            var randomSortedTiles = TileModel.tiles.OrderBy(g => Guid.NewGuid());

            // TODO : 외부에서 설정 가능하도록
            int jungleAreaCount = 3;
            int desertAreaCount = 3;

            foreach (var tile in randomSortedTiles) {
                if (tile.FeatureType != FeatureType.None) continue;
                if (StartCreateFeatureTypeArea(tile, FeatureType.Ice)) continue;
                if (StartCreateFeatureTypeArea(tile, FeatureType.Jungle, ref jungleAreaCount, 30)) continue;
                if (StartCreateFeatureTypeArea(tile, FeatureType.Desert, ref desertAreaCount, 50)) continue;

                tile.SetupFeatureType(FeatureType.Grass);
            }
        }

        private bool CheckFeatureTypeAreaCondition(Tile tile, FeatureType featureType, ref List<ClimateType> climateConditions, ref List<TerrainType> terrainConditions) {
            // NOTE : 기후 조건 체크
            if (FeatureInfo.climateConditions.TryGetValue(featureType, out climateConditions)) {
                if (!climateConditions.Any(x => (int)x == tile.ClimateType)) return false;
            }

            // NOTE : 지형 조건 체크
            if (FeatureInfo.terrainConditions.TryGetValue(featureType, out terrainConditions)) {
                if (!terrainConditions.Any(x => x == tile.TerrainType)) return false;
            }

            return true;
        }

        private bool StartCreateFeatureTypeArea(Tile tile, FeatureType featureType) {
            List<ClimateType> climateConditions = null;
            List<TerrainType> terrainConditions = null;
            if (!CheckFeatureTypeAreaCondition(tile, featureType, ref climateConditions, ref terrainConditions)) return false;

            tile.SetupFeatureType(featureType);
            return true;
        }

        // NOTE : 타일 속성을 일정 크기의 지역으로써 생성
        private bool StartCreateFeatureTypeArea(Tile tile, FeatureType featureType, ref int remainAreaCount, int areaSize) {
            if (remainAreaCount <= 0) return false;
            List<ClimateType> climateConditions = null;
            List<TerrainType> terrainConditions = null;
            if (!CheckFeatureTypeAreaCondition(tile, featureType, ref climateConditions, ref terrainConditions)) return false;

            --remainAreaCount;

            tile.SetupFeatureType(featureType);
            --areaSize;

            CreateFeatureTypeArea(tile, featureType, climateConditions, terrainConditions, ref areaSize);
            return true;
        }

        // NOTE : 지정된 카운트 만큼의 피쳐 타입의 영역을 생성
        private void CreateFeatureTypeArea(Tile currentTile, FeatureType featureType, List<ClimateType> climateConditions, List<TerrainType> terrainConditions, ref int remainCreateCount) {
            if (remainCreateCount <= 0) return;

            var nearTiles = TileHelper.GetNearTilesRandomSorted(currentTile).Where(x => {
                bool isUnSet = x.FeatureType == FeatureType.None;
                bool isCorrectClimate = climateConditions.Any(condition => (int)condition == x.ClimateType);
                bool isCorrectTerrain = terrainConditions.Any(condition => condition == x.TerrainType);
                return isUnSet && isCorrectClimate && isCorrectTerrain;
            }).ToArray();

            foreach (var nearTile in nearTiles) {
                if (remainCreateCount <= 0) return;

                nearTile.SetupFeatureType(featureType);
                --remainCreateCount;
            }

            foreach (var nearTile in nearTiles) {
                CreateFeatureTypeArea(nearTile, featureType, climateConditions, terrainConditions, ref remainCreateCount);
            }
        }

        private void CreateRandomContinent(IndexPair firstContinentTileIndex) {
            var isAnyEmptyContinentTIleList = allContinentTiles.Any(x => x.Count == 0);

            if (!isAnyEmptyContinentTIleList) {
                Debug.LogError("there is no empty continent tile");
                return;
            }

            // TODO : 무한 루프를 가능한 없애고 싶으므로 더 좋은 방법이 생각나면 변경
            // NOTE : 아직 설정이 안된 대륙 타일 중 하나를 설정
            while (true) {
                int continentType = UnityEngine.Random.Range(0, allContinentTiles.Length);
                if(allContinentTiles[continentType].Count == 0) {
                    CreateContinentTilePhase(firstContinentTileIndex, ref allContinentTiles[continentType]);
                    allContinentTiles[continentType].ForEach(x => x.ContinentType = continentType);
                    return;
                }
            }
        }

        // NOTE : 대륙 타일 설정 페이즈
        private void CreateContinentTilePhase(IndexPair firstContinentTileIndex, ref List<Tile> continentTileList)
        {
            var continentTilesForCreate = new List<ContinentTile>();

            // NOTE : 퍼센트를 계산하기 위한 기본 단위. 전체 타일의 1%에 해당
            int percentBasicUnit = numOfMaxContinentTiles / 100;

            // NOTE : 대륙 영향력. 영향력이 높을수록 인접타일이 대륙일 확률이 높음
            int influence = (int)(numOfMaxContinentTiles * Mathf.Clamp(influenceOfContinent, 0, 1));

            // NOTE : 첫 대륙타일을 기준으로 대륙을 생성.
            var firstContinentTile = new ContinentTile(TileHelper.GetTile(firstContinentTileIndex), --influence / percentBasicUnit);
            continentTilesForCreate.Add(firstContinentTile);

            // NOTE : 최소 생성 수를 넘을때까지 대륙 생성 알고리즘을 반복
            do {
                CreateContinentTilesFromNearTiles(continentTilesForCreate, influence, percentBasicUnit);
            } while (Mathf.Clamp(numOfLeastContinentTiles, 0, numOfMaxContinentTiles) > continentTilesForCreate.Count);

            Debug.Log($"{continentTilesForCreate.Count}continentTiles have created");
            continentTileList = continentTilesForCreate.Select(x => x.tile).ToList();
        }

        private void CreateContinentTilesFromNearTiles(List<ContinentTile> continentTiles, int influence, int percentBasicUnit)
        {
            bool isEnd = false;
            for (int i = 0; i < continentTiles.Count && !isEnd; ++i)
            {
                var currentTile = continentTiles[i];
                if (currentTile.isClosed) continue;
                currentTile.isClosed = true;

                foreach (var nearTile in TileHelper.GetNearTiles(currentTile.tile)) {
                    if (!continentTiles.Select(x => x.tile).Contains(nearTile) && !allContinentTiles.Any(x => x.Contains(nearTile))) {
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

        public void ReCreateMap()
        {
            Create(tileRange);
            TileHelper.InitHexMesh();
        }

        private void OnDisable()
        {
            TileHelper.ClearAllTiles();
        }

    }
}
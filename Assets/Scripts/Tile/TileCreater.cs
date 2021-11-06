using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

namespace Tile
{
    // TODO : ����ȭ
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

        // NOTE : ���Ÿ�� ������ �ʿ��� �Ķ����.
        [SerializeField] private int numOfMaxContinentTiles = 1000;        // NOTE : ���Ÿ�� �ִ� ������. TODO : �� ������κ��� ����
        [SerializeField] private int numOfLeastContinentTiles = 100;       // NOTE : ������ ���Ÿ���� �ּ� ����. �� ���ں��� ������ �߰� ������. ��� Ÿ���� �ִ� ������� Ŀ�� �� ����
        [SerializeField, Range(0, 1)] private float influenceOfContinent = 0.6f;          // NOTE : ���Ÿ�� ������ ��� ����� ����(0 ~ 1). ù Ÿ���� ������� Ÿ�� ������ x ����� ������ ����.
        [SerializeField, Range(0, 1)] private float moutainRatioOfContinentTiles = 0.1f;  // NOTE : ��� �߿��� �� Ÿ���� ����(0 ~ 1).

        // NOTE : �ۼ��� ��� Ÿ�ϸ���Ʈ�� Ÿ�Ժ��� �迭�� �Ҵ��. �������� ���� ����� �������
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
                    // NOTE : ���� ��ġ ����
                    var pos = tile.transform.localPosition = new Vector3(
                        (x + y * 0.5f - y / 2) * (HexMetrics.innerRadius * 2f),
                        0f,
                        y * (HexMetrics.outerRadius * 1.5f));

                    // NOTE : ��ǥ�� ����
                    tile.Setup(new IndexPair(x, y));

                    TileModel.tiles.Add(tile);

                    // NOTE : �� ����
                    Text label = Instantiate<Text>(cellLabelPrefab);
                    label.rectTransform.SetParent(gridCanvas.transform, false);
                    label.rectTransform.anchoredPosition = new Vector2(pos.x, pos.z);
                    label.text = tile.Coordinates.ToStringOnSeparateLines();
                    label.tag = "TileUI";
                    TileModel.tileLabels.Add(label);
                }
            }

            Debug.Log(TileModel.tiles.Count);

            // TODO : Ÿ�� ȯ�漳���� ���ο� ��ǥ�迡 ���缭 �����丵

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

        // NOTE : Ÿ�� �ε����κ��� ���ĸ� ���
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

        // NOTE : ���� ����
        private void SetupClimateType() {
            TileModel.tiles.ForEach(x => {
                x.ClimateType = (int)GetClimateTypeFromIndexPair(x.IndexPair);
            });
        }

        private void SetupFeatureType() {
            var randomSortedTiles = TileModel.tiles.OrderBy(g => Guid.NewGuid());

            // TODO : �ܺο��� ���� �����ϵ���
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
            // NOTE : ���� ���� üũ
            if (FeatureInfo.climateConditions.TryGetValue(featureType, out climateConditions)) {
                if (!climateConditions.Any(x => (int)x == tile.ClimateType)) return false;
            }

            // NOTE : ���� ���� üũ
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

        // NOTE : Ÿ�� �Ӽ��� ���� ũ���� �������ν� ����
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

        // NOTE : ������ ī��Ʈ ��ŭ�� ���� Ÿ���� ������ ����
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

            // TODO : ���� ������ ������ ���ְ� �����Ƿ� �� ���� ����� �������� ����
            // NOTE : ���� ������ �ȵ� ��� Ÿ�� �� �ϳ��� ����
            while (true) {
                int continentType = UnityEngine.Random.Range(0, allContinentTiles.Length);
                if(allContinentTiles[continentType].Count == 0) {
                    CreateContinentTilePhase(firstContinentTileIndex, ref allContinentTiles[continentType]);
                    allContinentTiles[continentType].ForEach(x => x.ContinentType = continentType);
                    return;
                }
            }
        }

        // NOTE : ��� Ÿ�� ���� ������
        private void CreateContinentTilePhase(IndexPair firstContinentTileIndex, ref List<Tile> continentTileList)
        {
            var continentTilesForCreate = new List<ContinentTile>();

            // NOTE : �ۼ�Ʈ�� ����ϱ� ���� �⺻ ����. ��ü Ÿ���� 1%�� �ش�
            int percentBasicUnit = numOfMaxContinentTiles / 100;

            // NOTE : ��� �����. ������� �������� ����Ÿ���� ����� Ȯ���� ����
            int influence = (int)(numOfMaxContinentTiles * Mathf.Clamp(influenceOfContinent, 0, 1));

            // NOTE : ù ���Ÿ���� �������� ����� ����.
            var firstContinentTile = new ContinentTile(TileHelper.GetTile(firstContinentTileIndex), --influence / percentBasicUnit);
            continentTilesForCreate.Add(firstContinentTile);

            // NOTE : �ּ� ���� ���� ���������� ��� ���� �˰����� �ݺ�
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
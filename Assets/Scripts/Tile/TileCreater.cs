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
        [SerializeField] private Tile       tilePrefab;
        [SerializeField] private IndexPair  tileRange = new IndexPair(75, 50);
        [SerializeField] private Transform  tilesRoot;
        [SerializeField] private HexMesh    hexMesh;
        [SerializeField] private Canvas     gridCanvas;
        [SerializeField] private Text       cellLabelPrefab;
        [SerializeField] private Color      defaultColor = Color.white;

        // NOTE : ���Ÿ�� ������ �ʿ��� �Ķ����.
        [SerializeField] private int   numOfMaxContinentTiles = 1000;        // NOTE : ���Ÿ�� �ִ� ������. TODO : �� ������κ��� ����
        [SerializeField] private int   numOfLeastContinentTiles = 100;       // NOTE : ������ ���Ÿ���� �ּ� ����. �� ���ں��� ������ �߰� ������. ��� Ÿ���� �ִ� ������� Ŀ�� �� ����
        [SerializeField] private float influenceOfContinent = 0.6f;          // NOTE : ���Ÿ�� ������ ��� ����� ����(0 ~ 1). ù Ÿ���� ������� Ÿ�� ������ x ����� ������ ����.

        // NOTE : �ۼ��� ��� Ÿ�ϸ���Ʈ�� Ÿ�Ժ��� �迭�� �Ҵ��. �������� ���� ����� �������
        private List<Tile>[] allContinentTiles = new List<Tile>[TilePropertyInfo.ContinentNames.Length];

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
            TileHelper.maxIndex = new IndexPair(indexRange.X, indexRange.Y);

            for (int y = 0; y < indexRange.Y; y++)
            {
                for (int x = 0; x < indexRange.X; x++)
                {
                    var tile = Instantiate(tilePrefab, tilesRoot);
                    // NOTE : ���� ��ġ ����
                    var pos = tile.transform.localPosition = new Vector3(
                        (x + y * 0.5f - y / 2) * (HexMetrics.innerRadius * 2f),
                        0f,
                        y * (HexMetrics.outerRadius * 1.5f));

                    // NOTE : ��ǥ�� ����
                    tile.Setup(new IndexPair(x, y));
                    tile.color = defaultColor;

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

            //// NOTE : Ÿ�� ���� ������
            var defaultPoint = TileHelper.maxIndex / 2;
            //// NOTE : Ÿ�� ���� �������� �Ź� �ٸ��� �ϱ� ���� �ణ ����
            var firstContinentTileIndex = defaultPoint;

            for(int i = 0; i < allContinentTiles.Length; ++i) {
                allContinentTiles[i] = new List<Tile>();
            }

            CreateRandomContinent(firstContinentTileIndex);

            foreach (var tile in TileModel.tiles) {
                if (allContinentTiles.Any(x => x.Contains(tile))) {
                    tile.setupType(TerrainType.Field, 0);
                    continue;
                }

                tile.setupType(TerrainType.Ocean, 0);
            }

            TileHelper.SetTilesColorToEnvironment();
            TileHelper.ReDrawHexMesh();

            for(int i = 0; i < allContinentTiles.Length; ++i) {
                var currentContinentName = TilePropertyInfo.ContinentNames[i];
                var currentContinent = allContinentTiles[i];
                Debug.Log($"{currentContinentName} : {currentContinent.Count}");
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
                int randomIndex = UnityEngine.Random.Range(0, allContinentTiles.Length);
                if(allContinentTiles[randomIndex].Count == 0) {
                    CreateContinentTilePhase(firstContinentTileIndex, ref allContinentTiles[randomIndex]);
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tile
{
    public class Tile : MonoBehaviour
    {
        public Color color;

        private static GameObject[] tileResources = new GameObject[(int)TerrainType.NumTerrainType];

        [SerializeField] private Transform resourceRoot;
        [SerializeField] private bool isMovable = false;
        [SerializeField] private int moveCost = 1;

        public HexCoordinates Coordinates { private set; get; }
        public IndexPair IndexPair { private set; get; }
        public TerrainType TerrainType { private set; get; }

        // TODO : FeatureType, COntinentType, ClimateType 정리. 캐스팅이 최대한 없어질 수 있도록
        public FeatureType FeatureType { get => featureType; }
        private FeatureType featureType = FeatureType.None;
        public int ContinentType { set => continentType = value; get => continentType; }
        private int continentType = -1;
        public int ClimateType { set => climateType = value; get => climateType; }
        private int climateType = -1;
        public bool IsMovable { get { return isMovable; } }
        public int MoveCost { get { return moveCost; } }
        public int ContinentInfluence { get; set; }

        public void Setup(IndexPair index)
        {
            IndexPair = index;
            Coordinates = HexCoordinates.FromOffsetCoordinates(index.X, index.Y);
        }

        public void SetupTerrainType(TerrainType terrainType) {
            TerrainType = terrainType;
            setupMoveCost(terrainType);
            int terrainTypeIndex = (int)terrainType;

            // NOTE : 리소스 로드
            if (!tileResources[terrainTypeIndex]) {
                tileResources[terrainTypeIndex] = Resources.Load<GameObject>(TileResourceInfo.TileResourcesPath[terrainTypeIndex]);
            }

            // TODO : featureType까지 고려해서 리소스를 붙이도록 하기
            var tileResourceTransform = Instantiate(tileResources[terrainTypeIndex]).transform;
            tileResourceTransform.parent = resourceRoot;
            tileResourceTransform.localPosition = Vector3.zero;
        }

        public void SetupFeatureType(FeatureType featureType) {
            this.featureType = featureType;
        }

        // TODO : 정의는 다른 클래스로 이동
        private void setupMoveCost(TerrainType terrainType)
        {
            switch (terrainType)
            {
                case TerrainType.Field:
                    moveCost = 1;
                    break;
                case TerrainType.Mountain:
                    moveCost = 2;
                    break;
                default:
                    moveCost = 0;
                    break;
            }

            isMovable = moveCost != 0;
        }

#if UNITY_EDITOR
        public void attachResourceFromResourcePath(string path)
        {
            var tileResource = Resources.Load<GameObject>(path);
            var tileResourceTransform = Instantiate(tileResource).transform;
            tileResourceTransform.parent = resourceRoot;
            tileResourceTransform.localPosition = Vector3.zero;
        }
#endif
    }
}
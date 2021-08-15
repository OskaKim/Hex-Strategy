using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tile
{
    public class Tile : MonoBehaviour
    {
        public static readonly float SIZE_X = 1.73f;
        public static readonly float SIZE_Y = 1.50f;

        private static GameObject[] tileResources = new GameObject[(int)TerrainType.NumTerrainType];

        [SerializeField] private Transform renderObject;
        [SerializeField] private GameObject debugGameObject;
        [SerializeField] private TextMesh debugIndexText;
        [SerializeField] private Transform resourceRoot;

        public IndexPair IndexPair { private set; get; }

        public void Setup(IndexPair index, Vector2 pos)
        {
            IndexPair = index;

            var renderPos = new Vector3(pos.x, 0, pos.y);

            if (index.y % 2 == 0)
            {
                renderPos.x -= SIZE_X / 2f;
            }

            renderObject.localPosition = renderPos;

#if UNITY_EDITOR
            DebugTextToIndex();
#endif
        }

#if UNITY_EDITOR
        public void CustomDebugText(string text)
        {
            debugIndexText.text = text;
        }

        public void DebugTextToIndex()
        {
            debugIndexText.text = $"({IndexPair.x}/{IndexPair.y})";
        }
#endif

        public void attachResource(int terrainType, int featureType)
        {
            // NOTE : ���ҽ� �ε�
            if(!tileResources[terrainType])
            {
                tileResources[terrainType] = Resources.Load<GameObject>(TileResourceInfo.TileResourcesPath[terrainType]);
            }

            // TODO : featureType���� ����ؼ� ���ҽ��� ���̵��� �ϱ�
            var tileResourceTransform = Instantiate(tileResources[terrainType]).transform;
            tileResourceTransform.parent = resourceRoot;
            tileResourceTransform.localPosition = Vector3.zero;
            tileResourceTransform.tag = tag;
            tileResourceTransform.gameObject.AddComponent<MeshCollider>();
        }
    }
}
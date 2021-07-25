using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tile
{
    public class Tile : MonoBehaviour
    {
        public static readonly float SIZE_X = 1.73f;
        public static readonly float SIZE_Y = 1.50f;

        [SerializeField] private Transform renderObject;
        [SerializeField] private GameObject debugGameObject;
        [SerializeField] private TextMesh debugIndexText;
        [SerializeField] private Transform resourceRoot;

        public IndexPair IndexPair { private set; get; }

        public void Setup(IndexPair index, Vector2 pos, Transform addedTile3DResource)
        {
            IndexPair = index;

            var renderPos = new Vector3(pos.x, 0, pos.y);

            if (index.y % 2 == 0)
            {
                renderPos.x -= SIZE_X / 2f;
            }

            renderObject.localPosition = renderPos;

            addedTile3DResource.parent = resourceRoot;
            addedTile3DResource.localPosition = Vector3.zero;

#if UNITY_EDITOR
            debugIndexText.text = $"({index.x}/{index.y})";
#endif
        }
    }
}
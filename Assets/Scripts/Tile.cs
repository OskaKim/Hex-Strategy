using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tile
{
    public class Tile : MonoBehaviour
    {
        public static readonly float SIZE = 2.0f;

        [SerializeField]
        private Transform renderObject;

        public IndexPair IndexPair { private set; get; }

        public void Setup(IndexPair index, Vector2 pos)
        {
            IndexPair = index;

            var renderPos = new Vector3(pos.x, 0, pos.y);

            if (index.y % 2 == 0)
            {
                renderPos.x -= SIZE / 2.0f;
            }

            renderObject.localPosition = renderPos;
        }
    }
}
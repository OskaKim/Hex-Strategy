using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tile
{
    public class Tile : MonoBehaviour
    {
        [SerializeField]
        private Transform renderObject;

        public IndexPair IndexPair { private set; get; }

        public void Setup(IndexPair index, Vector2 pos)
        {
            IndexPair = index;

            var renderPos = new Vector3(pos.x, 0, pos.y);

            if(index.y % 2 == 0)
            {
                renderPos.x -= 0.5f;
            }

            renderObject.localPosition = renderPos;
        }
    }
}
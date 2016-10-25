using UnityEngine;
using System.Collections.Generic;

namespace Manager
{
    public class MapManager : MonoBehaviour
    {
        /*Setup variable in editor*/
        [SerializeField]
        int map_width, map_height;

        /*Resources from editor*/
        [SerializeField]
        Transform Grid;

        //list of tiles
        List<Transform> Tiles;
        public List<Transform> pTiles
        {
            get { return Tiles; }
            set { Tiles = value; }
        }

        //scale of image constant
        const float TILEIMAGE_WIDTH = 0.25f;
        const float TILEIMAGE_HEIGHT = 0.3f;

        void Start()
        {
            //Generate Map
            Generators.MapGenerator.InitializeHexMap(Grid, out Tiles, map_width, map_height, TILEIMAGE_WIDTH, TILEIMAGE_HEIGHT);
        }
    }
}
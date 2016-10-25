using UnityEngine;
using System.Collections.Generic;

namespace Generators
{
    /*Use when scene initialized*/
    public class MapGenerator : MonoBehaviour
    {
        //param 1(TilePrefabs) -> all tiles will setup based that tileprefab
        //param 2(myTiles) -> return reference of tiles set up this time.
        //length_x, length_y -> length of map
        public static void InitializeHexMap(Transform TilePrefab, out List<Transform> myTiles, int length_x, int length_y, float width, float height, float center_x = 0, float center_y = 0)
        {
            //make list for return list reference
            List<Transform> _myTiles = new List<Transform>();
            _myTiles.Capacity = length_x * length_y;

            //get position of first tile initialized.
            float first_x, first_y;
            first_x = -length_x * width + center_x + width / 2;
            first_y = -length_y * height / 4 + center_y + height / 2;

            /*set up new tile*/
            for (int y = 0; y < length_y; ++y)
            {
                for (int x = 0; x < length_x; ++x)
                {
                    /*tile object initialize*/
                    Transform newTile = Instantiate(TilePrefab) as Transform;
                    /*set position*/

                    //set offset if y%2==1
                    float offsetx = 0;
                    if (y % 2 == 1)
                        offsetx = width;
                    newTile.position = new Vector2(first_x + x * 2 * width + offsetx, first_y + y * height / 2);
                    newTile.name = x + "-" + y;
                    //add to list of tiles
                    _myTiles.Add(newTile);
                }
            }

            //reference of tile list
            myTiles = _myTiles;
        }
    }
}
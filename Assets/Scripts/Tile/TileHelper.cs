using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace Tile
{
    public class TileModel : MonoBehaviour
    {
        public static readonly List<Tile> tiles = new List<Tile>();
        public static readonly List<Text> tileLabels = new List<Text>();
        public static HexMesh hexMesh;

        public static void ClearAll()
        {
            GameObjectUtility.ClearComponentListAndDelete(tiles);
            GameObjectUtility.ClearComponentListAndDelete(tileLabels);
            GameObjectUtility.DeleteGameObjectsFromTags(new string[] { "Tile", "TileUI" });
            if (hexMesh == null) hexMesh = FindObjectOfType<HexMesh>();
            if (hexMesh != null) hexMesh.Clear();
        }
    }

    public static class TileHelper
    {
        public static int maxIndexX { get; private set; }
        public static int maxIndexY { get; private set; }
        public static IndexPair maxIndex
        {
            get
            {
                return new IndexPair(maxIndexX, maxIndexY);
            }
            set
            {
                maxIndexX = value.x;
                maxIndexY = value.y;
            }
        }

        public static Tile GetTile(HexCoordinates coordinates)
        {
            return TileModel.tiles.FirstOrDefault(x => x.coordinates == coordinates);
        }

        public static Tile GetTile(IndexPair indexPair)
        {
            return TileModel.tiles.FirstOrDefault(x => x.IndexPair == indexPair);
        }

        public static Tile[] GetNearTiles(Tile tile)
        {
            var x = tile.coordinates.X;
            var z = tile.coordinates.Z;

            return new Tile[]
            {
               GetTile(new HexCoordinates(x,    z + 1)),
               GetTile(new HexCoordinates(x,    z - 1)),
               GetTile(new HexCoordinates(x - 1,z    )),
               GetTile(new HexCoordinates(x - 1,z + 1)),
               GetTile(new HexCoordinates(x + 1,z - 1)),
               GetTile(new HexCoordinates(x + 1,z    )),
            }.Where(x => x != null).ToArray();
        }

        public static void ClearAllTiles()
        {
            TileModel.ClearAll();
        }

#if UNITY_EDITOR
        public static void ClearAllDebugText()
        {
            var tiles = TileModel.tiles;

            tiles.ForEach(x => x.CustomDebugText(""));
        }
#endif
    }
}
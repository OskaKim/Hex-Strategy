using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

namespace Tile {
    public class TileModel : MonoBehaviour {
        public static readonly List<Tile> tiles = new List<Tile>();
        public static readonly List<Text> tileLabels = new List<Text>();
        public static HexMesh hexMesh;

        public static void ClearAll() {
            GameObjectUtility.ClearComponentListAndDelete(tiles);
            GameObjectUtility.ClearComponentListAndDelete(tileLabels);
            GameObjectUtility.DeleteGameObjectsFromTags(new string[] { "Tile", "TileUI" });
            if (hexMesh == null) hexMesh = FindObjectOfType<HexMesh>();
            TileHelper.ClearHexMesh();
        }
    }

    public static class TileHelper {
        public static int maxIndexX { get; private set; }
        public static int maxIndexY { get; private set; }
        public static IndexPair maxIndex {
            get {
                return new IndexPair(maxIndexX, maxIndexY);
            }
            set {
                maxIndexX = value.X;
                maxIndexY = value.Y;
            }
        }

        public static Tile GetTile(HexCoordinates coordinates) {
            return TileModel.tiles.FirstOrDefault(x => x.Coordinates == coordinates);
        }

        public static Tile GetTile(IndexPair indexPair) {
            return TileModel.tiles.FirstOrDefault(x => x.IndexPair == indexPair);
        }

        public static Tile[] GetNearTiles(Tile tile) {
            var x = tile.Coordinates.X;
            var z = tile.Coordinates.Z;

            return new Tile[] {
               GetTile(new HexCoordinates(x,    z + 1)),
               GetTile(new HexCoordinates(x,    z - 1)),
               GetTile(new HexCoordinates(x - 1,z    )),
               GetTile(new HexCoordinates(x - 1,z + 1)),
               GetTile(new HexCoordinates(x + 1,z - 1)),
               GetTile(new HexCoordinates(x + 1,z    )),
            }.Where(x => x != null).ToArray();
        }

        public static Tile[] GetNearTilesRandomSorted(Tile tile) {
            return GetNearTiles(tile).OrderBy(g => Guid.NewGuid()).ToArray();
        }

        public static void ClearAllTiles() {
            TileModel.ClearAll();
        }

        public static void ClearHexMesh() {
            if (!TileModel.hexMesh) return;
            TileModel.hexMesh.Clear();
        }
        public static void ReDrawHexMesh() {
            if (!TileModel.hexMesh) return;
            TileModel.hexMesh.Triangulate();
        }

        public static void InitHexMesh() {
            if (!TileModel.hexMesh) return;
            TileModel.hexMesh.MeshAlpha = 0.0f;
            ReDrawHexMesh();
        }

        public static void SetTilesColorToEnvironment() {
            TileModel.tiles.ForEach(x => x.color = TileColorDefinitions.GetEnvironmentColor(x.TerrainType));
            TileModel.hexMesh.MeshAlpha = 0.5f;
            ReDrawHexMesh();
        }

        public static void SetTilesColorToFeatureType() {
            TileModel.tiles.ForEach(x => x.color = TileColorDefinitions.GetFeatureTypeColor(x.FeatureType));
            TileModel.hexMesh.MeshAlpha = 0.5f;
            ReDrawHexMesh();
        }

        public static void SetTilesColorToContinent() {
            TileModel.tiles.ForEach(x => x.color = TileColorDefinitions.GetContinentColor(x.ContinentType));
            TileModel.hexMesh.MeshAlpha = 0.5f;
            ReDrawHexMesh();
        }

        public static void SetTilesColorToClimate() {
            TileModel.tiles.ForEach(x => x.color = TileColorDefinitions.GetClimateColor(x.ClimateType));
            TileModel.hexMesh.MeshAlpha = 0.5f;
            ReDrawHexMesh();
        }
    }
}
using UnityEngine;

namespace Tile {
    public class TileColorDefinitions {
        public static Color GetEnvironmentColor(TerrainType terrainType) {
            return environmentColors[(int)terrainType];
        }
        private static Color[] environmentColors = {
            new Color(0,0.5f,0,1),
            new Color(0,1,0,1),
            new Color(0,0,0.8f,1),
        };
    }
}
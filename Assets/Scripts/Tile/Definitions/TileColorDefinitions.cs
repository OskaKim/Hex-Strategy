using UnityEngine;

namespace Tile {
    public class TileColorDefinitions {
        public static Color GetEnvironmentColor(TerrainType terrainType) {
            return environmentColors[(int)terrainType];
        }
        public static float GetEnvironmentAlpha() {
            return 0.5f;
        }
        private static Color[] environmentColors = {
            new Color(0,0.5f,0,1),
            new Color(0,1,0,1),
            new Color(0,0,0.8f,1),
        };
        public static Color GetContinentColor(int continentType) {
            if (continentType == -1) return new Color(0, 0, 0, 1);
            return continentColor[continentType];
        }
        private static Color[] continentColor = {
            new Color(1,0,0,1),
            new Color(0,1,0,1),
            new Color(0,0,1,1),
            new Color(1,1,0,1),
            new Color(0,1,1,1),
            new Color(1,1,1,1),
        };
    }
}
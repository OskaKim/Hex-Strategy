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
        public static Color GetClimateColor(int climateType) {
            if (climateType == -1) return new Color(0, 0, 0, 1);
            return climateColor[climateType];
        }
        private static Color[] climateColor = {
            new Color(0.9f, 0.99f, 0.83f),  // NOTE : 한대기후
            new Color(0.16f, 0.81f, 0.83f), // NOTE : 냉대기후
            new Color(0.98f, 0.78f, 0.38f), // NOTE : 온대기후
            new Color(0.87f, 0.2f, 0.06f)   // NOTE : 열대기후
        };
    }
}
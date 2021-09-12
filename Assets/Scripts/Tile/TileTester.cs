using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Tile
{
    public class TileTester : MonoBehaviour
    {
        [SerializeField] IndexPair getIndex;
        [SerializeField] MyCamera.CameraModel cameraModel;
        [SerializeField] public HexCoordinates findPathStart;
        [SerializeField] public HexCoordinates findPathEnd;
        [SerializeField] public List<Tile> path;

        void Start() {
            TileInputHandler.GetInstance().ClickOnceEvent += ClickOnce;
            TileInputHandler.GetInstance().ClickContinuingEvent += ClickContinuing;
        }

        void OnDestroy() {
            TileInputHandler.GetInstance().ClickOnceEvent -= ClickOnce;
            TileInputHandler.GetInstance().ClickContinuingEvent -= ClickContinuing;
        }

        private void ClickOnce(int button, Tile clickedTile) {
            if (button == 0) findPathStart = clickedTile.Coordinates;
        }
        private void ClickContinuing(int button, Tile clickedTile) {
            if (button == 1) {
                findPathEnd = clickedTile.Coordinates;
                FindPath();
            }
        }

        public void FindPath() {

            var startTile = TileHelper.GetTile(findPathStart);
            var endTile = TileHelper.GetTile(findPathEnd);

            PathFinderManager.StartPlayerPathFinding(startTile, endTile, (outPath) => {
                path = outPath;
                TileHelper.SetTilesColorToEnvironment();

                float strengthPerPath = 1.0f / outPath.Count;
                int cnt = 0;
                foreach (var path in path) {
                    path.color = new Color(strengthPerPath * ++cnt, 0, 0, 1);
                }
                TileHelper.ReDrawHexMesh();
            });
        }
    }
}
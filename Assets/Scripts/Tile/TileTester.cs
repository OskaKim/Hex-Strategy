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
            TileInputHandler.GetInstance().ClickEvent += TileTester_ClickEvent;
        }

        void OnDestroy() {
            TileInputHandler.GetInstance().ClickEvent -= TileTester_ClickEvent;
        }

        private void TileTester_ClickEvent(int button, Tile clickedTile) {
            if (button == 0) findPathStart = clickedTile.Coordinates;
            else if (button == 1) {
                findPathEnd = clickedTile.Coordinates;
                FindPath();
            }
        }
        public void FindPath() {

            var startTile = TileHelper.GetTile(findPathStart);
            var endTile = TileHelper.GetTile(findPathEnd);
            path = PathFinding.FindPath(startTile, endTile);

            TileHelper.SetTilesColorToEnvironment();

            float strengthPerPath = 1.0f / path.Count;
            int cnt = 0;
            foreach (var path in path) {
                path.color = new Color(strengthPerPath * ++cnt, 0, 0, 1);
            }
            TileHelper.ReDrawHexMesh();
        }
    }
}
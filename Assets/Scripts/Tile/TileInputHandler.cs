using UnityEngine;
using UniRx;

namespace Tile {
    public class TileInputHandler : MonoBehaviour {
        public Color defaultColor = Color.white;
        public Color touchedColor = Color.magenta;

        void Start() {
            Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButtonUp(0))
                .Subscribe(_ => {
                    HandleInput();
                });
        }

        void HandleInput() {
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit)) {
                TouchCell(hit.point);
            }
        }

        public void TouchCell(Vector3 position) {
            position = transform.InverseTransformPoint(position);
            var clickedTile = TileHelper.GetTile(HexCoordinates.FromPosition(position));
            if (!clickedTile) return;

            clickedTile.color = touchedColor;

            TileHelper.GetNearTiles(clickedTile).ForEach(x => x.color = Color.green);
            TileHelper.ReDrawHexMesh();
        }
    }
}
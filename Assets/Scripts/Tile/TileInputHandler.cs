using UnityEngine;
using UniRx;
using System;

namespace Tile {
    public class TileInputHandler : MonoBehaviour {
        private static TileInputHandler instance;
        public static TileInputHandler GetInstance() { return instance; }
        public event Action<int, Tile> ClickEvent;

        void Start() {
            instance = this;

            Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButtonDown(0))
                .Subscribe(_ => {
                    HandleInput(0);
                });

            Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButtonDown(1))
                .Subscribe(_ => {
                    HandleInput(1);
                });
        }

        void HandleInput(int button) {
            Debug.Log($"clicked Button {button}");
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit)) {
                TouchCell(button, hit.point);
            }
        }

        public void TouchCell(int button, Vector3 position) {
            position = transform.InverseTransformPoint(position);
            var clickedTile = TileHelper.GetTile(HexCoordinates.FromPosition(position));
            if (clickedTile != null) {
                ClickEvent(button, clickedTile);
            }
        }
    }
}
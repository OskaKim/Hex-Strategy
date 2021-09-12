using UnityEngine;
using UniRx;
using System;

namespace Tile {
    public class TileInputHandler : MonoBehaviour {
        private static TileInputHandler instance;
        public static TileInputHandler GetInstance() { return instance; }
        public event Action<int, Tile> ClickOnceEvent;
        public event Action<int, Tile> ClickContinuingEvent;

        private void Awake() {
            instance = this;
        }

        void Start() {
            Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButtonDown(0))
                .Subscribe(_ => {
                    HandleInput(0, true);
                });

            Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButtonDown(1))
                .Subscribe(_ => {
                    HandleInput(1, true);
                });

            Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButton(0))
                .Subscribe(_ => {
                    HandleInput(0, false);
                });

            Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButton(1))
                .Subscribe(_ => {
                    HandleInput(1, false);
                });
        }

        private void HandleInput(int button, bool isOnce) {
            Debug.Log($"clicked Button {button}");
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit)) {
                TouchCell(button, hit.point, isOnce);
            }
        }

        public void TouchCell(int button, Vector3 position, bool isOnce) {
            position = transform.InverseTransformPoint(position);
            var clickedTile = TileHelper.GetTile(HexCoordinates.FromPosition(position));
            if (clickedTile != null) {
                if (isOnce) {
                    ClickOnceEvent(button, clickedTile);
                }
                else {
                    ClickContinuingEvent(button, clickedTile);
                }
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Tile
{
    // TODO : 유니티 디버그 기능으로 확인할 수 있도록 개수
    public class TileTester : MonoBehaviour
    {
        [SerializeField] IndexPair getIndex;
        [SerializeField] MyCamera.CameraModel cameraModel;

        private List<Tile> cacheTiles = new List<Tile>();

        private void Start()
        {
            // TODO : 타일 클릭 테스트
            //var clickStream = Observable.EveryUpdate()
            //.Where(_ => Input.GetMouseButtonDown(0))
            //.Subscribe(_ =>
            //{
            //    var clickedObject = InputHelper.GetGameObjectFromScreenPointToRay(Input.mousePosition);

            //    if (clickedObject) {
            //        cameraModel.SetTarget(clickedObject.transform);
            //    }
            //});

            var showNearTile = Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.F))
            .Subscribe(_ =>
            {
                foreach (var cacheTile in cacheTiles)   
                {
                    cacheTile.DebugTextToIndex();
                }
                cacheTiles.Clear();

                var tile = TileHelper.GetTile(getIndex);
                tile.CustomDebugText("target");
                cacheTiles.Add(tile);

                foreach (var nearTile in TileHelper.GetNearTiles(tile))
                {
                    Debug.Log($"{nearTile.IndexPair.x} / {nearTile.IndexPair.y})");
                    nearTile.CustomDebugText("near tile");
                    cacheTiles.Add(nearTile);
                }
            });
        }
    }
}